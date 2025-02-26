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
      this["layout"]["content"] = new CUIVerticalList()
      {
        FillEmptySpace = new CUIBool2(false, true),
        Scrollable = true,
        Style = CUIStylePrefab.Main,
        ConsumeDragAndDrop = true,
      };

      this["layout"]["content"]["WeaponDamage"] = CUIPrefab.TextAndSlider("Weapon Damage", "WeaponDamage", new FloatRange(0, 5));
      this["layout"]["content"]["PressureKillDelay"] = CUIPrefab.TextAndSlider("PressureKillDelay", "PressureKillDelay", new FloatRange(0, 5));




      this["layout"]["content"].Palette = PaletteOrder.Secondary;

      this.SaveToFile(SavePath);
      this.LoadSelfFromFile(SavePath);
    }


  }
}