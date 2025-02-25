#define BEBUG

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


namespace Lethaltrauma
{
  public enum DebugLevel
  {
    All = 0,
    PatchExecuted = 1,
  }

  public enum DebugAdditionalPrint
  {
    None = 0,
  }

  public class Debugger
  {
    public Logger Logger => Mod.Instance?.Logger;
    public bool Debug { get; set; }
    public DebugLevel CurrentLevel { get; set; }
    public DebugAdditionalPrint AlsoPrint { get; set; }


#if !BEBUG
    [Conditional("DONT")]
#endif
    public void Log(object msg, DebugLevel level = DebugLevel.All)
    {
      if (!Debug) return;
      if ((level & CurrentLevel) == 0) return;
      Logger.Log($"{level}| {msg}");
      if (AlsoPrint != DebugAdditionalPrint.None) PrintAditionalInfo();
    }

    public void PrintAditionalInfo()
    {
      // switch (AlsoPrint)
      // {
      //   case DebugAdditionalPrint.LevelInfo: Utils.PrintLevelInfo(); break;
      // }
    }
  }

}