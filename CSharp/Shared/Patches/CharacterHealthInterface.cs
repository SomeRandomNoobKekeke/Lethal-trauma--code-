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
    private static void ResetHumanPrefabHealthMultiplier(Character human)
    {
      if (human.humanPrefab != null)
      {
        human.HumanPrefabHealthMultiplier = human.humanPrefab.HealthMultiplier;
        if (GameMain.NetworkMember != null)
        {
          human.HumanPrefabHealthMultiplier *= human.humanPrefab.HealthMultiplierInMultiplayer;
        }
      }
      else
      {
        human.HumanPrefabHealthMultiplier = 1.0f;
      }
    }

    private static bool useHumanPrefabHealthMultiplier = true;
    public static bool UseHumanPrefabHealthMultiplier
    {
      get => useHumanPrefabHealthMultiplier;
      set
      {
        if (useHumanPrefabHealthMultiplier == value) return;
        useHumanPrefabHealthMultiplier = value;

        foreach (Character character in Character.CharacterList)
        {
          if (character.IsHuman)
          {
            if (value) ResetHumanPrefabHealthMultiplier(character);
            else character.HumanPrefabHealthMultiplier = 1.0f;
          }
        }
      }
    }

    [Dependency] public static ConfigProxy Config { get; set; }

    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Character).GetConstructors(AccessTools.all)[1],
        postfix: new HarmonyMethod(typeof(CharacterHealthInterface).GetMethod("Character_Constructor_Postfix"))
      );
    }


    public static void Character_Constructor_Postfix(Character __instance)
    {
      if (Config == null) return;
      if (Config.OverrideHealthMult)
      {
        if (__instance.IsHuman)
        {
          __instance.HumanPrefabHealthMultiplier = Config.HumanHealth;
        }
        else
        {
          __instance.HumanPrefabHealthMultiplier = Config.MonsterHealth;
        }
      }
    }
  }

}