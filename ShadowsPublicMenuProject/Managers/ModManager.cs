using System;
using System.Security.Cryptography.Xml;
using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Roles;
using Il2CppSG.Airlock.UI.TitleScreen;
using System.Collections;
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

                if (GameReferences.Rig != null && GameReferences.Rig.PState != null)
                    Settings.IsHost = GameReferences.Rig.PState.PlayerId == 9;

                if (GameReferences.Rig != null)
                {
                    if (GameReferences.Rig._collider != null)
                        GameReferences.Rig._collider.enabled = !Mods.Noclip;

                    GameReferences.Rig._speed = Mods.Speed ? 30f : 6.5f;
                }

                if (GameReferences.Killing != null && Mods.NoKillCooldown)
                    GameReferences.Killing.SetMaxCooldown(0);

                if (Mods.Tracers || Mods.BoxESP)
                    PlayerVisualManager.DrawVisuals();


            }
            catch (Exception e)
            {
                MelonLogger.Warning($"[FAIL] Something went wrong! Please report this to me, @Shadoww.py on discord or github issues tab with this: Failed at ModManager.Update(), error: {e}");
                Settings.ErrorCount += 1;
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
