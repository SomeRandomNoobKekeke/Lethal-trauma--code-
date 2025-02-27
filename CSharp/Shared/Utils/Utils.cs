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
  public partial class Utils
  {
#if SERVER
    public static bool IAmTheServer => true;
#else
    public static bool IAmTheServer => false;
#endif

#if SERVER
    public static bool AmITheHost => true;
#else
    public static bool AmITheHost => GameMain.IsSingleplayer || GameMain.Client.IsServerOwner;
#endif

#if CLIENT
    public static bool IHavePermissions => AmITheHost || GameMain.Client.HasPermission(ClientPermissions.All);
#endif


  }
}