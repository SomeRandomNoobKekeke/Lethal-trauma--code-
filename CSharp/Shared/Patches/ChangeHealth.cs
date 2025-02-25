using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;


namespace Lethaltrauma
{
  [PatchClass]
  public class ChangeHealth
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(CharacterHealth).GetMethod("get_MaxVitality", AccessTools.all),
        prefix: new HarmonyMethod(typeof(ChangeHealth).GetMethod("CharacterHealth_get_MaxVitality_Replace"))
      );
    }



    public static Logger Logger => Mod.Instance?.Logger;


    public static bool CharacterHealth_get_MaxVitality_Replace(CharacterHealth __instance, ref float __result)
    {
      __result = __instance.MaxVitality;
      Logger?.Log(__instance);
      return false;
    }
  }

}