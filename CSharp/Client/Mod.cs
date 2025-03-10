using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTDependencyInjection;
using CrabUI;

namespace Lethaltrauma
{
  public partial class Mod : IAssemblyPlugin
  {
    [Dependency] public SettingsUI SettingsUI { get; set; }

    public void SetupCUI()
    {
      //CUIPalette.LoadSet(Path.Combine(CUIPalette.PaletteSetsPath, "Red.xml"));
    }
    public void InitializeProjSpecific()
    {
      SettingsUI.CreateUI();
      CUI.TopMain.Append(SettingsUI);
      CUI.OnPauseMenuToggled += () => SettingsUI.Revealed = GUI.PauseMenuOpen;
      CUI.OnPauseMenuToggled += SaveLoadMenuButton.AddSaveLoadButton;
      //SettingsUI.Revealed = true;

      if (GameMain.IsSingleplayer)
      {
        Mod.Instance.OnDispose += () => ConfigManager.Save();
        Mod.Instance.OnInitialize += () => ConfigManager.Load();
      }

      if (GameMain.IsMultiplayer)
      {
        GameMain.LuaCs.Networking.Receive("lt_sync", NetManager.Receive);
        NetManager.Ask();
      }
    }

    public void DisposeProjSpecific()
    {

    }


  }
}