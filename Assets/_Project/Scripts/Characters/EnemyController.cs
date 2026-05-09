using Unity.Netcode;
using UnityEngine;

namespace Treemals.Characters
{
    // Attach to the Enemy prefab alongside NetworkObject and NetworkTransform.
    // The server moves this enemy; NetworkTransform replicates position to all clients.
    public class EnemyController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;

        private void Update()
        {
            // Only the server drives enemy movement.
            // NetworkTransform (server-authoritative) handles syncing to clients.
            if (!IsServer) return;

            Transform target = GetNearestPlayer();
            if (target == null) return;

            Vector3 dir = (target.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        }

        private Transform GetNearestPlayer()
        {
            Transform nearest = null;
            float minDist = float.MaxValue;

            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                if (client.PlayerObject == null) continue;

                float dist = Vector2.Distance(transform.position, client.PlayerObject.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = client.PlayerObject.transform;
                }
            }

            return nearest;
        }
    }
}
