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
  public class PressureKillDelay
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Character).GetConstructors(AccessTools.all)[1],
        postfix: new HarmonyMethod(typeof(PressureKillDelay).GetMethod("Character_Constructor_Postfix"))
      );
    }

    public static float DefaultPressureKillDelay = 5.0f;


    [Dependency] public static ConfigProxy Config { get; set; }
    [Dependency] public static Logger Logger { get; set; }

    public static float GlobalPressureKillDelay
    {
      set
      {
        foreach (Character character in Character.CharacterList)
        {
          character.CharacterHealth.PressureKillDelay = value;
        }
      }
    }

    public static void AfterInjectStatic()
    {
      Config.PressureKillDelayChanged += (b) =>
      {
        GlobalPressureKillDelay = b;
      };
    }

    public static void Character_Constructor_Postfix(Character __instance)
    {
      if (Config == null) return;
      __instance.CharacterHealth.PressureKillDelay = Config.PressureKillDelay;
    }



  }

}