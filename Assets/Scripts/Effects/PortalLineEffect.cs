using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// Controls a trail effect emitted by portals. They travel in the direction of the portal's destination.
    /// </summary>
    public class PortalLineEffect : Trail
    {
        [SerializeField] private Rigidbody2D rb;
        private Vector2 travelDirection = Vector2.zero;
        private Vector2 origin;
        private const float SPEED = 5f;

        /// <summary>
        /// Place this trail somewhere and make it move towards a destination.
        /// </summary>
        public void Init(Vector2 origin, Vector2 destination)
        {
            this.origin = origin;
            travelDirection = (destination - origin).normalized;
            rb.velocity = travelDirection * SPEED;
        }
    }
}