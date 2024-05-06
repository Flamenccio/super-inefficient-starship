using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    public class PortalLineEffect : Trail
    {
        [SerializeField] private Rigidbody2D rb;
        private Vector2 travelDirection = Vector2.zero;
        private Vector2 origin;
        private const float SPEED = 5f;

        public void Init(Vector2 origin, Vector2 destination)
        {
            this.origin = origin;
            travelDirection = (destination - origin).normalized;
            rb.velocity = travelDirection * SPEED;
        }
    }
}
