using UnityEngine;
using Flamenccio.Utility;
using System.Collections.Generic;
using Flamenccio.Core.Player;
using Flamenccio.Objects;
using Flamenccio.Utility.Timer;
using Flamenccio.Utility;

namespace Flamenccio.Enemy
{
    public class TrackerBehavior : EnemyBehavior
    {
        private enum EnemyState
        {
            Wander,
            Chase,
            Attack
        }

        // fields
        [SerializeField] private float fireRate;
        [SerializeField] private LayerMask wallsLayer;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private LayerMask invisWallLayer;
        [SerializeField] private LayerMask footprintLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float moveSpeed = 1.0f;
        [SerializeField] private float attackRange = 1.0f;

        // Timers
        private EventTimer behaviorTimer;
        private EventTimer checkTimer;
        private EventTimer attackTimer;
        
        private EnemyState behaviorState = EnemyState.Wander;
        private AllAngle travelDirection = new();
        private AllAngle faceDirection = new();
        private Rigidbody2D rb;

        // constants
        private const float OBSTACLE_SCAN_RADIUS = 1f / 2f;
        private const float OBSTACLE_SCAN_DISTANCE = 1f;
        private const float RADAR_RADIUS = 10.0f; // radius of circle used to find player tracks
        private const float FOOTPRINT_DISTANCE_MIN = 1.0f; // distance to footprint that tracker must be before going to next one
        private const float FOOTPRINT_DISTANCE_MAX = 8f;
        private const float CHECK_TIMER_MAX = 3f / 60f;
        private const float WANDER_BEHAVIOR_TIMER_MIN = 3.0f;
        private const float WANDER_BEHAVIOR_TIMER_MAX = 5.0f;
        private const float CHASE_BEHAVIOR_TIMER_MAX = 0.5f;
        private const float PATH_CORRECTION_RANDOMIZER_MIN = 0f; // both in degrees
        private const float PATH_CORRECTION_RANDOMIZER_MAX = 20f;
        private readonly List<string> playerTrackTags = new() { TagManager.GetTag(Tag.Player), TagManager.GetTag(Tag.PlayerFootprint) };

        // necessary classes
        private GameObject target; // the current object being followed

        public override void OnSpawn()
        {
            base.OnSpawn();
            
            if (!TryGetComponent(out rb))
            {
                Debug.LogError($"{name}: Rigidbody2D not found");
                enabled = false;
                return;
            }
            
            // Initialize event timers
            behaviorTimer = new(1.0f, true);
            checkTimer = new(CHECK_TIMER_MAX, true);
            attackTimer = new(fireRate, true);
            attackTimer.AddLapListener(() => Attack(target.transform.position));
            ChangeState(EnemyState.Wander);
        }

        protected override void Behavior()
        {
            rb.rotation = Mathf.LerpAngle(rb.rotation, faceDirection.Degree, 0.2f);
        }

        private void Attack(Vector2 position)
        {
            // TODO spawn bullet
        }

        #region States
        #region Wander state

        private void WanderBehavior()
        {
            rb.linearVelocity = travelDirection.Vector.normalized * moveSpeed;
            faceDirection.Degree = travelDirection.Degree;

            // Set behavior timer to a random value
            var newTime = UnityEngine.Random.Range(WANDER_BEHAVIOR_TIMER_MIN, WANDER_BEHAVIOR_TIMER_MAX);
            behaviorTimer.SetLapTime(newTime);
            travelDirection.Degree = UnityEngine.Random.Range(0f, 360f);
        }

        private void WanderCheck()
        {
            RaycastHit2D obstacle = Physics2D.CircleCast(transform.position, OBSTACLE_SCAN_RADIUS, travelDirection.Vector, OBSTACLE_SCAN_DISTANCE, wallsLayer);

            if (!obstacle)
            {
                var random = UnityEngine.Random.Range(PATH_CORRECTION_RANDOMIZER_MIN, PATH_CORRECTION_RANDOMIZER_MAX);
                travelDirection.Degree = random + CorrectPath(obstacle.normal, travelDirection.Vector);
            }

            var track = EntityScanner.SearchForNearestGameObjectsWithTag(playerTrackTags, transform.position, true, RADAR_RADIUS, footprintLayer, invisWallLayer);

            // If no tracks found, return
            if (!track) return;

            target = track;

            if (track.CompareTag(TagManager.GetTag(Tag.Player)))
            {
                ChangeState(EnemyState.Attack);
            }
            else if (track.CompareTag(TagManager.GetTag(Tag.PlayerFootprint)))
            {
                ChangeState(EnemyState.Chase);
            }
            else
            {
                Debug.LogError($"{name}: track tag not {Tag.Player} and not {Tag.PlayerFootprint}");
            }
        }
        #endregion

        #region Chase state

        private void ChaseBehavior()
        {
            if (!target) return;

            ClearPath();
            travelDirection.Vector = target.transform.position - transform.position;
            faceDirection.Degree = travelDirection.Degree;
            rb.linearVelocity = travelDirection.Vector.normalized * moveSpeed;
        }

