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



  }

}