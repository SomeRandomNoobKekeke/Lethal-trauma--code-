using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Barotrauma.Items.Components;
using LTDependencyInjection;


namespace Lethaltrauma
{
  [PatchClass]
  public class CrimesInMask
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(HumanAIController).GetMethod("ApplyStealingReputationLoss", AccessTools.all),
        prefix: new HarmonyMethod(typeof(CrimesInMask).GetMethod("HumanAIController_ApplyStealingReputationLoss_Prefix"))
      );

      Mod.Harmony.Patch(
        original: typeof(HumanAIController).GetMethod("StructureDamaged", AccessTools.all),
        prefix: new HarmonyMethod(typeof(CrimesInMask).GetMethod("HumanAIController_StructureDamaged_Replace"))
      );

      Mod.Harmony.Patch(
        original: typeof(CampaignMode).GetMethod("OutpostNPCAttacked", AccessTools.all),
        prefix: new HarmonyMethod(typeof(CrimesInMask).GetMethod("CampaignMode_OutpostNPCAttacked_Prefix"))
      );
    }

    [Dependency] public static ConfigProxy Config { get; set; }

    public static bool CampaignMode_OutpostNPCAttacked_Prefix(Character npc, Character attacker, AttackResult attackResult)
    {
      if (Config?.NoReputationLossInMask == true && attacker.HideFace) return false;
      return true;
    }

    public static bool HumanAIController_ApplyStealingReputationLoss_Prefix(Item item)
    {
      if (Config?.NoReputationLossInMask == true && Character.Controlled?.HideFace == true) return false;
      return true;
    }


    public static bool HumanAIController_StructureDamaged_Replace(HumanAIController __instance, Structure structure, float damageAmount, Character character)
    {
      const float MaxDamagePerSecond = 5.0f;
      const float MaxDamagePerFrame = MaxDamagePerSecond * (float)Timing.Step;

      const float WarningThreshold = 5.0f;
      const float ArrestThreshold = 20.0f;
      const float KillThreshold = 50.0f;

      if (character == null || damageAmount <= 0.0f) { return false; }
      if (structure?.Submarine == null || !structure.Submarine.Info.IsOutpost || character.TeamID == structure.Submarine.TeamID) { return false; }
      //structure not indestructible = something that's "meant" to be destroyed, like an ice wall in mines
      if (!structure.Prefab.IndestructibleInOutposts) { return false; }

      bool someoneSpoke = false;
      float maxAccumulatedDamage = 0.0f;
      foreach (Character otherCharacter in Character.CharacterList)
      {
        if (otherCharacter == character || otherCharacter.TeamID == character.TeamID || otherCharacter.IsDead ||
            otherCharacter.Info?.Job == null ||
            otherCharacter.AIController is not HumanAIController otherHumanAI ||
            Vector2.DistanceSquared(otherCharacter.WorldPosition, character.WorldPosition) > 1000.0f * 1000.0f)
        {
          continue;
        }
        if (!otherCharacter.CanSeeTarget(character, seeThroughWindows: true)) { continue; }

        otherHumanAI.structureDamageAccumulator.TryAdd(character, 0.0f);
        float prevAccumulatedDamage = otherHumanAI.structureDamageAccumulator[character];
        otherHumanAI.structureDamageAccumulator[character] += MathHelper.Clamp(damageAmount, -MaxDamagePerFrame, MaxDamagePerFrame);
        float accumulatedDamage = Math.Max(otherHumanAI.structureDamageAccumulator[character], maxAccumulatedDamage);
        maxAccumulatedDamage = Math.Max(accumulatedDamage, maxAccumulatedDamage);

        if (GameMain.GameSession?.Campaign?.Map?.CurrentLocation?.Reputation != null && character.IsPlayer)
        {
          if (Config == null || !Config.NoReputationLossInMask || !character.HideFace)
          {
            var reputationLoss = damageAmount * Reputation.ReputationLossPerWallDamage;
            GameMain.GameSession.Campaign.Map.CurrentLocation.Reputation.AddReputation(-reputationLoss, Reputation.MaxReputationLossFromWallDamage);
          }
        }

        if (!character.IsCriminal)
        {
          if (accumulatedDamage <= WarningThreshold) { return false; }

          if (accumulatedDamage > WarningThreshold && prevAccumulatedDamage <= WarningThreshold &&
              !someoneSpoke && !character.IsIncapacitated && character.Stun <= 0.0f)
          {
            //if the damage is still fairly low, wait and see if the character keeps damaging the walls to the point where we need to intervene
            if (accumulatedDamage < ArrestThreshold)
            {
              if (otherHumanAI.ObjectiveManager.CurrentObjective is AIObjectiveIdle idleObjective)
              {
                idleObjective.FaceTargetAndWait(character, 5.0f);
              }
            }
            otherCharacter.Speak(TextManager.Get("dialogdamagewallswarning").Value, null, Rand.Range(0.5f, 1.0f), "damageoutpostwalls".ToIdentifier(), 10.0f);
            someoneSpoke = true;
          }
        }

        // React if we are security
        if (character.IsCriminal ||
            (accumulatedDamage > ArrestThreshold && prevAccumulatedDamage <= ArrestThreshold) ||
            (accumulatedDamage > KillThreshold && prevAccumulatedDamage <= KillThreshold))
        {
          var combatMode = accumulatedDamage > KillThreshold ? AIObjectiveCombat.CombatMode.Offensive : AIObjectiveCombat.CombatMode.Arrest;
          if (combatMode == AIObjectiveCombat.CombatMode.Offensive)
          {
            character.IsCriminal = true;
          }
          if (!TriggerSecurity(otherHumanAI, combatMode))
          {
            // Else call the others
            foreach (Character security in Character.CharacterList.Where(c => c.TeamID == otherCharacter.TeamID).OrderBy(c => Vector2.DistanceSquared(character.WorldPosition, c.WorldPosition)))
            {
              if (!TriggerSecurity(security.AIController as HumanAIController, combatMode))
              {
                // Only alert one guard at a time
                return false;
              }
            }
          }
        }
      }

      bool TriggerSecurity(HumanAIController humanAI, AIObjectiveCombat.CombatMode combatMode)
      {
        if (humanAI == null) { return false; }
        if (!humanAI.Character.IsSecurity) { return false; }
        if (humanAI.ObjectiveManager.IsCurrentObjective<AIObjectiveCombat>()) { return false; }
        humanAI.AddCombatObjective(combatMode, character, delay: HumanAIController.GetReactionTime(),
            onCompleted: () =>
            {
              //if the target is arrested successfully, reset the damage accumulator
              foreach (Character anyCharacter in Character.CharacterList)
              {
                if (anyCharacter.AIController is HumanAIController anyAI)
                {
                  anyAI.structureDamageAccumulator?.Remove(character);
                }
              }
            });
        return true;
      }

      return false;
    }



  }

}