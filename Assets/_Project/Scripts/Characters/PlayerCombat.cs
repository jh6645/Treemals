using Unity.Netcode;
using UnityEngine;

namespace Treemals.Characters
{
    // Add this alongside PlayerController on the Player prefab.
    // Server handles all damage math; ClientRpc fires a local visual on every client.
    public class PlayerCombat : NetworkBehaviour
    {
        [SerializeField] private int   maxHp          = 100;
        [SerializeField] private float attackRadius   = 5f;
        [SerializeField] private float attackInterval = 1f;
        [SerializeField] private int   attackDamage   = 10;
        [SerializeField] private GameObject projectilePrefab;

        public NetworkVariable<int> Hp = new();

        private float _attackTimer;

        public override void OnNetworkSpawn()
        {
            if (IsServer) Hp.Value = maxHp;
        }

        private void Update()
        {
            if (!IsServer) return;

            _attackTimer += Time.deltaTime;
            if (_attackTimer < attackInterval) return;
            _attackTimer = 0f;

            TryAttack();
        }

        private void TryAttack()
        {
            EnemyController nearest = null;
            float minDist = float.MaxValue;

            foreach (var enemy in EnemyController.All)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < attackRadius && dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }

            if (nearest == null) return;

            Vector3 targetPos = nearest.transform.position;
            nearest.TakeDamage(attackDamage);

            // Snapshot the target position before it potentially despawns,
            // then tell every client to play a local projectile visual.
            SpawnProjectileVisualClientRpc(transform.position, targetPos);
        }

        // Called by EnemyController on the server when an enemy makes contact.
        public void TakeDamage(int amount)
        {
            if (!IsServer) return;

            Hp.Value -= amount;
            Debug.Log($"[Player {OwnerClientId}] HP {Hp.Value}/{maxHp}");

            if (Hp.Value <= 0)
            {
                Debug.Log($"[Player {OwnerClientId}] Died — despawning.");
                NetworkObject.Despawn(true);
            }
        }

        // Runs on every client. Spawns a purely local (non-networked) projectile visual.
        // No network position sync needed — all clients receive the same from/to snapshot.
        [ClientRpc]
        private void SpawnProjectileVisualClientRpc(Vector3 from, Vector3 to)
        {
            if (projectilePrefab == null) return;

            GameObject proj = Instantiate(projectilePrefab, from, Quaternion.identity);
            proj.GetComponent<ProjectileVisual>().Initialize(to);
        }
    }
}
