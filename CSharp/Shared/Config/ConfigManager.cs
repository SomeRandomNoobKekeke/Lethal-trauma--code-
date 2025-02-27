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


namespace Lethaltrauma
{
  [Singleton]
  public class ConfigManager : IConfigContainer
  {
    public static string SavePath => Path.Combine(Mod.Instance.Paths.Data, "Config.xml");

    [Dependency] public ConfigProxy Proxy { get; set; }
    [Dependency] public Parser Parser { get; set; }
    [Dependency] public Logger Logger { get; set; }

    private Config config = new Config();
    public Config Config
    {
      get => config;
      set
      {
        config = value;
        Proxy.InvokePropChanged(); //guh
      }
    }


    public void AfterInject()
    {
      Mod.Instance.OnDispose += Save;
      Mod.Instance.OnInitialize += Load;
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
    }

    public void Load()
    {
      Config newConfig = new Config();

      XDocument xdoc = XDocument.Load(SavePath);
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
    }
  }
}