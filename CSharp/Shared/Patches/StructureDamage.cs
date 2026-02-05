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
  public class StructureDamage
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Structure).GetMethod("AddDamage", AccessTools.all, new Type[]{
          typeof(Character),
          typeof(Vector2),
          typeof(Attack),
          typeof(Vector2),
          typeof(float),
          typeof(bool),
        }),
        prefix: new HarmonyMethod(typeof(StructureDamage).GetMethod("Structure_AddDamage_Prefix"))
      );

      Mod.Harmony.Patch(
        original: typeof(Item).GetMethod("AddDamage", AccessTools.all),
        prefix: new HarmonyMethod(typeof(StructureDamage).GetMethod("Item_AddDamage_Prefix"))
      );

      // Mod.Harmony.Patch(
      //   original: typeof(Limb).GetMethod("UpdateAttack", AccessTools.all),
      //   prefix: new HarmonyMethod(typeof(StructureDamage).GetMethod("Limb_UpdateAttack_Prefix"))
      // );

      // Mod.Harmony.Patch(
      //   original: typeof(Limb).GetMethod("UpdateAttack", AccessTools.all),
      //   postfix: new HarmonyMethod(typeof(StructureDamage).GetMethod("Limb_UpdateAttack_Postfix"))
      // );

      Mod.Harmony.Patch(
        original: typeof(Explosion).GetMethod("RangedStructureDamage", AccessTools.all),
        prefix: new HarmonyMethod(typeof(StructureDamage).GetMethod("Explosion_RangedStructureDamage_Prefix"))
      );
    }

    [Dependency] public static ConfigProxy Config { get; set; }
    [Dependency] public static Logger Logger { get; set; }

    public static void Explosion_RangedStructureDamage_Prefix(ref float damage, Character attacker)
    {
      if (attacker is null) return;
      if (attacker.IsHuman)
      {
        damage *= Config.WeaponStructureDamage; // bruh
      }
      else
      {
        damage *= Config.MonsterStructureDamage;
      }
    }

    public static void Limb_UpdateAttack_Prefix()
    {
      SharedState.FromLimbUpdateAttack = true;
    }
    public static void Limb_UpdateAttack_Postfix()
    {
      SharedState.FromLimbUpdateAttack = false;
    }

    public static void Structure_AddDamage_Prefix(Structure __instance, ref AttackResult __result, Character attacker, Vector2 worldPosition, Attack attack, Vector2 impulseDirection, float deltaTime, bool playSound = false)
    {
      if (attacker is null) return;
      if (attacker.IsHuman)
      {
        attack.DamageMultiplier = Config.WeaponStructureDamage;
      }
      else
      {
        attack.DamageMultiplier = Config.MonsterStructureDamage;
      }
    }

    public static void Item_AddDamage_Prefix(Item __instance, ref AttackResult __result, Character attacker, Vector2 worldPosition, Attack attack, Vector2 impulseDirection, float deltaTime, bool playSound = true)
    {
      if (attacker is null) return;

      bool isDoor = __instance.GetComponent<Door>() != null;

      if (isDoor)
      {
        if (attacker.IsHuman)
        {
          attack.DamageMultiplier = Config.WeaponStructureDamage;
        }
        else
        {
          attack.DamageMultiplier = Config.MonsterStructureDamage;
        }
      }
    }

  }
}