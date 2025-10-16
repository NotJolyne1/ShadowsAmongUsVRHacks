using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Sabotage;
using Il2CppSG.Airlock.UI.TitleScreen;
using Il2CppSG.Airlock.Util;
using Il2CppSystem.Threading;
using MelonLoader;
using ShadowsPublicMenu.Config;
using ShadowsPublicMenu.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowsPublicMenu.MenuPages
{
    public class MenuPage1
    {
        public static void Display()
        {
            float y = 50f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), "Join Discord"))
            {
                Application.OpenURL("https://discord.com/invite/2FzsKdvjMU");
            }

            y += 30f;


            if (GUI.Button(new Rect(20f, y, 160f, 30f), "Join Random (3D)"))
            {
                Object.FindObjectOfType<QuickMatchMenu>(true).gameObject.SetActive(true);
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), "Host Game (3D)"))
            {
                Object.FindObjectOfType<HostGameMenu>(true).gameObject.SetActive(true);
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), "Start Game (H)"))
            {
                RPCManager.RPC_StartGame();
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"No Game End (H): {Mods.PreventGameEnd}"))
            {
                Mods.PreventGameEnd = !Mods.PreventGameEnd;
                GameReferences.GameState._preventMatchEnding.Value = Mods.PreventGameEnd;
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), "Kill Everyone (H)"))
            {
                RPCManager.RPC_KillAllPlayers();
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Spaz All Colors (H): {Mods.spazAllColors}"))
            {
                Mods.spazAllColors = !Mods.spazAllColors;

                if (Mods.spazAllColors)
                {
                    foreach (PlayerState player in GameReferences.Spawn.PlayerStates)
                    {
                        if (player != null && !player.IsSpectating)
                            MelonCoroutines.Start(ModManager.SpazColors(player));
                    }
                }
            }

            y += 30f;
            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Sabotage Lights (H)"))
            {
                RPCManager.RPC_BeginSabotage(Sabotage.SabotageType.Lights);
            }

            y += 30f;
            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Sabotage Oxygen (H)"))
            {
                RPCManager.RPC_BeginSabotage(Sabotage.SabotageType.Oxygen);
            }

            y += 30f;
            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Sabotage Reactor (H)"))
            {
                RPCManager.RPC_BeginSabotage(Sabotage.SabotageType.Reactor);
            }
            y += 30f;


            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"End Sabotage"))
            {
                RPCManager.RPC_EndSabotage();
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Noclip: {Mods.Noclip}"))
            {
                Mods.Noclip = !Mods.Noclip;
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Speed: {Mods.Speed}"))
            {
                Mods.Speed = !Mods.Speed;
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"No Kill Cooldown: {Mods.NoKillCooldown}"))
            {
                Mods.NoKillCooldown = !Mods.NoKillCooldown;
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Hollow Box ESP: {Mods.BoxESP}"))
            {
                Mods.BoxESP = !Mods.BoxESP;
                foreach (var kvp in PlayerVisualManager.playerESPs)
                {
                    if (kvp.Value != null)
                    {
                        foreach (var line in kvp.Value)
                            if (line != null)
                                line.SetActive(Mods.BoxESP);
                    }
                }
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Tracers: {Mods.Tracers}"))
            {
                Mods.Tracers = !Mods.Tracers;
                foreach (var kvp in PlayerVisualManager.playerLines)
                {
                    if (kvp.Value != null)
                        kvp.Value.gameObject.SetActive(Mods.Tracers);
                }
            }
            y += 30f;

            if (GUI.Button(new Rect(20f, y, 160f, 30f), $"Fullbright: {Mods.Fullbright}"))
            {
                Mods.Fullbright = !Mods.Fullbright;
            }


        }
    }
}
