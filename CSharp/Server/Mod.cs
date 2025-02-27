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
using Barotrauma.Networking;

namespace Lethaltrauma
{
  public partial class Mod : IAssemblyPlugin
  {
    public void InitializeProjSpecific()
    {
      GameMain.LuaCs.Networking.Receive("lt_ask", NetManager.Give);
      GameMain.LuaCs.Networking.Receive("lt_sync", NetManager.Receive);
    }

    public void DisposeProjSpecific()
    {

    }
  }
}