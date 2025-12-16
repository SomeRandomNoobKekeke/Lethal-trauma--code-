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


using Barotrauma.Abilities;
using Barotrauma.Extensions;
using Barotrauma.IO;
using Barotrauma.Items.Components;
using Barotrauma.Networking;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
#if SERVER
using System.Text;
#endif

namespace Lethaltrauma
{
  [PatchClass]
  public class FriendlyFire
  {
    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Character).GetMethod("DamageLimb", AccessTools.all),
        prefix: new HarmonyMethod(typeof(FriendlyFire).GetMethod("Character_DamageLimb_Replace"))
      );
    }

    [Dependency] public static ConfigProxy Config { get; set; }
    [Dependency] public static Logger Logger { get; set; }

    public static bool Character_DamageLimb_Replace(Character __instance, ref AttackResult __result, Vector2 worldPosition, Limb hitLimb, IEnumerable<Affliction> afflictions, float stun, bool playSound, Vector2 attackImpulse, Character attacker = null, float damageMultiplier = 1, bool allowStacking = true, float penetration = 0f, bool shouldImplode = false, bool ignoreDamageOverlay = false, bool recalculateVitality = true)
    {
      Character _ = __instance;



      if (_.Removed) { __result = new AttackResult(); return false; }

      _.SetStun(stun);

      if (
        attacker != null && attacker != _ &&
        (
          !Config.AllowFriendlyFire ||
          GameMain.NetworkMember != null && !GameMain.NetworkMember.ServerSettings.AllowFriendlyFire
        )
      )
      {
        if (attacker.TeamID == _.TeamID)
        {
          if (afflictions.None(a => a.Prefab.IsBuff)) { __result = new AttackResult(); return false; }
        }
      }

      Vector2 dir = hitLimb.WorldPosition - worldPosition;
      if (attackImpulse.LengthSquared() > 0.0f)
      {
        Vector2 diff = dir;
        if (diff == Vector2.Zero) { diff = Rand.Vector(1.0f); }
        Vector2 hitPos = hitLimb.SimPosition + ConvertUnits.ToSimUnits(diff);
        hitLimb.body.ApplyLinearImpulse(attackImpulse, hitPos, maxVelocity: NetConfig.MaxPhysicsBodyVelocity * 0.5f);
        var mainLimb = hitLimb.character.AnimController.MainLimb;
        if (hitLimb != mainLimb)
        {
          // Always add force to mainlimb
          mainLimb.body.ApplyLinearImpulse(attackImpulse, hitPos, maxVelocity: NetConfig.MaxPhysicsBodyVelocity);
        }
      }
      bool wasDead = _.IsDead;
      Vector2 simPos = hitLimb.SimPosition + ConvertUnits.ToSimUnits(dir);
      AttackResult attackResult = hitLimb.AddDamage(simPos, afflictions, playSound, damageMultiplier: damageMultiplier, penetration: penetration, attacker: attacker);
      _.CharacterHealth.ApplyDamage(hitLimb, attackResult, allowStacking, recalculateVitality);
      if (shouldImplode)
      {
        // Only used by assistant's True Potential talent. Has to run here in order to properly give kill credit when it activates.
        _.Implode();
      }

      if (attacker != _)
      {
        bool wasDamageOverlayVisible = _.CharacterHealth.ShowDamageOverlay;
        if (ignoreDamageOverlay)
        {
          // Temporarily ignore damage overlay (husk transition damage)
          _.CharacterHealth.ShowDamageOverlay = false;
        }
        _.OnAttacked?.Invoke(attacker, attackResult);
        _.OnAttackedProjSpecific(attacker, attackResult, stun);
        // Reset damage overlay
        _.CharacterHealth.ShowDamageOverlay = wasDamageOverlayVisible;
        if (!wasDead)
        {
          _.TryAdjustAttackerSkill(attacker, attackResult);
        }
      }
      if (attackResult.Damage > 0)
      {
        Mod.PrintStackTrace();
        _.LastDamage = attackResult;
        if (attacker != null && attacker != _ && !attacker.Removed)
        {
          _.AddAttacker(attacker, attackResult.Damage);
          if (_.IsOnPlayerTeam)
          {
            CreatureMetrics.AddEncounter(attacker.SpeciesName);
          }
          if (attacker.IsOnPlayerTeam)
          {
            CreatureMetrics.AddEncounter(_.SpeciesName);
          }
        }
        _.ApplyStatusEffects(ActionType.OnDamaged, 1.0f);
        hitLimb.ApplyStatusEffects(ActionType.OnDamaged, 1.0f);
      }
#if CLIENT
      if (_.Params.UseBossHealthBar && Character.Controlled != null && Character.Controlled.teamID == attacker?.teamID)
      {
          CharacterHUD.ShowBossHealthBar(_, attackResult.Damage);
      }
#endif
      __result = attackResult; return false;
    }



  }

}