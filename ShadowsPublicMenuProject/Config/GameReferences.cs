using System;
using System.Collections.Generic;
using System.Text;
using Il2Cpp;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Customization;
using Il2CppSG.Airlock.Network;
using Il2CppSG.Airlock.Sabotage;
using Il2CppSG.Airlock.Util;
using Il2CppSG.Airlock.XR;
using MelonLoader;
using UnityEngine;

namespace ShadowsPublicMenu.Config
{
    public class GameReferences
    {
        public static Dictionary<PlayerState, NetworkedLocomotionPlayer> AllNetorkLocomotions = new Dictionary<PlayerState, NetworkedLocomotionPlayer>();
        public static SpawnManager Spawn;
        public static CapsuleCollider Collider;
        public static XRRig Rig;
        public static NetworkedLocomotionPlayer LocoPlayer;
        public static PlayerState Player;
        public static NetworkedKillBehaviour Killing;
        public static GameStateManager GameState;
        public static AirlockNetworkRunner Runner;
        public static PlayerVisual Visual;
        public static CustomizationManager Customization;
        public static LightsSabotage Lights;
        public static SabotageManager sabotage;


        public static void refreshGameRefs()
        {
            string reference = "";
            try
            {
                Spawn = null;
                Collider = null;
                Rig = null;
                LocoPlayer = null;
                Player = null;
                Killing = null;
                GameState = null;
                Runner = null;
                Visual = null;
                Customization = null;
                Lights = null;
                sabotage = null;


                reference = "Spawn Manager";
                Spawn = UnityEngine.Object.FindObjectOfType<SpawnManager>();

                reference = "XRRig";
                Rig = UnityEngine.Object.FindObjectOfType<XRRig>();

                reference = "Capsule Collider";
                Collider = Rig.GetComponent<CapsuleCollider>();

                reference = "NetworkedLocomotionPlayer";
                LocoPlayer = UnityEngine.Object.FindObjectOfType<NetworkedLocomotionPlayer>();

                reference = "PlayerState";
                Player = UnityEngine.Object.FindObjectOfType<PlayerState>();

                reference = "NetworkedKillBehavior";
                Killing = UnityEngine.Object.FindObjectOfType<NetworkedKillBehaviour>();

                reference = "GameState";
                GameState = UnityEngine.Object.FindObjectOfType<GameStateManager>();

                reference = "Airlock Runner";
                Runner = UnityEngine.Object.FindObjectOfType<AirlockNetworkRunner>();

                reference = "Customization Manager";
                Customization = UnityEngine.Object.FindObjectOfType<CustomizationManager>();

                reference = "Lights Sabotage";
                Lights = UnityEngine.Object.FindObjectOfType<LightsSabotage>();

                reference = "SabotageManager";
                sabotage = UnityEngine.Object.FindObjectOfType<SabotageManager>();

                Settings.GameRefsFound = true;
                MelonLogger.Msg("Found Game References!");
            }
            catch (Exception e)
            {
                MelonLogger.Warning($"[FAIL] Failed to refresh game references! Please report this to me on discord (@Shadoww.py) or github issues tab with this: Failed at: {reference} Error: {e}");
                Settings.ErrorCount += 1;
            }
        }

        public static void RefreshLocomotions()
        {
            AllNetorkLocomotions.Clear();

            foreach (PlayerState player in Spawn.PlayerStates)
            {
                NetworkedLocomotionPlayer locoPlayer = null;

                GameObject playerObj = GameObject.Find($"NetworkedLocomotionPlayer ({player.PlayerId})");
                if (playerObj == null) continue;

                locoPlayer = playerObj.GetComponent<NetworkedLocomotionPlayer>();
                if (locoPlayer == null) continue;

                AllNetorkLocomotions[player] = locoPlayer;
            }
        }


    }
}
