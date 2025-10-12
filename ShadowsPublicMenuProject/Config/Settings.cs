using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShadowsPublicMenu.Config
{
    internal class Settings
    {
        public static bool GUIEnabled = true;

        public static bool IsVR = false;
        public static bool IsHost = false;
        public static bool InGame = false;
        public static bool GameRefsFound = false;
        public static bool showRolePage = false;
        public static bool showFpsBar = true;

        public static int PlayerNum = 0;
        public static int PlayerPageNum = 0;

        public static int CurrentPage = 0;

        public static bool CodeRecievced = false;

        public static string CurrentRoom = "Not in a room";

    }

}