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
    public float WeaponDamage { get; set; }
    public bool OverrideHealthMult { get; set; }
    public float HumanHealth { get; set; }
    public float MonsterHealth { get; set; }
    public bool NoReputationLossInMask { get; set; }
    public float PressureKillDelay { get; set; }
  }

  public class Config : IConfig
  {
    public float WeaponDamage { get; set; } = 3.0f;
    public bool OverrideHealthMult { get; set; } = true;
    public float HumanHealth { get; set; } = 1.0f;
    public float MonsterHealth { get; set; } = 1.0f;
    public bool NoReputationLossInMask { get; set; } = true;
    public float PressureKillDelay { get; set; } = 1.0f;
  }

  [Singleton]
  public class ConfigProxy : IConfig
  {
    [Dependency] public IConfigContainer Container { get; set; }

    public event Action<float> WeaponDamageChanged;
    public event Action<bool> OverrideHumanHealthMultChanged;
    public event Action<float> HumanHealthChanged;
    public event Action<float> MonsterHealthChanged;
    public event Action<bool> NoReputationLossInMaskChanged;
    public event Action<float> PressureKillDelayChanged;



    public float WeaponDamage
    {
      get => Container.Config.WeaponDamage;
      set
      {
        Container.Config.WeaponDamage = value;
        WeaponDamageChanged?.Invoke(value);
      }
    }
    public bool OverrideHealthMult
    {
      get => Container.Config.OverrideHealthMult;
      set
      {
        Container.Config.OverrideHealthMult = value;
        OverrideHumanHealthMultChanged?.Invoke(value);
      }
    }
    public float HumanHealth
    {
      get => Container.Config.HumanHealth;
      set
      {
        Container.Config.HumanHealth = value;
        HumanHealthChanged?.Invoke(value);
      }
    }

    public float MonsterHealth
    {
      get => Container.Config.MonsterHealth;
      set
      {
        Container.Config.MonsterHealth = value;
        MonsterHealthChanged?.Invoke(value);
      }
    }

    public bool NoReputationLossInMask
    {
      get => Container.Config.NoReputationLossInMask;
      set
      {
        Container.Config.NoReputationLossInMask = value;
        NoReputationLossInMaskChanged?.Invoke(value);
      }
    }

    public float PressureKillDelay
    {
      get => Container.Config.PressureKillDelay;
      set
      {
        Container.Config.PressureKillDelay = value;
        PressureKillDelayChanged?.Invoke(value);
      }
    }
  }
}