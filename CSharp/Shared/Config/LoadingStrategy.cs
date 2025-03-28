using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LTDependencyInjection;
using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Barotrauma.Networking;

namespace Lethaltrauma
{

  public class ConfigLoadingStrategy
  {
#if CLIENT
    public static void Singleplayer()
    {
      Mod.Instance.OnInitialize += () => Mod.Instance.ConfigManager.Load();
      Mod.Instance.OnDispose += () => Mod.Instance.ConfigManager.Save();
    }

    public static void MultiplayerClient()
    {
      GameMain.LuaCs.Networking.Receive("lt_sync", Mod.Instance.NetManager.Receive);
      Mod.Instance.NetManager.Ask();
    }
#endif

#if SERVER
    public static void MultiplayerServer()
    {
      //it saves config in NetManager.Receive
      //Mod.Instance.OnDispose += () => ConfigManager.Save();
      GameMain.LuaCs.Networking.Receive("lt_ask", Mod.Instance.NetManager.Give);
      GameMain.LuaCs.Networking.Receive("lt_sync", Mod.Instance.NetManager.Receive);
      Mod.Instance.ConfigManager.Load();
      Mod.Instance.NetManager.Broadcast();
    }
#endif

    public static void UseAppropriate()
    {
#if CLIENT
      if (GameMain.IsSingleplayer)Singleplayer();
      if (GameMain.IsMultiplayer)MultiplayerClient();
#endif
#if SERVER
      MultiplayerServer();
#endif
    }
  }
}