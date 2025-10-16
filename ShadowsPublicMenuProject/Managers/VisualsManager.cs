using System.Collections.Generic;
using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using ShadowsPublicMenu.Config;
using UnityEngine;

namespace ShadowsPublicMenu.Managers
{
    public static class PlayerVisualManager
    {
        private static GameObject espHolder;
        private static GameObject lineRenderHolder;

        public static Dictionary<PlayerState, GameObject[]> playerESPs = new Dictionary<PlayerState, GameObject[]>();
        public static Dictionary<PlayerState, LineRenderer> playerLines = new Dictionary<PlayerState, LineRenderer>();
        private static Dictionary<PlayerState, NetworkedLocomotionPlayer> playerLocomotions = new Dictionary<PlayerState, NetworkedLocomotionPlayer>();

        public static void DrawVisuals()
        {
            if (GameReferences.Rig == null || GameReferences.Spawn?.PlayerStates == null)
            {
                CleanupAll();
                return;
            }

            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 localPos = GameReferences.Rig.transform.position;
            bool localSpectator = GameReferences.Rig.PState.IsSpectating;

            var currentPlayers = new HashSet<PlayerState>();
            foreach (var ps in GameReferences.Spawn.PlayerStates)
                if (ps != null) currentPlayers.Add(ps);

            CleanupMissingPlayers(currentPlayers);

            foreach (var state in currentPlayers)
            {
                if (state == GameReferences.Rig.PState || !state.IsSpawned || !state.IsConnected || state.IsSpectating)
                {
                    RemoveESP(state);
                    RemoveTracer(state);
                    continue;
                }

                if (state.NetworkName?.Value != null && state.NetworkName.Value.Contains("Color##"))
                {
                    RemoveESP(state);
                    RemoveTracer(state);
                    continue;
                }

                if (!playerLocomotions.TryGetValue(state, out var loco) || loco == null)
                {
                    GameObject playerObj = GameObject.Find($"NetworkedLocomotionPlayer ({state.PlayerId})");
                    if (playerObj == null)
                    {
                        RemoveESP(state);
                        RemoveTracer(state);
                        continue;
                    }

                    loco = playerObj.GetComponent<NetworkedLocomotionPlayer>();
                    if (loco == null)
                    {
                        RemoveESP(state);
                        RemoveTracer(state);
                        continue;
                    }

                    playerLocomotions[state] = loco;
                }

                Vector3 targetPos = loco.RigidbodyPosition + new Vector3(0f, 0.6f, 0f);
                Color playerColor = Helpers.GetColorFromID(state.ColorId);

                if (Mods.BoxESP)
                {
                    if (!playerESPs.TryGetValue(state, out var esp) || esp == null)
                    {
                        esp = CreateHollowESPBox();
                        playerESPs[state] = esp;
                    }
                    UpdateHollowBox(esp, targetPos, cam, playerColor);
                }
                else RemoveESP(state);

                if (Mods.Tracers)
                {
                    if (!playerLines.TryGetValue(state, out var line) || line == null)
                    {
                        line = CreateLineRenderer();
                        playerLines[state] = line;
                    }
                    UpdateTracer(line, localPos, targetPos, playerColor);
                }
                else RemoveTracer(state);
            }
        }

        private static void CleanupMissingPlayers(HashSet<PlayerState> currentPlayers)
        {
            foreach (var kvp in new Dictionary<PlayerState, GameObject[]>(playerESPs))
                if (!currentPlayers.Contains(kvp.Key) || kvp.Key == null) RemoveESP(kvp.Key);

            foreach (var kvp in new Dictionary<PlayerState, LineRenderer>(playerLines))
                if (!currentPlayers.Contains(kvp.Key) || kvp.Key == null) RemoveTracer(kvp.Key);

            foreach (var kvp in new Dictionary<PlayerState, NetworkedLocomotionPlayer>(playerLocomotions))
                if (!currentPlayers.Contains(kvp.Key) || kvp.Key == null) playerLocomotions.Remove(kvp.Key);
        }

