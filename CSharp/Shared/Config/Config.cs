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
  public interface IConfigContainer
  {
    public Config Config { get; set; }
  }

  public interface IConfig
  {
    public float WeaponDamage { get; set; }
    public bool OverrideHumanHealthMult { get; set; }
    public float HumanHealth { get; set; }
    public float MonsterHealth { get; set; }
  }

  public class Config : IConfig
  {
    public float WeaponDamage { get; set; } = 1.0f;
    public bool OverrideHumanHealthMult { get; set; } = true;
    public float HumanHealth { get; set; } = 1.0f;
    public float MonsterHealth { get; set; } = 1.0f;
  }


  public class ConfigProxy : IConfig
  {
    public IConfigContainer Container { get; set; }

    public event Action<float> WeaponDamageChanged;
    public event Action<bool> OverrideHumanHealthMultChanged;
    public event Action<float> HumanHealthChanged;
    public event Action<float> MonsterHealthChanged;


    public float WeaponDamage
    {
      get => Container.Config.WeaponDamage;
      set
      {
        Container.Config.WeaponDamage = value;
        WeaponDamageChanged?.Invoke(value);
      }
    }
    public bool OverrideHumanHealthMult
    {
      get => Container.Config.OverrideHumanHealthMult;
      set
      {
        Container.Config.OverrideHumanHealthMult = value;
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
  }
}