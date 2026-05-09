using Unity.Netcode;
using UnityEngine;

namespace Treemals.Network
{
    // Place one instance of this in the Game scene.
    // Only the host/server spawns enemies; clients ignore this entirely.
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnInterval = 3f;
        [SerializeField] private float spawnRadius = 15f;

        private float _timer;

        private void Update()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

            _timer += Time.deltaTime;
            if (_timer < spawnInterval) return;

            _timer = 0f;
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            // Random point on a circle — uniform coverage from all directions.
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 spawnPos = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                Mathf.Sin(angle) * spawnRadius,
                0f
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();

            Debug.Log($"[EnemySpawner] Spawned at ({spawnPos.x:F1}, {spawnPos.y:F1})");
        }
    }
}
