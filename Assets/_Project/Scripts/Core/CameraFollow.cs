using System.Collections;
using UnityEngine;

namespace Treemals.Core
{
    // Attach to Main Camera in the Game scene.
    // PlayerController calls SetTarget() when the local player spawns.
    public class CameraFollow : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _shakeOffset;
        private Coroutine _shakeCoroutine;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        // Called by PlayerController on landing. Safe to call from non-owner clients — noop if camera has no target.
        public void Shake(float duration, float magnitude)
        {
            if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float decay = 1f - (elapsed / duration);
                _shakeOffset = (Vector3)(Random.insideUnitCircle * magnitude * decay);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _shakeOffset = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 pos = _target.position + _shakeOffset;
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}
