using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Barotrauma;

namespace LTDependencyInjection
{
  /// <summary>
  /// Properties marked with [Dependency], that are also mapped, will be injected,  
  /// and their own properties will be injected
  /// </summary>
  public class DependencyAttribute : System.Attribute { }
  /// <summary>
  /// Properties marked with [EntryPoint] will be created if == null
  /// and then their own properties will be injected
  /// </summary>
  public class EntryPointAttribute : System.Attribute { }
  /// <summary>
  /// Properties marked with [Singleton] will be created, added to Singletons and mapped
  /// </summary>
  public class SingletonAttribute : System.Attribute { }
  /// <summary>
  /// Classes marked with [HasStaticDependencies] will be scanned in InjectStaticProperties(Assembly assembly, bool onlyIfHasStaticDependencies = true)
  /// </summary>
  public class HasStaticDependenciesAttribute : System.Attribute { }
  /// <summary>
  /// Classes marked with [DontInjectStatic] won't be injected by InjectStaticProperties(Assembly assembly, bool onlyIfHasStaticDependencies = true)
  /// </summary>
  public class DontInjectStaticAttribute : System.Attribute { }
}