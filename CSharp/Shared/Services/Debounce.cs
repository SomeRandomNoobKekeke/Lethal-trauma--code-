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
  // Idk, mb it should be just an extension to LuaCsTimer
  public class Debouncer
  {
    public static LuaCsTimer Timer => GameMain.LuaCs.Timer;

    private Dictionary<string, LuaCsTimer.TimedAction> Scheduled = new();

    public void Debounce(string name, int millisecondDelay, Action action)
    {
      LuaCsTimer.TimedAction timedAction = new LuaCsTimer.TimedAction((object[] args) =>
        {
          action();
          Scheduled.Remove(name);
        },
        millisecondDelay
      );

      if (Scheduled.ContainsKey(name))
      {
        Scheduled[name].ExecutionTime = timedAction.ExecutionTime;
      }
      else
      {
        Timer.AddTimer(timedAction);
      }

    }
  }
}