using System.Collections.Generic;
using Il2CppFusion;
using Il2CppSG.Airlock;
using Il2CppSG.Airlock.Network;
using ShadowsPublicMenu.Config;
using UnityEngine;
using static Il2CppSG.Airlock.Graphics.VulpineRenderer;

namespace ShadowsPublicMenu.Managers
{
    public static class PlayerVisualManager
    {
        private static GameObject espHolder;
        public static Dictionary<PlayerState, GameObject[]> playerESPs = new Dictionary<PlayerState, GameObject[]>();

        private static GameObject lineRenderHolder;
        public static Dictionary<PlayerState, LineRenderer> playerLines = new Dictionary<PlayerState, LineRenderer>();

        private static Dictionary<PlayerState, NetworkedLocomotionPlayer> playerLocomotions = new Dictionary<PlayerState, NetworkedLocomotionPlayer>();


        private static readonly Dictionary<GameObject, List<Renderer>> playerRenderers = new Dictionary<GameObject, List<Renderer>>();
        private static readonly Dictionary<int, Material> playerMaterials = new Dictionary<int, Material>();
        public static void DrawVisuals()
        {
            if (GameReferences.Rig == null || GameReferences.Spawn?.PlayerStates == null)
                return;

            bool localAlive = GameReferences.Rig.PState.IsAlive;
            bool localSpectator = GameReferences.Rig.PState.IsSpectating;
            Vector3 localPos = GameReferences.Rig.transform.position;
            Camera cam = Camera.main;
            if (cam == null) return;

            foreach (var state in GameReferences.Spawn.PlayerStates)
            {
                bool playerValid = state != null && state.IsSpawned && state.IsConnected && !state.IsSpectating &&
                                   (state.NetworkName?.Value == null || !state.NetworkName.Value.Contains("Color##")) &&
                                   state != GameReferences.Rig.PState;

                bool targetAlive = state?.IsAlive ?? false;

                if (!playerValid || localSpectator)
                {
                    if (Mods.BoxESP) RemoveESP(state);
                    if (Mods.Tracers) RemoveTracer(state);
                    continue;
                }

                if (!playerLocomotions.TryGetValue(state, out NetworkedLocomotionPlayer locoPlayer) || locoPlayer == null)
                {
                    GameObject playerObj = GameObject.Find($"NetworkedLocomotionPlayer ({state.PlayerId})");
                    if (playerObj == null)
                    {
                        if (Mods.BoxESP) RemoveESP(state);
                        if (Mods.Tracers) RemoveTracer(state);
                        continue;
                    }

                    locoPlayer = playerObj.GetComponent<NetworkedLocomotionPlayer>();
                    if (locoPlayer == null)
                    {
                        if (Mods.BoxESP) RemoveESP(state);
                        if (Mods.Tracers) RemoveTracer(state);
                        continue;
                    }

                    playerLocomotions[state] = locoPlayer;
                }

                if (Mods.BoxESP)
                {
                    if (!playerESPs.TryGetValue(state, out GameObject[] espParts) || espParts == null)
                    {
                        espParts = CreateHollowESPBox();
                        playerESPs[state] = espParts;
                    }

                    Vector3 espPos = playerLocomotions[state].RigidbodyPosition + new Vector3(0f, 0.6f, 0f);
                    Color espColor = Helpers.GetColorFromID(state.ColorId);
                    UpdateHollowBox(espParts, espPos, cam, espColor);
                }

                if (Mods.Tracers)
                {
                    if (!playerLines.TryGetValue(state, out LineRenderer line) || line == null)
                    {
                        line = CreateLineRenderer();
                        playerLines[state] = line;
                    }

                    float lineWidth = 0.025f;
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;
                    Color lineColor = Helpers.GetColorFromID(state.ColorId);
                    line.startColor = lineColor;
                    line.endColor = lineColor;

                    line.SetPosition(0, localPos);
                    line.SetPosition(1, playerLocomotions[state].RigidbodyPosition);

                    if (!line.gameObject.activeInHierarchy)
                        line.gameObject.SetActive(true);
                }
            }

            if (Mods.BoxESP) CleanupESP();
            if (Mods.Tracers) CleanupTracers();
        }

        private static void RemoveESP(PlayerState state)
        {
            if (state == null) return;
            if (playerESPs.TryGetValue(state, out GameObject[] oldESP))
            {
                if (oldESP != null)
                    foreach (var o in oldESP)
                        if (o != null) Object.Destroy(o);
                playerESPs.Remove(state);
            }
        }

        private static GameObject[] CreateHollowESPBox()
        {
            if (espHolder == null)
                espHolder = new GameObject("ESP_Holder");

            GameObject[] lines = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.Destroy(line.GetComponent<Collider>());
                line.transform.parent = espHolder.transform;

                var renderer = line.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("GUI/Text Shader"));
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;

                lines[i] = line;
            }

            return lines;
        }

        private static void UpdateHollowBox(GameObject[] lines, Vector3 pos, Camera cam, Color color)
        {
            if (lines == null || lines.Length < 4) return;

            float width = 0.9f;
            float height = 1.8f;
            float thickness = 0.05f;

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
            {
                if (line != null)
                {
                    var rend = line.GetComponent<Renderer>();
                    rend.enabled = true;
                    rend.material.color = color;
                }
            }
        }

        private static void CleanupESP()
        {
            List<PlayerState> toRemove = new List<PlayerState>();
            foreach (var kvp in playerESPs)
            {
                if (kvp.Key == null || !kvp.Key.IsConnected || !kvp.Key.IsSpawned || kvp.Key.IsSpectating ||
                    (kvp.Key.NetworkName?.Value != null && kvp.Key.NetworkName.Value.Contains("Color##")))
                    toRemove.Add(kvp.Key);
            }
            foreach (var key in toRemove)
                RemoveESP(key);
        }

        private static void RemoveTracer(PlayerState state)
        {
            if (state == null) return;
            if (playerLines.TryGetValue(state, out LineRenderer oldLine))
            {
                if (oldLine != null)
                    Object.Destroy(oldLine.gameObject);
                playerLines.Remove(state);
            }
        }

        private static LineRenderer CreateLineRenderer()
        {
            if (lineRenderHolder == null)
                lineRenderHolder = new GameObject("LineRender_Holder");

            GameObject lineObj = new GameObject("LineObject");
            lineObj.transform.parent = lineRenderHolder.transform;
            LineRenderer newLine = lineObj.AddComponent<LineRenderer>();

            newLine.numCapVertices = 10;
            newLine.numCornerVertices = 5;
            newLine.material.shader = Shader.Find("GUI/Text Shader");
            newLine.positionCount = 2;
            newLine.useWorldSpace = true;
            newLine.startWidth = 0.025f;
            newLine.endWidth = 0.025f;

            return newLine;
        }

        private static void CleanupTracers()
        {
            List<PlayerState> toRemove = new List<PlayerState>();
            foreach (var kvp in playerLines)
            {
                if (kvp.Key == null || !kvp.Key.IsConnected || !kvp.Key.IsSpawned || kvp.Key.IsSpectating ||
                    (kvp.Key.NetworkName?.Value != null && kvp.Key.NetworkName.Value.Contains("Color##")))
                    toRemove.Add(kvp.Key);
            }
            foreach (var key in toRemove)
                RemoveTracer(key);
        }


    }
}
