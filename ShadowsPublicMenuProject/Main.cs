using System.Collections;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Il2CppInternal.Cryptography;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.UI.TitleScreen;
using Il2CppSystem.Runtime.InteropServices;
using MelonLoader;
using ShadowsPublicMenu.Config;
using ShadowsPublicMenu.Managers;
using ShadowsPublicMenu.MenuPages;
using UnityEngine;
using UnityEngine.InputSystem;
using Il2CppSteamworks;
using static ShadowsPublicMenu.Config.GameReferences;
using static ShadowsPublicMenu.Config.Settings;

[assembly: MelonInfo(typeof(ShadowsPublicMenu.Main), "Shadows Public Menu", Settings.Version, "Shadoww.py", "https://github.com/NotJolyne1/ShadowsAmongUsVRHacks/releases/tag/Release")]

[assembly: MelonGame("Schell Games", "Among Us 3D")]
[assembly: MelonGame("Schell Games", "Among Us VR")]

namespace ShadowsPublicMenu
{
    public class Main : MelonMod
    {
        private Texture2D _roundedRectTexture;

        private float nextFpsUpdateTime = 0f;
        private int frames = 0;
        private int fps = 0;
        public static bool passed = true;
        private bool cached = false;
        public static CSteamID cachedId;

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("Thank you for using Shadows Menu!");
        }

        [System.Obsolete]
        public override void OnApplicationStart()
        {
            MelonLogger.Msg($"Initializing Menu...");

            IsVR = Application.productName.Contains("VR");
            VersionCheck();


            MelonLogger.Msg($@"
{"\u001b[35m"}+----------------------------------------------------------------------+
|                                                                      |
|                         SHADOW'S MENU                                |
|                    Developed by Shadoww.py                           |
|                                                                      |
| Thank you for using Shadows Menu! Click Left Ctrl key to toggle it!  |
|                                                                      |
| Join my Discord for early access to updates and to make suggestions! |
| https://discord.com/invite/2FzsKdvjMU                                |
|                                                                      |
+----------------------------------------------------------------------+{"\u001b[0m"}
");
       
        }





        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            InGame = sceneName != "Boot" && sceneName != "Title";

            if (!InGame)
            {
                Settings.CurrentRoom = "Not in a room";
                Settings.CodeRecievced = false;
            }
            else
            {
                GameObject.Find("BlindboxHeadTrigger")?.SetActive(false);
                GameObject.Find("SightboxHeadTrigger")?.SetActive(false);
                MelonCoroutines.Start(WaitSendTelemetry());
            }
        }


        public override void OnUpdate()
        {
            if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
                Settings.GUIEnabled = !Settings.GUIEnabled;
            UpdateFps();

            if (SteamAPI.IsSteamRunning() && SteamUser.BLoggedOn() && !cached)
            {
                // Caches ID for telemetry / this replaces the use of moderation IDs for telemetry bc that could be seen as suspicious to some people
                cachedId = SteamUser.GetSteamID();
                cached = true;
            }

            if (Settings.GUIColorInt == 8)
            {
                Settings.rainbowColor += Time.deltaTime * 0.25f;
                if (Settings.rainbowColor > 1f)
                    Settings.rainbowColor = 0f;

                Settings.GUIColor = Color.HSVToRGB(Settings.rainbowColor, 1f, 1f);
            }

            if (!InGame)
            {
                GameRefsFound = false;
                return;
            }

            ModManager.Update();

            if (InGame && !GameRefsFound)
                GameReferences.refreshGameRefs();

        }




