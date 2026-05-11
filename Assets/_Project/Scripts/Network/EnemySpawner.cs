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

        [Header("Debug")]
        [SerializeField] private bool spawningEnabled = true;
        [SerializeField] private bool debugLogs = false;

        private float _timer;
        private bool _lastSpawningEnabled = true;

        // Called by code (e.g. WaveController, GameManager) to toggle spawning.
        public void SetSpawning(bool value)
        {
            if (spawningEnabled == value) return;
            spawningEnabled = value;
            LogToggle();
        }

        private void Update()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

            // Detect Inspector toggle during Play Mode
            if (spawningEnabled != _lastSpawningEnabled)
                LogToggle();

            _timer += Time.deltaTime;
            if (_timer < spawnInterval) return;
            _timer = 0f;

            if (!spawningEnabled)
            {
                if (debugLogs) Debug.Log("[EnemySpawner] Spawn skipped — spawning disabled");
                return;
            }

            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 spawnPos = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                Mathf.Sin(angle) * spawnRadius,
                0f
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();

            if (debugLogs) Debug.Log($"[EnemySpawner] Spawned at ({spawnPos.x:F1}, {spawnPos.y:F1})");
        }

        private void LogToggle()
        {
            _lastSpawningEnabled = spawningEnabled;
            if (debugLogs) Debug.Log($"[EnemySpawner] Spawning {(spawningEnabled ? "enabled" : "disabled")}");
        }
    }
}
