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
  public class CharacterHealthInterface
  {
    public static void UpdateHealthMultipliers()
    {
      foreach (Character character in Character.CharacterList)
      {
        if (Config.OverrideHealthMult)
        {
          if (character.IsHuman)
          {
            character.HumanPrefabHealthMultiplier = Config.HumanHealth;
          }
          else
          {
            character.HumanPrefabHealthMultiplier = Config.MonsterHealth;
          }
        }
        else
        {
          if (character.humanPrefab != null)
          {
            character.HumanPrefabHealthMultiplier = character.humanPrefab.HealthMultiplier;
            if (GameMain.NetworkMember != null)
            {
              character.HumanPrefabHealthMultiplier *= character.humanPrefab.HealthMultiplierInMultiplayer;
            }
          }
          else
          {
            character.HumanPrefabHealthMultiplier = 1.0f;
          }
        }
      }
    }


    [Dependency] public static ConfigProxy Config { get; set; }
    [Dependency] public static ConfigManager ConfigManager { get; set; }

    public static void AfterInjectStatic()
    {
      ConfigManager.ConfigChanged += UpdateHealthMultipliers;
      Config.OverrideHealthMultChanged += (v) => UpdateHealthMultipliers();
      Config.HumanHealthChanged += (v) => UpdateHealthMultipliers();
      Config.MonsterHealthChanged += (v) => UpdateHealthMultipliers();
    }

    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Character).GetConstructors(AccessTools.all)[0],
        postfix: new HarmonyMethod(typeof(CharacterHealthInterface).GetMethod("Character_Constructor_Postfix"))
      );
    }


    public static void Character_Constructor_Postfix(Character __instance)
    {
      try
      {
        if (Config == null || !Config.OverrideHealthMult) return;

        if (__instance.IsHuman)
        {
          __instance.HumanPrefabHealthMultiplier = Config.HumanHealth;
        }
        else
        {
          __instance.HumanPrefabHealthMultiplier = Config.MonsterHealth;
        }
      }
      catch (Exception e)
      {
        Mod.Log($"Character_Constructor_Postfix threw {e.Message}, Character:[{__instance}]");
      }
    }
  }

}