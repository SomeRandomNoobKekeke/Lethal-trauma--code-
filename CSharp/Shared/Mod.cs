using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTDependencyInjection;
#if CLIENT
using CrabUI;
#endif

using System.Runtime.CompilerServices;
[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace Lethaltrauma
{
  public class PatchClassAttribute : System.Attribute { }
  public partial class Mod : IAssemblyPlugin
  {
    public static string Name = "Lethaltrauma";
    public static Harmony Harmony = new Harmony("lethaltrauma");

    [EntryPoint] public static Mod Instance { get; set; }
    [Singleton] public Debugger Debugger { get; set; }
    [Singleton] public Logger Logger { get; set; }
    [Singleton] public NetManager NetManager { get; set; }
    [Singleton] public ConfigManager ConfigManager { get; set; }

    public event Action OnInitialize;
    public event Action OnDispose;

    public ModPaths Paths { get; set; }

    public ServiceCollection Services = new ServiceCollection() { Debug = false };

    public void SetupServices()
    {
      Services.Map<IConfigContainer, ConfigManager>();
    }

    public void Initialize()
    {
      Instance = this;
      //AddCommands();

      Paths = new ModPaths(Name);

#if CLIENT
      CUI.ModDir = Paths.ModDir;
      CUI.AssetsPath = Paths.AssetsFolder;
      CUIPalette.DefaultPalette = "Red";
      CUI.HookIdentifier = Name;
      CUI.Initialize();

      SetupCUI();
#endif

      SetupServices();
      Services.InjectEverything();

      Debugger.Debug = Paths.IsInLocalMods;
      //Debugger.CurrentLevel = DebugLevel.NetEvents | DebugLevel.ConfigLoading;
      PatchAll();


      InitializeProjSpecific();

      OnInitialize?.Invoke();

      Logger.Info($"{Name} initialized");
    }

    public void PatchAll()
    {
      Assembly CallingAssembly = Assembly.GetCallingAssembly();

      foreach (Type type in CallingAssembly.GetTypes())
      {
        if (Attribute.IsDefined(type, typeof(PatchClassAttribute)))
        {
          MethodInfo init = type.GetMethod("Initialize", AccessTools.all);
          if (init != null)
          {
            init.Invoke(null, new object[] { });
          }
        }
      }
    }

    public static void Log(object msg, Color? color = null)
    {
      color ??= Color.Cyan;
      LuaCsLogger.LogMessage($"{msg ?? "null"}", color * 0.8f, color);
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose()
    {
      OnDispose?.Invoke();

      //RemoveCommands();
      DisposeProjSpecific();

#if CLIENT
      CUI.Dispose();
#endif
      Instance = null;
    }
  }
}