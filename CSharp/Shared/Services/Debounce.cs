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
  // Idk, mb it should be just an extension to LuaCsTimer
  public class Debouncer
  {
    public static LuaCsTimer Timer => GameMain.LuaCs.Timer;
    [Dependency] public Logger Logger { get; set; }

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
        Timer.timedActions.Remove(Scheduled[name]);
        Scheduled[name] = timedAction;
        Timer.AddTimer(timedAction);
      }
      else
      {
        Timer.AddTimer(timedAction);
        Scheduled[name] = timedAction;
      }

    }
  }
}