using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTDependencyInjection;
#if CLIENT
using LTCrabUI;
#endif

namespace Lethaltrauma
{
  public class PatchClassAttribute : System.Attribute { }
  public partial class Mod : IAssemblyPlugin
  {
    public static string Name = "Lethal Trauma";
    public static Harmony Harmony = new Harmony("lethaltrauma");

    [EntryPoint] public static Mod Instance { get; set; }
    [Singleton] public Debugger Debugger { get; set; }
    [Singleton] public Logger Logger { get; set; }

    public ModPaths Paths { get; set; }

    public ServiceCollection Services = new ServiceCollection() { Debug = false };

    public void SetupServices()
    {
      Services.Map<IConfigContainer, ConfigManager>();
    }

    public void Initialize()
    {
      Instance = this;
      AddCommands();

      Paths = new ModPaths(Name);

#if CLIENT
      CUI.ModDir = Paths.ModDir;
      CUI.AssetsPath = Paths.AssetsFolder;
      CUI.Initialize();

      SetupCUI();
#endif

      SetupServices();
      Services.InjectEverything();
      //Services.PrintState();

      Debugger.Debug = true;
      PatchAll();


      InitializeProjSpecific();

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

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose()
    {
      RemoveCommands();
      DisposeProjSpecific();

#if CLIENT
      CUI.Dispose();
#endif
      Instance = null;
    }
  }
}