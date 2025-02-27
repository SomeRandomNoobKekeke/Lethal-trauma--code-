using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using LTCrabUI;
using LTDependencyInjection;

namespace Lethaltrauma
{
  public class SaveLoaderMenuButton
  {
    static void CreateVerificationPrompt(string text, Action confirmAction)
    {
      var msgBox = new GUIMessageBox("", text,
          new LocalizedString[] { TextManager.Get("Yes"), TextManager.Get("No") })
      {
        UserData = "verificationprompt",
        DrawOnTop = true
      };
      msgBox.Buttons[0].OnClicked = (_, __) =>
      {
        GUI.PauseMenuOpen = false;
        confirmAction?.Invoke();
        return true;
      };
      msgBox.Buttons[0].OnClicked += msgBox.Close;
      msgBox.Buttons[1].OnClicked += msgBox.Close;
    }

    public static void AddSaveLoadButton()
    {
      if (GameMain.GameSession?.GameMode is not SinglePlayerCampaign) return;
      if (GUI.PauseMenuOpen)
      {
        GUIFrame frame = GUI.PauseMenu;
        GUIComponent pauseMenuInner = frame.GetChild(1);

        GUIComponent list = frame.GetChild(1).GetChild(0);

        GUIButton button = new GUIButton(new RectTransform(new Vector2(1f, 0.1f), list.RectTransform), "Save Load")
        {
          Color = new Color(255, 255, 255),
        };



        button.OnClicked = (sender, args) =>
        {
          CreateVerificationPrompt("Srsly?", () =>
          {
            SaveUtil.SaveGame(GameMain.GameSession.DataPath);
            Submarine.Unload();
            SaveUtil.LoadGame(GameMain.GameSession.DataPath);
          });
          return true;
        };

        button.RectTransform.RepositionChildInHierarchy(3);

        pauseMenuInner.RectTransform.RelativeSize = new Vector2(0.2f, 0.5f);
      }
    }
  }

}