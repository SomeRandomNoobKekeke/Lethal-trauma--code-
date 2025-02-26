using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTDependencyInjection;

namespace Lethaltrauma
{
  [Singleton]
  public class Parser
  {
    public bool UltimateParse(string raw, Type T, out object result)
    {
      result = null;
      raw = raw.Trim();

      if (T == typeof(string)) { result = raw; return true; }
      if (T == typeof(bool)) { bool ok = bool.TryParse(raw, out bool b); result = b; return ok; }
      if (T == typeof(int)) { bool ok = int.TryParse(raw, out int i); result = i; return ok; }
      if (T == typeof(float))
      {
        if (raw.StartsWith('.')) return false;
        if (raw.EndsWith('.')) return false;
        bool ok = float.TryParse(raw, out float f); result = f; return ok;
      }
      if (T == typeof(double))
      {
        if (raw.StartsWith('.')) return false;
        if (raw.EndsWith('.')) return false;
        bool ok = double.TryParse(raw, out double d); result = d; return ok;
      }

      return false;
    }
  }

}