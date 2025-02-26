using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTCrabUI;
using LTDependencyInjection;

namespace Lethaltrauma
{
  public class SettingsUI : CUIFrame
  {
    public static string SavePath => Path.Combine(Mod.Instance.Paths.DataUI, "SettingsUI.xml");
    [Dependency] public Debouncer Debouncer { get; set; }
    [Dependency] public ConfigProxy Config { get; set; }
    [Dependency] public Logger Logger { get; set; }


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


      Config.PropChanged += () => SyncWithConfig();
      SyncWithConfig();

      //this["layout"]["content"]["damage"].Palette = PaletteOrder.Tertiary;
      // this["layout"]["content"]["damage"].Palette = PaletteOrder.Tertiary;
    }



    public void CreateUI()
    {
      Revealed = false;
      Absolute = new CUINullRect(0, 0, 300, 300);
      Anchor = CUIAnchor.CenterRight;

      this["layout"] = new CUIVerticalList() { Relative = new CUINullRect(0, 0, 1, 1), };
      this["layout"]["header"] = new CUITextBlock("Lethaltrauma Settings")
      {
        TextScale = 1.0f,
        TextAlign = CUIAnchor.Center,
        Style = CUIStylePrefab.FrameCaption,
      };
      this["content"] = this["layout"]["content"] = new CUIVerticalList()
      {
        FillEmptySpace = new CUIBool2(false, true),
        Scrollable = true,
        Style = CUIStylePrefab.Main,
        ConsumeDragAndDrop = true,
        ScrollSpeed = 0.5f,
        Gap = 10.0f,
      };


      CUIVerticalList Damage = new CUIVerticalList() { FitContent = new CUIBool2(false, true) };
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("RangedWeaponDamage", "RangedWeaponDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("MeleeWeaponDamage", "MeleeWeaponDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("ExplosionDamage", "ExplosionDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("TurretDamage", "TurretDamage", new FloatRange(0, 5)));
      Damage.Append(CUIPrefab.TextAndSliderWithLabel("MonsterAttackDamage", "MonsterAttackDamage", new FloatRange(0, 5)));
      this["layout"]["content"]["damage"] = CUIPrefab.WrapInGroup("Damage", Damage);


      CUIVerticalList Health = new CUIVerticalList() { FitContent = new CUIBool2(false, true) };
      Health.Append(CUIPrefab.TickboxWithLabel("OverrideHealthMult", "OverrideHealthMult"));
      Health.Append(CUIPrefab.TextAndSliderWithLabel("HumanHealthMult", "HumanHealth", new FloatRange(0, 5)));
      this["layout"]["content"]["health"] = CUIPrefab.WrapInGroup("Health", Health);


      this["layout"]["content"]["PressureKillDelay"] = CUIPrefab.WrapInGroup("PressureKillDelay", CUIPrefab.TextAndSlider("PressureKillDelay", new FloatRange(0, 5)));

      this["layout"]["content"]["NoReputationLossInMask"] = CUIPrefab.TickboxWithLabel("NoReputationLossInMask", "NoReputationLossInMask");

      this["layout"]["content"]["CustomGuards"] = CUIPrefab.TickboxWithLabel("CustomGuards", "UseCustomGuards");





      this["layout"]["content"].Palette = PaletteOrder.Secondary;



      this.SaveToFile(SavePath);
      this.LoadSelfFromFile(SavePath);
    }


  }
}