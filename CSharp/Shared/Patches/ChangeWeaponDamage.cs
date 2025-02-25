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
  public class ChangeWeaponDamage
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Character).GetMethod("ApplyAttack", AccessTools.all),
        prefix: new HarmonyMethod(typeof(ChangeWeaponDamage).GetMethod("Character_ApplyAttack_Prefix"))
      );

      Mod.Harmony.Patch(
        original: typeof(Explosion).GetMethod("DamageCharacters", AccessTools.all),
        prefix: new HarmonyMethod(typeof(ChangeWeaponDamage).GetMethod("Explosion_DamageCharacters_Prefix"))
      );
    }

    [Dependency] public static ConfigProxy Config { get; set; }

    public static bool Character_ApplyAttack_Prefix(Character __instance, ref AttackResult __result, Character attacker, Vector2 worldPosition, Attack attack, float deltaTime, Vector2 impulseDirection, bool playSound = false, Limb targetLimb = null, float penetration = 0f)
    {
      if (Config == null) return true;

      bool fromWeapon =
        attack.SourceItem?.GetComponent<Projectile>()?.Launcher?.GetComponent<RangedWeapon>() != null ||
        attack.SourceItem?.GetComponent<RangedWeapon>() != null ||
        attack.SourceItem?.GetComponent<MeleeWeapon>() != null;

      if (fromWeapon)
      {
        attack.DamageMultiplier = Config.WeaponDamage;
      }

      return true;
    }

    public static bool Explosion_DamageCharacters_Prefix(Explosion __instance, Vector2 worldPosition, Attack attack, float force, Entity damageSource, Character attacker)
    {
      if (Config == null) return true;
      attack.DamageMultiplier = Config.WeaponDamage;

      return true;
    }



  }

}