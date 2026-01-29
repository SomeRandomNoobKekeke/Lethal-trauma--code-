using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTDependencyInjection;
using Barotrauma.Items.Components;

namespace Lethaltrauma
{
  public class SharedState
  {
    public static bool? fromRangedWeapon;

    public static bool FromLimbUpdateAttack;
  }
}