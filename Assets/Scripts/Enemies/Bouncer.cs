using UnityEngine;
using Flamenccio.Utility;

namespace Enemy
{
    /// <summary>
    /// Controls the behavior of an enemy. Rotates and slowly moves around the map, bouncing off of obstacles. Fires bouncing bullets periodically in four directions.
    /// </summary>
    public class Bouncer : TurretSpin, IEnemy
    {
        [SerializeField] private LayerMask invisWallLayer;
        private Vector2 travelDirection = Vector2.up;
        private const float RAY_LENGTH = 0.25f;
        private const float CIRCLE_CAST_RADIUS = 0.5f;

        protected override void OnSpawn()
        {
            AllAngle randomDirection = new()
            {
                Degree = Random.Range(0f, 360f)
            };
            travelDirection = randomDirection.Vector;
            base.OnSpawn();
        }

        protected override void Behavior()
        {
            rb.linearVelocity = travelDirection * moveSpeed;
            TravelRay();
            base.Behavior();
        }

        private void TravelRay()
        {
            RaycastHit2D raycast = Physics2D.CircleCast(transform.position, CIRCLE_CAST_RADIUS, travelDirection, RAY_LENGTH, invisWallLayer);

            if (raycast.collider != null)
            {
                travelDirection = Vector2.Reflect(travelDirection, raycast.normal);
            }
        }
    }
}