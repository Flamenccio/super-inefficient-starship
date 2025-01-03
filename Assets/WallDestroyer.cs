using UnityEngine;

namespace Flamenccio.Objects
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class WallDestroyer : MonoBehaviour
    {
        public GameObject FollowTarget { get; private set; }
        public float Radius { get => collider.radius; }
        public bool Sleeping { get; private set; }
        private CircleCollider2D collider;

        private void Awake()
        {
            collider = GetComponent<CircleCollider2D>();
        }

        /// <summary>
        /// Sets target to follow
        /// </summary>
        /// <param name="target">Transform to follow</param>
        public void SetFollowTarget(Transform target)
        {
            if (!target)
            {
                Debug.LogWarning($"{name}: target is null");
                return;
            }
            
            FollowTarget = target;
        }

        /// <summary>
        /// Removes follow target and puts wall destroyer to sleep
        /// </summary>
        public void ClearTarget()
        {
            FollowTarget = null;
        }

        /// <summary>
        /// Sets radius of area to destroy walls.
        /// </summary>
        /// <param name="radius">Area of circle collider</param>
        public void SetRadius(float radius)
        {
            if (radius <= 0)
            {
                Debug.LogError($"{name}: invalid radius.");
                return;
            }
            
            collider.radius = radius;
        }

        public void SetSleep(bool sleep)
        {
            if (Sleeping == sleep)
            {
                return;
            }
            
            Sleeping = sleep;
            collider.enabled = !Sleeping;
        }
    }
}
