using System.Runtime.InteropServices;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Roles;
using MelonLoader;
using ShadowsPublicMenu.Config;
using ShadowsPublicMenu.Managers;
using UnityEngine;

namespace ShadowsPublicMenu.MenuPages
{
    public class PlayerPage1
    {
        public static void Display([Optional] PlayerState target)
        {
            float y = 50f;
            bool canWork = Settings.InGame && target != null;

            if (GUI.Button(new Rect(180f, y, 160f, 30f), "Kill Player (H)") && canWork)
                RPCManager.RPC_KillPlayer(target);
            y += 30f;

            if (GUI.Button(new Rect(180f, y, 160f, 30f), "Force Imposter (H)") && canWork)
                RPCManager.RPC_ForceRole(target, GameRole.Impostor);
            y += 30f;

            if (GUI.Button(new Rect(180f, y, 160f, 30f), "Force Vigilante (H)") && canWork)
                RPCManager.RPC_ForceRole(target, GameRole.Vigilante);
            y += 30f;

            if (GUI.Button(new Rect(180f, y, 160f, 30f), "Force Crewmate (H)") && canWork)
                RPCManager.RPC_ForceRole(target, GameRole.Crewmember);
            y += 30f;

            if (GUI.Button(new Rect(180f, y, 160f, 30f), $"Spaz Colors (H): {Mods.spazColors}") && canWork)
            {
                Mods.spazColors = !Mods.spazColors;
                MelonCoroutines.Start(ModManager.SpazColors(target));
            }
            y += 30f;

            if (GUI.Button(new Rect(180f, y, 160f, 30f), "Teleport To Player") && canWork)
            {
                if (!GameReferences.AllNetorkLocomotions.TryGetValue(target, out NetworkedLocomotionPlayer locoPlayer) || locoPlayer == null)
                {
                    GameReferences.RefreshLocomotions();

                    if (!GameReferences.AllNetorkLocomotions.TryGetValue(target, out locoPlayer) || locoPlayer == null)
                        return;
                }

                GameReferences.Rig.Transform.rotation = locoPlayer.RigidbodyRotation;
                GameReferences.Rig.Transform.position = locoPlayer.RigidbodyPosition;
            }
            y += 30f;


        }
    }
}