        private void ChaseCheck()
        {
            GameObject playerScan = EntityScanner.SearchForNearestGameObjectsWithTag(transform.position, true, attackRange, playerLayer, invisWallLayer, TagManager.GetTag(Tag.Player));

            if (playerScan)
            {
                ChangeState(EnemyState.Attack);
                target = playerScan;

                return;
            }

            // Try to get the Footprint script from target.
            if (!target || !target.TryGetComponent<Footprint>(out var currentFootprint))
            {
                ChangeState(EnemyState.Wander);
                target = null;

                return;
            }

            float distanceToFootprint = Vector2.Distance(transform.position, target.transform.position);
            target = FollowFootprint(currentFootprint, distanceToFootprint);

            if (!target)
            {
                ChangeState(EnemyState.Wander);
            }
        }
        #endregion

        #region Attack state

        // Unlike other states, this one is run every frame.
        private void AttackBehavior()
        {
            rb.linearVelocity = Vector2.zero;

            if (behaviorState != EnemyState.Attack || !target) return;

            faceDirection.Vector = target.transform.position - transform.position;
            Attack(target.transform.position);
        }

        private void AttackCheck()
        {
            if (behaviorState != EnemyState.Attack) return;

            if (!target)
            {
                ChangeState(EnemyState.Wander);
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);

            if (!EntityScanner.IsInLineOfSight(transform.position, target.transform.position, attackRange, invisWallLayer, playerLayer)
                || distanceToPlayer > attackRange)
            {
                target = SearchFootprint();
                ChangeState(EnemyState.Chase);
            }
        }
        #endregion
        #endregion

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
            // TODO instead of using this method, instantiate a GameObject who triggers walls to self-destruct
            
            List<Collider2D> walls = new(Physics2D.OverlapCircleAll(transform.position, 2.0f, wallLayer));

            foreach (Collider2D wall in walls)
            {
                wall.GetComponent<Wall>().Die();
            }
        }

        #region Footprints
        /// <summary>
        /// Finds and returns the closest footprint.
        /// </summary>
        private GameObject SearchFootprint()
        {
            var closestFootprint = EntityScanner.SearchForNearestGameObjectsWithTag(transform.position, true, RADAR_RADIUS, footprintLayer, invisWallLayer, TagManager.GetTag(Tag.PlayerFootprint));

            return closestFootprint;
        }

        /// <summary>
        /// Follows the trail of footprints, either moving to the next footprint or previous footprint based on the distance.
        /// </summary>
        /// <param name="currentFootprint">The current target footprint.</param>
        /// <param name="distanceToFootprint">Distance to the current target footprint.</param>
        /// <returns>The next footprint in the trail.</returns>
        private GameObject FollowFootprint(Footprint currentFootprint, float distanceToFootprint)
        {
            // If current footprint doesn't exist
            if (!currentFootprint)
            {
                return null;
            }
            
            if (!EntityScanner.IsInLineOfSight(transform.position, target.transform.position, distanceToFootprint, invisWallLayer, footprintLayer))
            {
                return target = GetPreviousFootprint(currentFootprint);
            }
            
            if (distanceToFootprint <= FOOTPRINT_DISTANCE_MIN)
            {
                return currentFootprint.NextFootprint.gameObject;
            }
            
            if (distanceToFootprint >= FOOTPRINT_DISTANCE_MAX)
            {
                return GetPreviousFootprint(currentFootprint);
            }
                
            return currentFootprint.gameObject;
        }

        /// <summary>
        /// Safely retrives and returns the previous footprint of the current footprint.
        /// </summary>
        /// <param name="currentFootprint">The current footprint that's being followed.</param>
        /// <returns>The GameObject of the previous footprint; null if the current footprint is the oldest.</returns>
        private GameObject GetPreviousFootprint(Footprint currentFootprint)
        {
            if (!currentFootprint)
            {
                return null;
            }
            
            return currentFootprint.PrevFootprint ? currentFootprint.PrevFootprint.gameObject : null;
        }
        #endregion

        private void ChangeState(EnemyState state)
        {
            //behaviorMaxTime = 0f;
            behaviorState = state; 
            behaviorTimer.StopTimer();
            behaviorTimer.ClearLapListeners();
            checkTimer.StopTimer();
            checkTimer.ClearLapListeners();

            switch (state)
            {
                case EnemyState.Attack:
                    behaviorTimer.AddLapListener(AttackBehavior);
                    checkTimer.AddLapListener(AttackCheck);
                    behaviorTimer.SetLapTime(0f);
                    break;

                case EnemyState.Chase:
                    behaviorTimer.AddLapListener(ChaseBehavior);
                    checkTimer.AddLapListener(ChaseCheck);
                    behaviorTimer.SetLapTime(CHASE_BEHAVIOR_TIMER_MAX);
                    break;

                case EnemyState.Wander:
                    behaviorTimer.AddLapListener(WanderBehavior);
                    checkTimer.AddLapListener(WanderCheck);
                    behaviorTimer.SetLapTime(WANDER_BEHAVIOR_TIMER_MAX);
                    break;
            }
            
            checkTimer.StartTimer();
            behaviorTimer.StartTimer();
        }
    }
}
