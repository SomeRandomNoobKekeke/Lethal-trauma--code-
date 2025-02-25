using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;

using Barotrauma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;

namespace Lethaltrauma
{
  public partial class Mod
  {
    public List<DebugConsole.Command> AddedCommands = new List<DebugConsole.Command>();
    public void AddCommands()
    {
      // AddedCommands.Add(new DebugConsole.Command("spm_saveload", "", (string[] args) =>
      // {
      //   Logic.SaveLoader.SaveLoad();
      // }));

      DebugConsole.Commands.InsertRange(0, AddedCommands);
    }

    public void RemoveCommands()
    {
      AddedCommands.ForEach(c => DebugConsole.Commands.Remove(c));
      AddedCommands.Clear();
    }
  }
}