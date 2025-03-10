using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using CrabUI;
using LTDependencyInjection;

namespace Lethaltrauma
{
  public class SettingsUI : CUIFrame
  {
    public static string SavePath => Path.Combine(Mod.Instance.Paths.DataUI, "SettingsUI.xml");
    [Dependency] public Debouncer Debouncer { get; set; }
    [Dependency] public ConfigProxy Config { get; set; }
    [Dependency] public ConfigManager ConfigManager { get; set; }
    [Dependency] public Logger Logger { get; set; }

    public Vector2 MinimizedSize;
    public Vector2 RestoredSize;


    public void SyncWithConfig()
    {
      foreach (PropertyInfo pi in Config.Props)
      {
        DispatchDown(new CUIData(pi.Name, pi.GetValue(Config)));
      }
    }

    public override void Hydrate()
    {
      OnAnyCommand += (command) =>
      {
        Debouncer.Debounce("guh", 50, () => Config.SetProp(command.Name, command.Data));
      };


      Config.PropChanged += SyncWithConfig;
      ConfigManager.ConfigChanged += SyncWithConfig;
      SyncWithConfig();

      CUITextBlock header = this.Get<CUITextBlock>("layout.header");
      MinimizedSize = new Vector2(300, header.UnwrappedMinSize.Y - 1);
      RestoredSize = MinimizedSize with { Y = 300 };

      header.OnDClick += (e) =>
      {
        if (Real.Size != MinimizedSize) Absolute = Absolute with { Size = MinimizedSize };
        else Absolute = Absolute with { Size = RestoredSize };
      };

      this.Get<CUIButton>("layout.content.load buttons.vanilla").OnMouseDown += (e) =>
      {
        ConfigManager.Load(ConfigManager.VanillaConfigName);
      };

      this.Get<CUIButton>("layout.content.load buttons.default").OnMouseDown += (e) =>
      {
        ConfigManager.Load(ConfigManager.DefaultConfigName);
      };
    }

    public void AfterInject()
    {
      Mod.Instance.OnDispose += () => this.SaveToFile(SavePath);
    }



    public void CreateUI()
    {
      Revealed = false;
      Anchor = CUIAnchor.TopRight;

      this["layout"] = new CUIVerticalList() { Relative = new CUINullRect(0, 0, 1, 1), };
      this["layout"]["header"] = new CUITextBlock("Lethaltrauma Settings")
      {
        TextScale = 1.0f,
        TextAlign = CUIAnchor.Center,
        Style = CUIStylePrefab.FrameCaption,
      };

      Absolute = new CUINullRect(0, 0, 300, this.Get<CUITextBlock>("layout.header").UnwrappedMinSize.Y - 1);

      this["content"] = this["layout"]["content"] = new CUIVerticalList()
      {
        FillEmptySpace = new CUIBool2(false, true),
        Scrollable = true,
        Style = CUIStylePrefab.Main,
        ConsumeDragAndDrop = true,
        ScrollSpeed = 0.5f,
        Gap = 10.0f,
      };

      this["layout"]["content"]["load buttons"] = new CUIHorizontalList()
      {
        FitContent = new CUIBool2(false, true),
      };
      this["layout"]["content"]["load buttons"]["vanilla"] = new CUIButton("Vanilla")
      {
        FillEmptySpace = new CUIBool2(true, false),
      };
      this["layout"]["content"]["load buttons"]["default"] = new CUIButton("Default")
      {
        FillEmptySpace = new CUIBool2(true, false),
      };




      CUIVerticalList Damage = new CUIVerticalList() { FitContent = new CUIBool2(false, true) };
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("Ranged Weapon Damage", "RangedWeaponDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("Melee Weapon Damage", "MeleeWeaponDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("Explosion Damage", "ExplosionDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("Turret Damage", "TurretDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("Monster Attack Damage", "MonsterAttackDamage", new FloatRange(0, 5)));
      this["layout"]["content"]["damage"] = CUIPrefab.WrapInGroup("Damage", Damage);

      CUIVerticalList Reaction = new CUIVerticalList() { FitContent = new CUIBool2(false, true) };
      Reaction.Append(CUIPrefab.TickboxWithLabel("Override Bot Aim", "OverrideAim"));
      Reaction.Append(CUIPrefab.TextAndSliderWithLabel("Bot Aim Accuracy", "AimAccuracy", new FloatRange(0, 1)));
      Reaction.Append(CUIPrefab.TextAndSliderWithLabel("Bot Aim Speed", "AimSpeed", new FloatRange(0, 1)));
      this["layout"]["content"]["reaction"] = CUIPrefab.WrapInGroup("Bot Aim", Reaction);


      CUIVerticalList Health = new CUIVerticalList() { FitContent = new CUIBool2(false, true) };
      Health.Append(CUIPrefab.TickboxWithLabel("Override Health Multiplier", "OverrideHealthMult"));
      Health.Append(CUIPrefab.TextAndSliderWithLabel("Human Health Multiplier", "HumanHealth", new FloatRange(0, 5)));
      Health.Append(CUIPrefab.TextAndSliderWithLabel("Monster Health Multiplier", "MonsterHealth", new FloatRange(0, 5)));
      this["layout"]["content"]["health"] = CUIPrefab.WrapInGroup("Health", Health);

      this["layout"]["content"]["PressureKillDelay"] = CUIPrefab.WrapInGroup("Pressure Kill Delay", CUIPrefab.TextAndSlider("PressureKillDelay", new FloatRange(0, 5)));

      this["layout"]["content"]["NoReputationLossInMask"] = CUIPrefab.TickboxWithLabel("No Reputation Loss In Mask", "NoReputationLossInMask");

      this["layout"]["content"].DeepPalette = PaletteOrder.Secondary;
      this["layout"]["content"]["load buttons"].DeepPalette = PaletteOrder.Tertiary;

      //HACK
      if (Mod.Instance.Paths.IsInLocalMods)
      {
        this.SaveToFile(SavePath);
      }

      this.LoadSelfFromFile(SavePath);
    }


  }
}