        public override void OnGUI()
        {

            GUI.color = Settings.GUIColor;


            if (!GUIEnabled || !passed)
                return;



            try
            {
                DrawMainMenu();
                DrawTopCenterStatusBar();

                PlayerState player = null;
                if (InGame && Spawn?.PlayerStates != null && PlayerNum >= 0 && PlayerNum < Spawn.PlayerStates.Count)
                    player = Spawn.PlayerStates[PlayerNum];

                DrawPlayerInfo(player);
                DrawPlayerNavigationButtons();

                if (PlayerPageNum <= 0) PlayerPageNum = 1;
                if (PlayerPageNum > 1) PlayerPageNum = 1;

                switch (PlayerPageNum)
                {
                    case 1:
                        PlayerPage1.Display(player);
                        break;
                }
            }


            catch (System.Exception e)
            {
                Settings.ErrorCount += 1;
                if (Settings.ErrorCount > 25)
                {
                    MelonLogger.Warning($"[FAIL] Something went wrong! Please report this to me, @Shadoww.py on discord or github issues tab with this: Failed at ModManager.Update(), error: {e}");
                }
            }

            if (!CodeRecievced && InGame)
            {
                CodeRecievced = true;
                Settings.CurrentRoom = Helpers.GetCurrentRoomCode();
            }
        }

        private void UpdateFps()
        {
            frames++;
            float time = Time.unscaledTime;
            if (time >= nextFpsUpdateTime)
            {
                float interval = time - (nextFpsUpdateTime - 1f);
                fps = Mathf.RoundToInt(frames / interval);
                frames = 0;
                nextFpsUpdateTime = time + 1f;
            }
        }

