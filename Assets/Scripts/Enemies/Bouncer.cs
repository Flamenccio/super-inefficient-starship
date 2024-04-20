using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Enemy
{
    public class Bouncer : TurretSpin, IEnemy
    {
        [SerializeField] private LayerMask invisWallLayer;
        private Vector2 travelDirection = Vector2.up;
        private RaycastHit2D raycast;
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

            rb.velocity = travelDirection * moveSpeed;
            TravelRay();
            base.Behavior();
        }
        private void TravelRay()
        {
            raycast = Physics2D.CircleCast(transform.position, CIRCLE_CAST_RADIUS, travelDirection, RAY_LENGTH, invisWallLayer);
            if (raycast.collider != null)
            {
                travelDirection = Vector2.Reflect(travelDirection, raycast.normal);
            }
        }
    }
}
