using UnityEngine;

namespace Treemals.Characters
{
    // Plain MonoBehaviour — not networked.
    // Spawned locally on every client via PlayerCombat's ClientRpc.
    // Moves from spawn position toward the target and destroys itself on arrival.
    public class ProjectileVisual : MonoBehaviour
    {
        [SerializeField] private float speed = 12f;

        private Vector3 _target;

        public void Initialize(Vector3 target)
        {
            _target = target;
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target) < 0.05f)
                Destroy(gameObject);
        }
    }
}
