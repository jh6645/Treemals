using Unity.Netcode;
using UnityEngine;

namespace Treemals.Structures
{
    // Base class for all placeable structures.
    // Handles server-side lifetime. Override OnPlayerEnter for activation logic.
    public abstract class StructureBehaviour : NetworkBehaviour
    {
        [SerializeField] private float lifetime = 20f;

        private float spawnTime;

        // Called server-side immediately after NetworkObject.Spawn()
        public void ServerInit()
        {
            spawnTime = Time.time;
        }

        protected virtual void Update()
        {
            if (!IsServer) return;
            if (lifetime > 0f && Time.time - spawnTime >= lifetime)
                NetworkObject.Despawn(true);
        }
    }
}
