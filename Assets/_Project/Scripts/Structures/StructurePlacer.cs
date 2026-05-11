using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Treemals.Characters;

namespace Treemals.Structures
{
    // Attached to the Player prefab alongside PlayerController.
    // Q = place active structure in front of player.
    // Add structures[] entries in the Inspector to unlock more structure types.
    public class StructurePlacer : NetworkBehaviour
    {
        [SerializeField] private StructureData[] structures;
        [SerializeField] private LayerMask blockingLayers;
        [SerializeField] private float overlapCheckRadius = 0.45f;

        private PlayerController playerController;
        private int activeIndex;

        private void Awake() => playerController = GetComponent<PlayerController>();

        private void Update()
        {
            if (!IsOwner) return;
            if (Keyboard.current.qKey.wasPressedThisFrame)
                TryPlace();
        }

        private void TryPlace()
        {
            if (structures == null || structures.Length == 0) return;

            var data = structures[activeIndex];
            Vector3 spawnPos = transform.position + (Vector3)(playerController.FacingDir * data.placementDistance);

            if (Physics2D.OverlapCircle(spawnPos, overlapCheckRadius, blockingLayers) != null) return;

            PlaceServerRpc(spawnPos, activeIndex);
        }

        [ServerRpc]
        private void PlaceServerRpc(Vector3 position, int structureIndex)
        {
            if (structures == null || structureIndex < 0 || structureIndex >= structures.Length) return;

            var data = structures[structureIndex];
            var go = Instantiate(data.prefab, position, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn(true);
            go.GetComponent<StructureBehaviour>().ServerInit();
        }
    }
}
