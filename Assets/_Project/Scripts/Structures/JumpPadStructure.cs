using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Treemals.Characters;

namespace Treemals.Structures
{
    // Launches any ILaunchable unit that enters or stays in its trigger area.
    // Launch direction is fixed at placement time and stored as a NetworkVariable
    // so all clients read the same direction for their local trigger evaluations.
    //
    // Players:  launch fires on owner client  (client-authoritative movement)
    // Enemies:  launch fires on server        (server-authoritative movement)
    public class JumpPadStructure : StructureBehaviour
    {
        [SerializeField] private float launchSpeed    = 30f;
        [SerializeField] private float launchDuration = 0.9f;
        [SerializeField] private float perTargetCooldown = 1.5f;

        // Stored at placement — every unit launched in this direction regardless of approach angle
        public readonly NetworkVariable<Vector2> LaunchDir = new(
            Vector2.up,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        // Keyed by NetworkObjectId — shared by player and enemy targets
        private readonly Dictionary<ulong, float> lastLaunchTime = new();

        protected override void OnPlaced(Vector2 facingDir)
        {
            LaunchDir.Value = facingDir.normalized;
        }

        // Both Enter and Stay call the same evaluator.
        // Enter guarantees immediate response; Stay handles units that remain inside.
        private void OnTriggerEnter2D(Collider2D other) => EvaluateTrigger(other);
        private void OnTriggerStay2D(Collider2D other)  => EvaluateTrigger(other);

        private void EvaluateTrigger(Collider2D other)
        {
            // Players — launch on the owner client
            var player = other.GetComponent<PlayerController>();
            if (player != null && player.IsOwner)
            {
                TryLaunch(player, player.NetworkObjectId);
                return;
            }

            // Enemies — launch on the server only
            if (!IsServer) return;
            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
                TryLaunch(enemy, enemy.NetworkObjectId);
        }

        private void TryLaunch(ILaunchable target, ulong id)
        {
            if (target.IsBeingLaunched) return;

            if (lastLaunchTime.TryGetValue(id, out float last) && Time.time - last < perTargetCooldown) return;
            lastLaunchTime[id] = Time.time;

            target.ApplyLaunch(LaunchDir.Value, launchSpeed, launchDuration);
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 dir = LaunchDir.Value.sqrMagnitude > 0.01f ? LaunchDir.Value : Vector2.up;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, (Vector3)dir * 2.5f);
            Gizmos.DrawSphere(transform.position + (Vector3)dir * 2.5f, 0.12f);
        }
    }
}
