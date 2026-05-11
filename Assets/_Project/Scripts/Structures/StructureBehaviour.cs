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

        // Called server-side immediately after NetworkObject.Spawn().
        // facingDir is forwarded to OnPlaced so subclasses can store directional state.
        public void ServerInit(Vector2 facingDir = default)
        {
            spawnTime = Time.time;
            OnPlaced(facingDir);
        }

        // Override in subclasses to react to placement direction (e.g. JumpPadStructure stores launch dir).
        protected virtual void OnPlaced(Vector2 facingDir) { }

        protected virtual void Update()
        {
            if (!IsServer) return;
            if (lifetime > 0f && Time.time - spawnTime >= lifetime)
                NetworkObject.Despawn(true);
        }
    }
}
