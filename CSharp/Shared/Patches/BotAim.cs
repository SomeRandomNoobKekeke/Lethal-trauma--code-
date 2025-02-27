using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTDependencyInjection;

namespace Lethaltrauma
{
  [PatchClass]
  public class BotAim
  {
    public static void UpdateAim()
    {
      foreach (Character character in Character.CharacterList)
      {
        if (Config.OverrideHealthMult)
        {
          if (character.AIController is HumanAIController humanAI)
          {
            humanAI.AimSpeed = Config.AimSpeed;
            humanAI.AimAccuracy = Config.AimAccuracy;
          }
        }
        else
        {
          if (character.humanPrefab != null)
          {
            if (character.AIController is HumanAIController humanAI)
            {
              humanAI.AimSpeed = character.humanPrefab.AimSpeed;
              humanAI.AimAccuracy = character.humanPrefab.AimAccuracy;
            }
          }
        }
      }
    }

    [Dependency] public static ConfigProxy Config { get; set; }

    public static void AfterInjectStatic()
    {
      Config.OverrideAimChanged += (v) => UpdateAim();
      Config.AimAccuracyChanged += (v) => UpdateAim();
      Config.AimSpeedChanged += (v) => UpdateAim();
    }

    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(HumanPrefab).GetMethod("InitializeCharacter", AccessTools.all),
        postfix: new HarmonyMethod(typeof(BotAim).GetMethod("HumanPrefab_InitializeCharacter_Postfix"))
      );
    }


    public static void HumanPrefab_InitializeCharacter_Postfix(HumanPrefab __instance, Character npc, ISpatialEntity positionToStayIn = null)
    {
      if (Config == null || !Config.OverrideAim) return;

      if (npc.AIController is HumanAIController humanAI)
      {
        humanAI.AimSpeed = Config.AimSpeed;
        humanAI.AimAccuracy = Config.AimAccuracy;
      }
    }
  }

}