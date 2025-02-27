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

    public void Give(object[] args)
    {
      Debugger.Log($"Lethaltrauma Server Give", DebugLevel.NetEvents);
      IReadMessage netMessage = args[0] as IReadMessage;
      Client client = args[1] as Client;

      IWriteMessage outMsg = GameMain.LuaCs.Networking.Start("lt_sync");
      ConfigManager.Encode(outMsg);

      GameMain.LuaCs.Networking.Send(outMsg, client.Connection);
    }

    public void Receive(object[] args)
    {
      Debugger.Log($"Lethaltrauma Server Receive", DebugLevel.NetEvents);
      IReadMessage inMsg = args[0] as IReadMessage;
      Client client = args[1] as Client;

      if (client.Connection != GameMain.Server.OwnerConnection &&
          !client.HasPermission(ClientPermissions.All)) return;

      ConfigManager.Decode(inMsg);

      Broadcast();
    }

    public void Broadcast()
    {
      Debugger.Log($"Lethaltrauma Server Broadcast", DebugLevel.NetEvents);
      IWriteMessage outMsg = GameMain.LuaCs.Networking.Start("lt_sync");
      ConfigManager.Encode(outMsg);
      GameMain.LuaCs.Networking.Send(outMsg);
    }
  }
}