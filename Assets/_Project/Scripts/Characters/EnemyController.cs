using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Treemals.Characters
{
    // Attach to the Enemy prefab alongside NetworkObject and NetworkTransform.
    // The server drives all movement and combat; NetworkTransform syncs position to clients.
    public class EnemyController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed   = 2f;
        [SerializeField] private int   maxHp       = 30;
        [SerializeField] private int   contactDamage  = 10;
        [SerializeField] private float contactRadius  = 0.8f;
        [SerializeField] private float damageInterval = 1f;

        // All active enemies — PlayerCombat uses this to find attack targets without Physics2D.
        public static readonly List<EnemyController> All = new();

        public NetworkVariable<int> Hp = new();

        private float _damageTimer;

        public override void OnNetworkSpawn()
        {
            All.Add(this);
            if (IsServer) Hp.Value = maxHp;
        }

        public override void OnNetworkDespawn()
        {
            All.Remove(this);
        }

        private void Update()
        {
            if (!IsServer) return;

            MoveTowardNearestPlayer();
            TryDamagePlayers();
        }

        private void MoveTowardNearestPlayer()
        {
            Transform target = GetNearestPlayer();
            if (target == null) return;

            Vector3 dir = (target.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        }

        private void TryDamagePlayers()
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer < damageInterval) return;
            _damageTimer = 0f;

            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                if (client.PlayerObject == null) continue;

                float dist = Vector2.Distance(transform.position, client.PlayerObject.transform.position);
                if (dist > contactRadius) continue;

                client.PlayerObject.GetComponent<PlayerCombat>()?.TakeDamage(contactDamage);
            }
        }

        // Called by PlayerCombat on the server when a player attacks this enemy.
        public void TakeDamage(int amount)
        {
            if (!IsServer) return;

            Hp.Value -= amount;
            Debug.Log($"[Enemy] HP {Hp.Value}/{maxHp}");

            if (Hp.Value <= 0)
            {
                Debug.Log("[Enemy] Died — despawning.");
                NetworkObject.Despawn(true);
            }
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
