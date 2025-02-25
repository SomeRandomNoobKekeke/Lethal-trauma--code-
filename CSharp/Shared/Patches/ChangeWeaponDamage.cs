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
  public class ChangeWeaponDamage
  {
    public static void Initialize()
    {
      // Mod.Harmony.Patch(
      //   original: typeof(CampaignMode).GetMethod("AddExtraMissions", AccessTools.all),
      //   postfix: new HarmonyMethod(typeof(AddMoreMissions).GetMethod("CampaignMode_AddExtraMissions_Postfix"))
      // );
    }



  }

}