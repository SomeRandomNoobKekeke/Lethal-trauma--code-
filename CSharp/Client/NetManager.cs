using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using LTDependencyInjection;

namespace Lethaltrauma
{
  public partial class NetManager
  {
    [Dependency] public Debugger Debugger { get; set; }
    [Dependency] public ConfigManager ConfigManager { get; set; }

    public void Sync()
    {
      if (GameMain.IsSingleplayer) return;
      Debugger.Log($"Lethaltrauma Client Sync", DebugLevel.NetEvents);

      IWriteMessage outMsg = GameMain.LuaCs.Networking.Start("lt_sync");
      ConfigManager.Encode(outMsg);
      GameMain.LuaCs.Networking.Send(outMsg);
    }

    public void Ask()
    {
      Debugger.Log($"Lethaltrauma Client Ask", DebugLevel.NetEvents);
      IWriteMessage message = GameMain.LuaCs.Networking.Start("lt_ask");
      GameMain.LuaCs.Networking.Send(message);
    }

    public void Receive(object[] args)
    {
      Debugger.Log($"Lethaltrauma Client Receive", DebugLevel.NetEvents);
      IReadMessage inMsg = args[0] as IReadMessage;
      Client client = args[1] as Client;
      ConfigManager.Decode(inMsg);
    }
  }
}