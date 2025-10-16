using System;
using System.Collections;
using System.Security.Cryptography.Xml;
using Il2CppFusion;
using Il2CppInterop.Runtime;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Minigames;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Roles;
using Il2CppSG.Airlock.UI.TitleScreen;
using MelonLoader;
using ShadowsPublicMenu.Config;
using UnityEngine;

namespace ShadowsPublicMenu.Managers
{
    public class ModManager
    {


        
        public static void Update()
        {
            if (!Settings.InGame)
                return;

            try
            {
                RenderSettings.fog = !Mods.Fullbright;
                GameReferences.Lights.VFX?.gameObject?.SetActive(!Mods.Fullbright);

                if (GameReferences.Rig != null && GameReferences.Rig.PState != null)
                    Settings.IsHost = GameReferences.Rig.PState.HasStateAuthority;

                if (GameReferences.Rig != null)
                {
                    if (GameReferences.Rig._collider != null)
                        GameReferences.Rig._collider.enabled = !Mods.Noclip;

                    GameReferences.Rig._speed = Mods.Speed ? 20f : 6.5f;
                }

                if (GameReferences.Killing != null && Mods.NoKillCooldown)
                    GameReferences.Killing.SetMaxCooldown(0);

                if (Mods.Tracers || Mods.BoxESP)
                    PlayerVisualManager.DrawVisuals();



            }
            catch (Exception e)
            {
                Settings.ErrorCount += 1;
                if (Settings.ErrorCount > 25)
                {
                    MelonLogger.Warning($"[FAIL] Something went wrong! Please report this to me, @Shadoww.py on discord or github issues tab with this: Failed at ModManager.Update(), error: {e}");
                }
            }
        }


        public static IEnumerator SpazColors(PlayerState target)
        {
            GameReferences.Customization._inWardrobe = true;

            while (Mods.spazColors || Mods.spazAllColors)
            {
                target.UpdateColorID(0);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(1);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(2);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(3);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(4);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(5);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(6);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(7);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(8);
                yield return new WaitForSeconds(0.1f);
                target.UpdateColorID(9);
                yield return new WaitForSeconds(0.1f);
            }
            GameReferences.Customization._inWardrobe = false;
        }
    }
}
