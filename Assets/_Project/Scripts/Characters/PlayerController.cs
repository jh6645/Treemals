using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Treemals.Core;

namespace Treemals.Characters
{
    public class PlayerController : NetworkBehaviour, ILaunchable
    {
        [SerializeField] private float moveSpeed = 5f;

        // Child GameObject holding SpriteRenderer. Arc and squash are applied here
        // so collision and network position remain flat and unaffected.
        [SerializeField] private Transform visualRoot;

        [SerializeField] private float jumpHeight = 0.3f;

        // Optional: assign a particle/sprite prefab to pop on landing (non-networked, owner only)
        [SerializeField] private GameObject landingFxPrefab;

        public Vector2 FacingDir { get; private set; } = Vector2.down;

        private bool isLaunched;
        public bool IsBeingLaunched => isLaunched;
        private CameraFollow _cameraFollow;

        public override void OnNetworkSpawn()
        {
            // DEBUG: green = local, red = remote
            visualRoot.GetComponent<SpriteRenderer>().color = IsOwner ? Color.green : Color.red;

            if (!IsOwner) return;

            // Game scene may not be loaded yet when host spawns — deferred in Update
            _cameraFollow = CameraFollow.Instance;
            _cameraFollow?.SetTarget(transform);
        }

        private void Update()
        {
            if (!IsOwner) return;

            // Host spawns before Game scene loads — pick up CameraFollow once it exists
            if (_cameraFollow == null && CameraFollow.Instance != null)
            {
                _cameraFollow = CameraFollow.Instance;
                _cameraFollow.SetTarget(transform);
            }

            if (isLaunched) return;

            Vector2 input = ReadMovementInput();
            if (input.sqrMagnitude > 0.01f)
                FacingDir = input;

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

        public void ApplyLaunch(Vector2 direction, float launchSpeed, float duration)
        {
            if (!IsOwner || isLaunched) return;
            StartCoroutine(LaunchRoutine(direction.normalized, launchSpeed, duration));
        }

        private IEnumerator LaunchRoutine(Vector2 dir, float speed, float duration)
        {
            isLaunched = true;

            // Wind-up: tall and narrow before burst
            visualRoot.localScale = new Vector3(0.6f, 1.5f, 1f);
            yield return new WaitForSeconds(0.06f);

            // Launch burst: wide and flat
            visualRoot.localScale = new Vector3(1.4f, 0.7f, 1f);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float progress = elapsed / duration;

                // Quadratic ease-out: strong initial burst that settles smoothly
                float speedFactor = Mathf.Pow(1f - progress, 2f);
                transform.Translate(dir * speed * speedFactor * Time.deltaTime, Space.World);

                // Fake arc: sine wave on visual root only — collision stays flat
                float arcY = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
                visualRoot.localPosition = new Vector3(0f, arcY, 0f);

                // Scale: blend from burst shape → airborne stretch at arc peak → back to burst
                float arcPhase = Mathf.Sin(progress * Mathf.PI);
                visualRoot.localScale = Vector3.Lerp(
                    new Vector3(1.4f, 0.7f, 1f),   // burst/ground shape
                    new Vector3(0.8f, 1.25f, 1f),   // airborne stretch at peak
                    arcPhase
                );

                elapsed += Time.deltaTime;
                yield return null;
            }

            visualRoot.localPosition = Vector3.zero;

            // Landing impact: exaggerated squash
            visualRoot.localScale = new Vector3(1.7f, 0.4f, 1f);

            _cameraFollow?.Shake(0.25f, 0.18f);

            if (landingFxPrefab != null)
                Instantiate(landingFxPrefab, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
            visualRoot.localScale = Vector3.one;

            isLaunched = false;
        }
    }
}
