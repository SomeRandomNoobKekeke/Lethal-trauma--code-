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
  [Singleton]
  public class ConfigManager : IConfigContainer
  {
    public static string GetConfigPath(string name) => Path.Combine(Mod.Instance.Paths.Data, name + ".xml");
    public static string ConfigName = "Config";
    public static string VanillaConfigName = "Vanilla";
    public static string DefaultConfigName = "Default";

    public static string SavePath => GetConfigPath(ConfigName);

    [Dependency] public ConfigProxy Proxy { get; set; }
    [Dependency] public Parser Parser { get; set; }
    [Dependency] public Logger Logger { get; set; }
    [Dependency] public Debugger Debugger { get; set; }
    [Dependency] public NetManager NetManager { get; set; }

    public event Action ConfigChanged;

    private Config config = new Config();
    public Config Config
    {
      get => config;
      set
      {
        config = value;
        ConfigChanged?.Invoke();
      }
    }


    public void AfterInject()
    {
#if CLIENT
      Proxy.PropChanged  += () => 
      {
        if (GameMain.IsMultiplayer && Utils.IHavePermissions) NetManager.Sync();
      };
#endif
    }

    public void Save()
    {

      XDocument xdoc = new XDocument();
      xdoc.Add(new XElement("Config"));
      XElement root = xdoc.Root;

      foreach (PropertyInfo pi in typeof(Config).GetProperties())
      {
        root.Add(new XElement(pi.Name, pi.GetValue(Config)));
      }

      xdoc.Save(SavePath);
      Debugger.Log($"Save {SavePath}", DebugLevel.ConfigLoading);
    }

    public void Load(string name = null)
    {
#if CLIENT
      if(GameMain.IsMultiplayer && !Utils.IHavePermissions)return;
#endif

      name ??= ConfigName;
      string path = GetConfigPath(name);
      if (!File.Exists(path)) return;

      Debugger.Log($"loading {name}", DebugLevel.ConfigLoading);

      Config newConfig = new Config();

      XDocument xdoc = XDocument.Load(path);
      XElement root = xdoc.Root;

      foreach (XElement element in root.Elements())
      {
        PropertyInfo pi = typeof(Config).GetProperty(element.Name.ToString());
        if (pi == null) continue;

        if (Parser.UltimateParse(element.Value, pi.PropertyType, out object value))
        {
          pi.SetValue(newConfig, value);
        }
        else
        {
          Logger.Warning($"Coldn't parse {element} into {pi}");
        }
      }

      Config = newConfig;

      //HACK
#if CLIENT
      if (GameMain.IsMultiplayer && Utils.IHavePermissions) NetManager.Sync();
#endif
    }

    public void Encode(IWriteMessage msg)
    {
      foreach (PropertyInfo pi in typeof(Config).GetProperties())
      {
        NetManager.WriteObject(pi.GetValue(Config), msg);
      }
    }

    public void Decode(IReadMessage msg)
    {
      Config newConfig = new Config();
      foreach (PropertyInfo pi in typeof(Config).GetProperties())
      {
        pi.SetValue(newConfig, NetManager.ReadObject(pi.PropertyType, msg));
      }

      Config = newConfig;
    }
  }
}