        private static void CleanupAll()
        {
            foreach (var kvp in playerESPs) DestroyESP(kvp.Value);
            playerESPs.Clear();

            foreach (var kvp in playerLines)
                if (kvp.Value != null) Object.Destroy(kvp.Value.gameObject);
            playerLines.Clear();

            playerLocomotions.Clear();
        }

        private static void RemoveESP(PlayerState state)
        {
            if (state == null) return;
            if (playerESPs.TryGetValue(state, out var oldESP))
            {
                DestroyESP(oldESP);
                playerESPs.Remove(state);
            }
        }

        private static void DestroyESP(GameObject[] esp)
        {
            if (esp == null) return;
            foreach (var o in esp)
                if (o != null) Object.Destroy(o);
        }

        private static GameObject[] CreateHollowESPBox()
        {
            if (espHolder == null) espHolder = new GameObject("ESP_Holder");

            GameObject[] lines = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.Destroy(line.GetComponent<Collider>());
                line.transform.parent = espHolder.transform;

                var rend = line.GetComponent<Renderer>();
                rend.material = new Material(Shader.Find("GUI/Text Shader"));
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;

                lines[i] = line;
            }
            return lines;
        }

        private static void UpdateHollowBox(GameObject[] lines, Vector3 pos, Camera cam, Color color)
        {
            if (lines == null || lines.Length < 4) return;

            float width = 0.9f, height = 1.8f, thickness = 0.05f;
            Vector3 camForward = cam.transform.forward;
            camForward.y = 0;
            if (camForward.sqrMagnitude < 0.001f) camForward = Vector3.forward;
            Quaternion rotation = Quaternion.LookRotation(camForward.normalized);

            lines[0].transform.position = pos + rotation * Vector3.up * (height / 2f);
            lines[0].transform.localScale = new Vector3(width, thickness, thickness);
            lines[0].transform.rotation = rotation;

            lines[1].transform.position = pos - rotation * Vector3.up * (height / 2f);
            lines[1].transform.localScale = new Vector3(width, thickness, thickness);
            lines[1].transform.rotation = rotation;

            lines[2].transform.position = pos + rotation * Vector3.right * (width / 2f);
            lines[2].transform.localScale = new Vector3(thickness, height, thickness);
            lines[2].transform.rotation = rotation;

            lines[3].transform.position = pos - rotation * Vector3.right * (width / 2f);
            lines[3].transform.localScale = new Vector3(thickness, height, thickness);
            lines[3].transform.rotation = rotation;

            foreach (var line in lines)
                if (line != null)
                    line.GetComponent<Renderer>().material.color = color;
        }

        private static void RemoveTracer(PlayerState state)
        {
            if (state == null) return;
            if (playerLines.TryGetValue(state, out var line))
            {
                if (line != null) Object.Destroy(line.gameObject);
                playerLines.Remove(state);
            }
        }

        private static LineRenderer CreateLineRenderer()
        {
            if (lineRenderHolder == null)
                lineRenderHolder = new GameObject("LineRender_Holder");

            GameObject lineObj = new GameObject("LineObject");
            lineObj.transform.parent = lineRenderHolder.transform;
            LineRenderer renderer = lineObj.AddComponent<LineRenderer>();

            renderer.numCapVertices = 10;
            renderer.numCornerVertices = 5;
            renderer.material.shader = Shader.Find("GUI/Text Shader");
            renderer.positionCount = 2;
            renderer.useWorldSpace = true;
            renderer.startWidth = 0.025f;
            renderer.endWidth = 0.025f;

            return renderer;
        }

        private static void UpdateTracer(LineRenderer line, Vector3 start, Vector3 end, Color color)
        {
            if (line == null) return;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.startColor = color;
            line.endColor = color;
            if (!line.gameObject.activeInHierarchy) line.gameObject.SetActive(true);
        }
    }
}