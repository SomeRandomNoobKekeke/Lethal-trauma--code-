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

namespace Lethaltrauma
{
  public partial class NetManager
  {
    public static void WriteObject(object o, IWriteMessage msg)
    {
      if (o is float f) { msg.WriteSingle(f); return; }
      if (o is int i) { msg.WriteInt32(i); return; }
      if (o is bool b) { msg.WriteBoolean(b); return; }
      if (o is string s) { msg.WriteString(s); return; }
    }

    public static object ReadObject(Type T, IReadMessage msg)
    {
      if (T == typeof(float)) return msg.ReadSingle();
      if (T == typeof(int)) return msg.ReadInt32();
      if (T == typeof(bool)) return msg.ReadBoolean();
      if (T == typeof(string)) return msg.ReadString();
      return null;
    }
  }
}