using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;



namespace Lethaltrauma
{
  public class PatchClassAttribute : System.Attribute { }
  public partial class Mod : IAssemblyPlugin
  {
    public static string Name = "Lethaltrauma";
    public static Harmony Harmony = new Harmony("lethaltrauma");

    public static Mod Instance { get; set; }

    public Debugger Debugger { get; set; } = new Debugger();
    public Logger Logger { get; set; } = new Logger();



    public void Initialize()
    {
      Instance = this;
      AddCommands();

      Debugger.Debug = true;
      PatchAll();

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
      Instance = null;
    }
  }
}