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
  public class GiveWeapons
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(HumanPrefab).GetMethod("GiveItems", AccessTools.all),
        postfix: new HarmonyMethod(typeof(PressureKillDelay).GetMethod("Character_Constructor_Postfix"))
      );
    }



    [Dependency] public static ConfigProxy Config { get; set; }




    public static bool HumanPrefab_GiveItems_Replace(HumanPrefab __instance, ref bool __result, Character character, Submarine submarine, WayPoint spawnPoint, Rand.RandSync randSync = Rand.RandSync.Unsynced, bool createNetworkEvents = true)
    {
      HumanPrefab _ = __instance;

      if (_.ItemSets == null || !_.ItemSets.Any()) { __result = false; return false; }
      var spawnItems = ToolBox.SelectWeightedRandom(_.ItemSets, it => it.commonness, randSync).element;
      if (spawnItems != null)
      {
        foreach (ContentXElement itemElement in spawnItems.GetChildElements("item"))
        {
          int amount = itemElement.GetAttributeInt("amount", 1);
          for (int i = 0; i < amount; i++)
          {
            HumanPrefab.InitializeItem(character, itemElement, submarine, _, spawnPoint, createNetworkEvents: createNetworkEvents);
          }
        }
      }
      __result = true; return false;
    }

  }

}