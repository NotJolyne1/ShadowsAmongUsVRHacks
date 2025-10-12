using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Graphics;
using Il2CppSG.Airlock.Roles;
using ShadowsPublicMenu.Config;
using static UnityEngine.GraphicsBuffer;

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



    }
}
