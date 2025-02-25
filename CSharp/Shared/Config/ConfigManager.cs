using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LTDependencyInjection;
using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;



namespace Lethaltrauma
{

  [Singleton]
  public class ConfigManager : IConfigContainer
  {
    public Config Config { get; set; } = new Config();

    public void Save()
    {

    }

    public void Load()
    {

    }
  }
}