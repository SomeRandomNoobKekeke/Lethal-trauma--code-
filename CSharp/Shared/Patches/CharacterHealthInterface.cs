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
    /// <summary>
    /// Probably should be in a separate util class, but i'm a bit lazy
    /// </summary>
    public static void SetHealthMilt(Character character)
    {
      if (character.IsHuman)
      {
        if (character.TeamID == CharacterTeamType.Team1)
        {
          character.HumanPrefabHealthMultiplier = Config.CrewHumanHealth;
        }
        else
        {
          character.HumanPrefabHealthMultiplier = Config.EnemyHumanHealth;
        }
      }
      else
      {
        character.HumanPrefabHealthMultiplier = Config.MonsterHealth;
      }
    }
    public static void ResetHealthMilt(Character character)
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


    public static void UpdateHealthMultipliers()
    {
      foreach (Character character in Character.CharacterList)
      {
        if (Config.OverrideHealthMult)
        {
          SetHealthMilt(character);
        }
        else
        {
          ResetHealthMilt(character);
        }
      }
    }


    [Dependency] public static ConfigProxy Config { get; set; }
    [Dependency] public static ConfigManager ConfigManager { get; set; }

    public static void AfterInjectStatic()
    {
      ConfigManager.ConfigChanged += UpdateHealthMultipliers;
      Config.OverrideHealthMultChanged += (v) => UpdateHealthMultipliers();
      Config.CrewHumanHealthChanged += (v) => UpdateHealthMultipliers();
      Config.EnemyHumanHealthChanged += (v) => UpdateHealthMultipliers();
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
        GameMain.LuaCs.Timer.Wait((object[] args) => SetHealthMilt(__instance), 100);
      }
      catch (Exception e)
      {
        Mod.Log($"Character_Constructor_Postfix threw {e.Message}, Character:[{__instance}]");
      }
    }
  }

}