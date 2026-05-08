using UnityEngine;

namespace Treemals.Core
{
    // Attach to Main Camera in the Game scene.
    // PlayerController calls SetTarget() when the local player spawns.
    public class CameraFollow : MonoBehaviour
    {
        private Transform _target;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 pos = _target.position;
            pos.z = transform.position.z; // preserve camera depth
            transform.position = pos;
        }
    }
}
