using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Treemals.Core;

namespace Treemals.Characters
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            // Assign this player as the camera target
            CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
            if (cam != null) cam.SetTarget(transform);
        }

        private void Update()
        {
            if (!IsOwner) return;

            Vector2 input = ReadMovementInput();
            transform.Translate(input * moveSpeed * Time.deltaTime, Space.World);
        }

        private Vector2 ReadMovementInput()
        {
            Vector2 input = Vector2.zero;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  input.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  input.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;

            return input.normalized;
        }
    }
}
