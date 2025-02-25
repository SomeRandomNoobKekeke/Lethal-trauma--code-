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
        prefix: new HarmonyMethod(typeof(GiveWeapons).GetMethod("HumanPrefab_GiveItems_Replace"))
      );
    }


    [Dependency] public static ConfigProxy Config { get; set; }


    public static bool HumanPrefab_GiveItems_Replace(HumanPrefab __instance, ref bool __result, Character character, Submarine submarine, WayPoint spawnPoint, Rand.RandSync randSync = Rand.RandSync.Unsynced, bool createNetworkEvents = true)
    {
      HumanPrefab _ = __instance;

      if (_.ItemSets == null || !_.ItemSets.Any()) { __result = false; return false; }

      ContentXElement spawnItems;

      if (_.Tags.Contains("dependsondifficulty"))
      {
        float difficulty = GameMain.GameSession.LevelData.Difficulty;

        double minDistance = 1000;
        int minIndex = -1;

        for (int i = 0; i < _.ItemSets.Count; i++)
        {
          double distance = Math.Abs(difficulty - _.ItemSets[i].commonness);

          if (distance < minDistance)
          {
            minDistance = distance;
            minIndex = i;
          }
        }

        spawnItems = _.ItemSets[minIndex].element;
      }
      else
      {
        spawnItems = ToolBox.SelectWeightedRandom(_.ItemSets, it => it.commonness, randSync).element;
      }

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