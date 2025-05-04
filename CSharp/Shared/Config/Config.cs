using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LTDependencyInjection;
using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;



namespace Lethaltrauma
{
  public interface IConfigContainer
  {
    public Config Config { get; set; }
  }

  public interface IConfig
  {
    public float RangedWeaponDamage { get; set; }
    public float MeleeWeaponDamage { get; set; }
    public float ExplosionDamage { get; set; }
    public float TurretDamage { get; set; }
    public float MonsterAttackDamage { get; set; }
    public bool OverrideHealthMult { get; set; }
    public float HumanHealth { get; set; }
    public float MonsterHealth { get; set; }
    public bool OverrideAim { get; set; }
    public float AimSpeed { get; set; }
    public float AimAccuracy { get; set; }
    public bool NoReputationLossInMask { get; set; }
    public float PressureKillDelay { get; set; }
  }

  public class Config : IConfig
  {
    public float RangedWeaponDamage { get; set; } = 1.0f;
    public float MeleeWeaponDamage { get; set; } = 1.0f;
    public float ExplosionDamage { get; set; } = 1.0f;
    public float TurretDamage { get; set; } = 1.0f;
    public float MonsterAttackDamage { get; set; } = 1.0f;
    public bool OverrideHealthMult { get; set; } = true;
    public float HumanHealth { get; set; } = 1.0f;
    public float MonsterHealth { get; set; } = 1.0f;
    public bool OverrideAim { get; set; } = true;
    public float AimSpeed { get; set; } = 0.8f;
    public float AimAccuracy { get; set; } = 0.75f;
    public bool NoReputationLossInMask { get; set; } = true;
    public float PressureKillDelay { get; set; } = 1.0f;
    public bool AllowFriendlyFire { get; set; } = true;
  }

  [Singleton]
  public class ConfigProxy : IConfig
  {
    [Dependency] public IConfigContainer Container { get; set; }
    [Dependency] public Logger Logger { get; set; }
    [Dependency] public Parser Parser { get; set; }


    public event Action<bool> OverrideHealthMultChanged;
    public event Action<float> HumanHealthChanged;
    public event Action<float> MonsterHealthChanged;
    public event Action<bool> OverrideAimChanged;
    public event Action<float> AimSpeedChanged;
    public event Action<float> AimAccuracyChanged;
    public event Action<float> PressureKillDelayChanged;
    public event Action PropChanged;

    public void SetProp(string name, object value)
    {
      PropertyInfo pi = typeof(ConfigProxy).GetProperty(name);
      if (pi == null) return;
      try
      {
        if (Parser.UltimateParse(value.ToString(), pi.PropertyType, out object result))
        {
          pi.SetValue(this, result);
        }
      }
      catch (Exception e)
      {
        Logger.Warning(e);
      }
    }

    public IEnumerable<PropertyInfo> Props => typeof(ConfigProxy).GetProperties();
    public IEnumerable<string> PropNames => typeof(ConfigProxy).GetProperties().Select(pi => pi.Name);



    public float RangedWeaponDamage
    {
      get => Container.Config.RangedWeaponDamage;
      set
      {
        Container.Config.RangedWeaponDamage = value;
        PropChanged?.Invoke();
      }
    }
    public float MeleeWeaponDamage
    {
      get => Container.Config.MeleeWeaponDamage;
      set
      {
        Container.Config.MeleeWeaponDamage = value;
        PropChanged?.Invoke();
      }
    }
    public float ExplosionDamage
    {
      get => Container.Config.ExplosionDamage;
      set
      {
        Container.Config.ExplosionDamage = value;
        PropChanged?.Invoke();
      }
    }
    public float TurretDamage
    {
      get => Container.Config.TurretDamage;
      set
      {
        Container.Config.TurretDamage = value;
        PropChanged?.Invoke();
      }
    }
    public float MonsterAttackDamage
    {
      get => Container.Config.MonsterAttackDamage;
      set
      {
        Container.Config.MonsterAttackDamage = value;
        PropChanged?.Invoke();
      }
    }

    public bool OverrideHealthMult
    {
      get => Container.Config.OverrideHealthMult;
      set
      {
        Container.Config.OverrideHealthMult = value;
        OverrideHealthMultChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }
    public float HumanHealth
    {
      get => Container.Config.HumanHealth;
      set
      {
        Container.Config.HumanHealth = value;
        HumanHealthChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }

    public float MonsterHealth
    {
      get => Container.Config.MonsterHealth;
      set
      {
        Container.Config.MonsterHealth = value;
        MonsterHealthChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }

    public bool OverrideAim
    {
      get => Container.Config.OverrideAim;
      set
      {
        Container.Config.OverrideAim = value;
        OverrideAimChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }
    public float AimSpeed
    {
      get => Container.Config.AimSpeed;
      set
      {
        Container.Config.AimSpeed = value;
        AimSpeedChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }
    public float AimAccuracy
    {
      get => Container.Config.AimAccuracy;
      set
      {
        Container.Config.AimAccuracy = value;
        AimAccuracyChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }

    public bool NoReputationLossInMask
    {
      get => Container.Config.NoReputationLossInMask;
      set
      {
        Container.Config.NoReputationLossInMask = value;
        PropChanged?.Invoke();
      }
    }

    public float PressureKillDelay
    {
      get => Container.Config.PressureKillDelay;
      set
      {
        Container.Config.PressureKillDelay = value;
        PressureKillDelayChanged?.Invoke(value);
        PropChanged?.Invoke();
      }
    }

    public bool AllowFriendlyFire
    {
      get => Container.Config.AllowFriendlyFire;
      set
      {
        Container.Config.AllowFriendlyFire = value;
        PropChanged?.Invoke();
      }
    }
  }
}