using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Graphics;
using Il2CppSG.Airlock.Roles;
using Il2CppSG.Airlock.Sabotage;
using MelonLoader;
using ShadowsPublicMenu.Config;
using System.Collections;
using UnityEngine;

namespace ShadowsPublicMenu.Managers
{
    public class RPCManager
    {
        public static void RPC_ForceRole(PlayerState target, GameRole role)
        {
            if (Settings.IsHost && target != null && target.IsConnected && target.IsSpawned)
            {
                PlayerRef targetRef = Helpers.GetPlayerRefFromID(target.PlayerId);
                GameReferences.Killing.AlterRole(role, targetRef);
            }
        }

        public static void RPC_KillPlayer(PlayerState target)
        {
            if (Settings.IsHost)
            {
                PlayerRef targetRef = Helpers.GetPlayerRefFromID(target.PlayerId);
                GameReferences.Killing.RPC_TargetedAction(targetRef, targetRef, (int)ProximityTargetedAction.Kill);
            }
        }

        public static void RPC_KillAllPlayers()
        {
            if (Settings.IsHost)
            {
                foreach (PlayerState player in GameReferences.Spawn.PlayerStates)
                {
                    if (player != null && !player.IsSpectating && player.IsConnected && player.IsSpawned)
                    {
                        PlayerRef targetRef = Helpers.GetPlayerRefFromID(player.PlayerId);
                        GameReferences.Killing.RPC_TargetedAction(targetRef, targetRef, (int)ProximityTargetedAction.Kill);
                    }
                }
            }
        }
        public static void RPC_StartGame()
        {
            if (Settings.IsHost)
            {
                GameReferences.GameState.StartGame();
            }
        }

        public static void RPC_BeginSabotage(Sabotage.SabotageType sabotage)
        {
            if (GameReferences.sabotage != null && Settings.IsHost && !Settings.SabotageActive)
            {
                GameReferences.sabotage.RPC_SendSabotageToAll((int)sabotage, -1);
                Settings.SabotageActive = true;
                MelonCoroutines.Start(WaitSabotage());
            }
        }

        public static void RPC_EndSabotage()
        {
            if (GameReferences.sabotage != null)
            {
                GameReferences.sabotage.RPC_EndSabotage(true);
                Settings.SabotageActive = false;
            }
        }

        private static IEnumerator WaitSabotage()
        {
            Settings.SabotageActive = true;
            yield return new WaitForSeconds(4f);
            Settings.SabotageActive = false;
        }
    }
}
