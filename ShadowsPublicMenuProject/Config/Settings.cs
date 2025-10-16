using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShadowsPublicMenu.Config
{
    internal class Settings
    {
        public const string Version = "1.6";
        public const bool betaBuild = false;

        public static bool GUIEnabled = true;
        public static Color GUIColor = Color.cyan;
        public static int GUIColorInt = 0;
        public static float rainbowColor = 0f;

        public static bool IsVR = false;
        public static bool IsHost = false;
        public static bool InGame = false;
        public static bool Post = false;
        public static bool GameRefsFound = false;
        public static bool showRolePage = false;
        public static bool showFpsBar = true;

        public static int PlayerNum = 0;
        public static int PlayerPageNum = 0;

        public static int CurrentPage = 0;
        public static int ErrorCount = 0;

        public static bool CodeRecievced = false;
        public static bool SabotageActive = false;

        public static string CurrentRoom = "Not in a room";
    }
}