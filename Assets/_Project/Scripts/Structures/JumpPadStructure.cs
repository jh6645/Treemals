using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Treemals.Characters;

namespace Treemals.Structures
{
    // Jump Pad: launches any player who steps on it in their facing direction.
    // Trigger fires on the OWNER client — client-authoritative launch, synced via ClientNetworkTransform.
    // No ServerRpc needed: the owner moves themselves, NetworkTransform does the rest.
    public class JumpPadStructure : StructureBehaviour
    {
        [SerializeField] private float launchSpeed = 30f;
        [SerializeField] private float launchDuration = 0.9f;
        [SerializeField] private float perPlayerCooldown = 1f;

        // Keyed by OwnerClientId — prevents rapid re-launches while standing on the pad
        private readonly Dictionary<ulong, float> lastLaunchTime = new();

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            Debug.Log($"[JumpPad] OnTriggerEnter2D hit: {other.name} | isPlayer={player != null} | isOwner={player?.IsOwner}");
            if (player == null || !player.IsOwner) return;

            ulong id = player.OwnerClientId;
            if (lastLaunchTime.TryGetValue(id, out float last) && Time.time - last < perPlayerCooldown) return;
            lastLaunchTime[id] = Time.time;

            player.ApplyLaunch(player.FacingDir, launchSpeed, launchDuration);
        }
    }
}
