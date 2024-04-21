using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace Enemy
{
    public class Tracker : EnemyShootBase, IEnemy
    {
        private enum EnemyState
        {
            Patrol,
            Pursuit,
            Attack
        }
        // fields 
        public int Tier { get => tier; }
        [SerializeField] private LayerMask wallsLayer;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private LayerMask invisWallLayer;
        [SerializeField] private LayerMask footprintLayer;

        private EnemyState behaviorState = EnemyState.Patrol;
        private float behaviorTimer = 0f;
        private float checkTimer = 0f;
        private AllAngle travelDirection = new();
        private AllAngle faceDirection = new();

        // constants
        private const float OBSTACLE_SCAN_RADIUS = 1f / 2f;
        private const float OBSTACLE_SCAN_DISTANCE = 1f / 3f;
        private const float RADAR_RADIUS = 10.0f; // radius of circle used to find player tracks
        private const float FOOTPRINT_DISTANCE_MIN = 1.0f; // distance to footprint that tracker must be before going to next one
        private const float FOOTPRINT_DISTANCE_MAX = 8f;
        private const float CHECK_TIMER_MAX = 0.10f;
        private const float WANDER_BEHAVIOR_TIMER_MIN = 3.0f;
        private const float WANDER_BEHAVIOR_TIMER_MAX = 5.0f;
        private const float CHASE_BEHAVIOR_TIMER_MAX = 0.5f;
        private const float PATH_CORRECTION_RANDOMIZER_MIN = 0f; // both in degrees
        private const float PATH_CORRECTION_RANDOMIZER_MAX = 20f;

        // necessary classes
        private GameObject target = null; // the current object being followed

        protected override void Behavior()
        {
            switch (behaviorState)
            {
                case EnemyState.Patrol:
                    WanderState();
                    break;
                case EnemyState.Pursuit:
                    ChaseState();
                    break;
                case EnemyState.Attack:
                    AttackState();
                    break;
            }

            rb.rotation = Mathf.LerpAngle(rb.rotation, faceDirection.Degree, 0.2f);
            behaviorTimer -= Time.deltaTime;
            checkTimer -= Time.deltaTime;
        }
        private void Attack(Vector2 position)
        {
            if (fireTimer >= fireRate) Fire(position);
            base.Behavior();
        }
        /// <summary>
        /// Finds and returns the closest footprint.
        /// </summary>
        private GameObject SearchFootprint()
        {
            List<Collider2D> footprintList = new(Physics2D.OverlapCircleAll(transform.position, RADAR_RADIUS, footprintLayer));
            float closestDistance = Mathf.Infinity;
            GameObject closestFootprint = null;

            foreach (Collider2D footprint in footprintList)
            {
                float distanceToFootprint = Vector2.Distance(transform.position, footprint.transform.position);

                if (!IsInLineOfSight(transform, footprint.transform, distanceToFootprint, invisWallLayer, footprintLayer)) continue;

                if (distanceToFootprint < closestDistance)
                {
                    closestDistance = distanceToFootprint;
                    closestFootprint = footprint.gameObject;
                }
            }

            return closestFootprint;
        }
        private float CorrectPath(Vector2 normalVector, Vector2 rigidbodyVector)
        {
            AllAngle angle = new()
            {
                Vector = Vector2.Reflect(rigidbodyVector, normalVector)
            };

            return angle.Degree;
        }
        private void ClearPath()
        {
            List<Collider2D> walls = new(Physics2D.OverlapCircleAll(transform.position, 2.0f, wallLayer));

            foreach (Collider2D wall in walls)
            {
                wall.GetComponent<Wall>().Die();
            }
        }
        private void WanderState()
        {
            WanderCheck();
            WanderBehavior();
        }
        private void WanderBehavior()
        {
            rb.velocity = travelDirection.Vector.normalized * moveSpeed;
            faceDirection.Degree = travelDirection.Degree;

            if (behaviorTimer > 0f || behaviorState != EnemyState.Patrol) return;

            behaviorTimer = Random.Range(WANDER_BEHAVIOR_TIMER_MIN, WANDER_BEHAVIOR_TIMER_MAX);
            travelDirection.Degree = Random.Range(0f, 360f);
        }
        private void WanderCheck()
        {
            if (checkTimer > 0f) return;

            checkTimer = CHECK_TIMER_MAX;
            RaycastHit2D obstacle = Physics2D.CircleCast(transform.position, OBSTACLE_SCAN_RADIUS, travelDirection.Vector, OBSTACLE_SCAN_DISTANCE, wallsLayer);

            if (obstacle.collider != null)
            {
                float random = Random.Range(PATH_CORRECTION_RANDOMIZER_MIN, PATH_CORRECTION_RANDOMIZER_MAX);
                travelDirection.Degree = random + CorrectPath(obstacle.normal, travelDirection.Vector);
            }

            List<Collider2D> playerTracks = new(Physics2D.OverlapCircleAll(transform.position, RADAR_RADIUS, playerLayer | footprintLayer));

            foreach (Collider2D track in playerTracks)
            {
                float distanceToTrack = Vector2.Distance(transform.position, track.transform.position);

                if (!IsInLineOfSight(transform, track.transform, distanceToTrack, invisWallLayer, playerLayer | footprintLayer)) continue;

                if (track.CompareTag("Player"))
                {
                    ChangeState(EnemyState.Attack);
                    target = track.gameObject;
                    return;
                }

                if (track.CompareTag("Footprint"))
                {
                    ChangeState(EnemyState.Pursuit);
                    target = track.gameObject;
                    return;
                }
            }
        }
        private void ChaseState()
        {
            ChaseCheck();
            ChaseBehavior();
        }
        private void ChaseBehavior()
        {
            if (behaviorTimer > 0f || behaviorState != EnemyState.Pursuit) return;

            ClearPath();
            behaviorTimer = CHASE_BEHAVIOR_TIMER_MAX;
            travelDirection.Vector = target.transform.position - transform.position;
            faceDirection.Degree = travelDirection.Degree;
            rb.velocity = travelDirection.Vector.normalized * moveSpeed;
        }
        private void ChaseCheck()
        {
            if (checkTimer > 0f) return;

            checkTimer = CHECK_TIMER_MAX;
            Collider2D playerScan = Physics2D.OverlapCircle(transform.position, RADAR_RADIUS, playerLayer);

            if (playerScan != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerScan.transform.position);

                if (IsInLineOfSight(transform, playerScan.transform, distanceToPlayer, invisWallLayer, playerLayer))
                {
                    if (distanceToPlayer <= attackRange)
                    {
                        ChangeState(EnemyState.Attack);
                        target = playerScan.gameObject;
                    }
                    return;
                }
            }

            float distanceToFootprint = Vector2.Distance(transform.position, target.transform.position);
            
            if (!target.TryGetComponent<Footprint>(out var currentFootprint))
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (IsInLineOfSight(transform, target.transform, distanceToFootprint, invisWallLayer, footprintLayer))
            {
                if (distanceToFootprint <= FOOTPRINT_DISTANCE_MIN) target = currentFootprint.NextFootprint.gameObject;
                else if (distanceToFootprint >= FOOTPRINT_DISTANCE_MAX) target = currentFootprint.PrevFootprint.gameObject;
            }
            else
            {
                target = currentFootprint.PrevFootprint.gameObject;
            }

            if (target == null) ChangeState(EnemyState.Patrol);
        }
        private void AttackState()
        {
            AttackCheck();
            AttackBehavior();
        }
        private void AttackBehavior() // unlike the other states, this one is run every frame
        {
            rb.velocity = Vector2.zero;
            behaviorTimer = 0f;

            if (behaviorState != EnemyState.Attack) return;

            faceDirection.Vector = target.transform.position - transform.position;
            Attack(target.transform.position);
        }
        private void AttackCheck()
        {
            if (checkTimer > 0f || behaviorState != EnemyState.Attack) return;

            checkTimer = CHECK_TIMER_MAX;
            float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);

            if (IsInLineOfSight(transform, target.transform, attackRange, invisWallLayer, playerLayer) && distanceToPlayer <= attackRange) return;

            target = SearchFootprint();
            ChangeState(EnemyState.Pursuit);
        }
        private void ChangeState(EnemyState state)
        {
            behaviorTimer = 0f;
            behaviorState = state;
        }
        private bool IsInLineOfSight(Transform origin, Transform target, float maxDist, LayerMask obstructingLayers, LayerMask targetLayers)
        {
            Vector2 dir = target.position - origin.position;
            RaycastHit2D ray = Physics2D.Raycast(origin.position, dir.normalized, maxDist, obstructingLayers | targetLayers);

            if (ray.collider == null) return false;

            int hitLayer = ray.collider.gameObject.layer;

            return (targetLayers & (1 << hitLayer)) != 0;
        }
    }
}