        // Sends telemetry for debugging / analytics
        private static async Task SubmitTelemetry()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Telemetry-ID", $"{cachedId}");
                client.DefaultRequestHeaders.Add("Telemetry-Errors", $"{Settings.ErrorCount}");

                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://shadowsmenu.jolyne108.workers.dev/");

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (System.Exception ex)
                {
                    MelonLogger.Warning($"[FAIL] Something went wrong! Please report this to me, @Shadoww.py on discord or github issues tab with this: Failed at Main.SubmitTelemetry(), error: {ex}");
                    Settings.ErrorCount += 1;
                }
            }
        }


        public static IEnumerator WaitSendTelemetry()
        {
            yield return new WaitForSeconds(4f);
            _ = SubmitTelemetry();
        }


        private void DrawPlayerNavigationButtons()
        {
            if (GUI.Button(new Rect(180f, 20f, 80f, 30f), "◄----") && PlayerNum > 0)
                PlayerNum--;

            if (GUI.Button(new Rect(260f, 20f, 80f, 30f), "----►") && PlayerNum < 9)
                PlayerNum++;
        }

        private void DrawPlayerInfo(PlayerState player)
        {
            string name = "Nobody";

            if (player != null && player.IsSpawned && player.IsConnected)
            {
                name = player.NetworkName?.Value ?? "Nobody";

                if (name.Contains("Color##"))
                    name = "Joining..";
            }

            GUI.Box(new Rect(180f, 0f, 160f, 20f), $"{name} ({PlayerNum})");

        }


        private void DrawMainMenu()
        {
            GUI.Box(new Rect(20f, 0f, 160f, 20f), "Shadows Menu" + $" [{CurrentPage}]");

            if (GUI.Button(new Rect(20f, 20f, 80f, 30f), "◄----"))
                CurrentPage--;

            if (GUI.Button(new Rect(100f, 20f, 80f, 30f), "----►"))
                CurrentPage++;

            if (CurrentPage < 1) CurrentPage = 1;
            else if (CurrentPage >= 3) CurrentPage = 1;

            switch (CurrentPage)
            {
                case 1: MenuPages.MenuPage1.Display(); break;
                case 2: MenuPages.MenuPage2.Display(); break;

            }
        }

        private void DrawTopCenterStatusBar()
        {
            if (!Settings.showFpsBar)
                return;

            string fpsText = $"FPS: {fps}";
            string MenuName = "Shadows Menu";
            string fullText = $"{fpsText} | {MenuName} | {Settings.CurrentRoom}";

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                normal = { textColor = Color.cyan },
                clipping = TextClipping.Clip,
                padding = new RectOffset(12, 12, 6, 6)
            };

            Vector2 textSize = style.CalcSize(new GUIContent(fullText));
            float barHeight = textSize.y + 8;
            float barWidth = textSize.x + 20;

            float x = (Screen.width - barWidth) / 2;
            float y = 5f;

            if (_roundedRectTexture == null)
                _roundedRectTexture = CreateRoundedTexture(512, 128, 24, new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.25f));

            _roundedRectTexture.filterMode = FilterMode.Bilinear;

            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), _roundedRectTexture);

            GUI.color = Color.cyan;
            GUI.Label(new Rect(x, y, barWidth, barHeight), fullText, style);
            GUI.color = Settings.GUIColor;
        }


        private Texture2D CreateRoundedTexture(int width, int height, float radius, Color color)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false) { filterMode = FilterMode.Bilinear };
            Color transparent = new Color(0, 0, 0, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Min(x, width - 1 - x);
                    float dy = Mathf.Min(y, height - 1 - y);
                    float alpha = 1f;

                    if (dx < radius && dy < radius)
                    {
                        float cornerDist = Vector2.Distance(new Vector2(dx, dy), new Vector2(radius, radius));
                        alpha = Mathf.Clamp01(1f - ((cornerDist - (radius - 1f)) / 2f));
                    }

                    Color finalColor = color;
                    finalColor.a *= alpha;
                    tex.SetPixel(x, y, finalColor.a > 0.01f ? finalColor : transparent);
                }
            }

            tex.Apply();
            return tex;
        }


        private static async void VersionCheck()
        {
            string VersionUsing = Settings.Version;
            string NewestVersion = string.Empty;
            bool outdated = false;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    NewestVersion = await client.GetStringAsync("https://shadowsmenu.jolyne108.workers.dev/version.txt");
                    NewestVersion = NewestVersion.Trim();
                }

                if (System.Version.TryParse(VersionUsing, out System.Version usingVersion) &&
                    System.Version.TryParse(NewestVersion, out System.Version newestVersion))
                {
                    int comparison = newestVersion.CompareTo(usingVersion);

                    if (comparison > 0)
                    {
                        MelonLogger.Msg($"[OUTDATED] A newer version of Shadows Menu is available! Please update to version {NewestVersion}");
                        outdated = true;
                    }
                    else if (comparison < 0)
                    {
                        MelonLogger.Msg($"[BETA] You are using a beta version of Shadows Menu, beta version {VersionUsing}");
                    }
                    else if (comparison == 0 && Settings.betaBuild)
                    {
                        MelonLogger.Msg($"[OUTDATED] You are using a beta build, but the full release of this build has been released. \nPlease update.");
                        outdated = true;
                    }
                    else
                    {
                        MelonLogger.Msg($"[UP TO DATE] Shadows Menu is up to date on Version {VersionUsing}");
                    }

                    if (outdated)
                    {
                        Application.OpenURL("https://discord.com/invite/2FzsKdvjMU");
                    }
                }
                else
                {
                    MelonLogger.Warning($"[FAIL] Failed to compare versions. Current: {VersionUsing}, Newest: {NewestVersion}");
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[FAIL] Version check failed!: {ex.Message}");
            }
        }



        public static Color SetMenuTheme()
        {
            switch (Settings.GUIColorInt)
            {
                case 0: return Settings.GUIColor = Color.cyan;
                case 1: return Settings.GUIColor = Color.blue;
                case 2: return Settings.GUIColor = Color.magenta;
                case 3: return Settings.GUIColor = Color.red;
                case 4: return Settings.GUIColor = Color.yellow;
                case 5: return Settings.GUIColor = Color.green;
                case 6: return Settings.GUIColor = Color.white;
                case 7: return Settings.GUIColor = Color.gray;
                case 8: return Settings.GUIColor = Color.HSVToRGB(Settings.rainbowColor, 1f, 1f);
            }
            return Settings.GUIColor = Color.white;
        }

    }
}
