using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.UI.TitleScreen;
using Il2CppSG.Airlock.Util;
using MelonLoader;
using ShadowsPublicMenu.Config;
using ShadowsPublicMenu.Managers;
using UnityEngine;

namespace ShadowsPublicMenu.MenuPages
{
    public class MenuPage2
    {
        public static void Display()
        {
            float y = 50f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Toggle FPS Bar: {Settings.showFpsBar}"))
            {
                Settings.showFpsBar = !Settings.showFpsBar;
            }
            y += 30f;


        }
    }
}
