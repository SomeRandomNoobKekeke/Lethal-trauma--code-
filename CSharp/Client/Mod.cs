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
using LTCrabUI;

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
      SettingsUI.Revealed = true;
    }

    public void DisposeProjSpecific()
    {

    }


  }
}