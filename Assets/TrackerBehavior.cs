using UnityEngine;
using Flamenccio.Utility;
using System.Collections.Generic;
using Flamenccio.Core.Player;
using Flamenccio.Objects;
using Flamenccio.Utility.Timer;
using Flamenccio.Utility;
using UnityEditor;

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
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private LayerMask invisWallLayer;
        [SerializeField] private LayerMask footprintLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float moveSpeed = 1.0f;
        [SerializeField] private float attackRange = 1.0f;
        [SerializeField] private GameObject bulletPrefab;

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
        private const float OBSTACLE_SCAN_DISTANCE = 1.0f;
        private const float RADAR_RADIUS = 10.0f; // radius of circle used to find player tracks
        private const float FOOTPRINT_DISTANCE_MIN = 1.0f; // distance to footprint that tracker must be before going to next one
        private const float FOOTPRINT_DISTANCE_MAX = 8f;
        private const float CHECK_TIMER_MAX = 3f / 60f;
        private const float WANDER_BEHAVIOR_TIMER_MIN = 3.0f;
        private const float WANDER_BEHAVIOR_TIMER_MAX = 5.0f;
        private const float CHASE_BEHAVIOR_TIMER_MAX = 0.5f;
        private const float PATH_CORRECTION_RANDOMIZER = 20f;
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
        }

        private void Start()
        {
            // Initialize event timers
            behaviorTimer = new(1.0f, true);
            checkTimer = new(CHECK_TIMER_MAX, true);
            attackTimer = new(fireRate, true);
            attackTimer.AddLapListener(() => Attack(target.transform.position));
            attackTimer.StopTimer();
            travelDirection.Degree = 0f;
            ChangeState(EnemyState.Wander, true);
        }

        protected override void Behavior()
        {
            if (behaviorState == EnemyState.Attack)
            {
                // Faces player when attacking
                faceDirection.Vector = target.transform.position - transform.position;
            }
            
            rb.rotation = Mathf.LerpAngle(rb.rotation, faceDirection.Degree, 0.2f);
        }

        private void Attack(Vector2 position)
        {
            var angle = new AllAngle()
            {
                Vector = position - (Vector2)transform.position
            };
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, angle.Degree));
        }

        #region States
        #region Wander state

        private void WanderBehavior()
        {
            var converter = new AllAngle();
            converter.Degree = UnityEngine.Random.Range(0f, 360f);
            MoveInDirection(converter.Vector);
            
            // Set behavior timer to a random value
            /*
            var newTime = UnityEngine.Random.Range(WANDER_BEHAVIOR_TIMER_MIN, WANDER_BEHAVIOR_TIMER_MAX);
            behaviorTimer.StopTimer();
            behaviorTimer.SetLapTime(newTime);
            behaviorTimer.StartTimer();
            */
        }

        private void WanderCheck()
        {
            RaycastHit2D obstacle = Physics2D.CircleCast(transform.position, OBSTACLE_SCAN_RADIUS, rb.linearVelocity.normalized, OBSTACLE_SCAN_DISTANCE, wallLayer | invisWallLayer);

            if (obstacle)
            {
                var random = UnityEngine.Random.Range(-PATH_CORRECTION_RANDOMIZER, PATH_CORRECTION_RANDOMIZER);
                var converter = new AllAngle();
                converter.Degree = random + CorrectPath(obstacle.normal, rb.linearVelocity.normalized);
                MoveInDirection(converter.Vector);
            }

            var track = EntityScanner.SearchForNearestGameObjectsWithTag(
                transform.position, 
                true, 
                RADAR_RADIUS, 
                footprintLayer | playerLayer, 
                invisWallLayer, 
                TagManager.GetTag(Tag.PlayerFootprint), TagManager.GetTag(Tag.Player));

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
            MoveInDirection(target.transform.position - transform.position);
        }

        private void ChaseCheck()
        {
            // Check for nearby player in line of sight
            GameObject playerScan = EntityScanner.SearchForNearestGameObjectsWithTag(transform.position, true, attackRange, playerLayer, invisWallLayer, TagManager.GetTag(Tag.Player));

            if (playerScan)
            {
                target = playerScan;
                ChangeState(EnemyState.Attack);
                return;
            }

            // TODO FIX
            // If the current target doesn't exist or is not a footprint, wander
            if (!target)
            {
                Debug.Log("Wandering: target doesn't exist");
                ChangeState(EnemyState.Wander);
                target = null;
                return;
            }

            if (!target.CompareTag(TagManager.GetTag(Tag.PlayerFootprint)))
            {
                Debug.Log("Wandering: target is not a footprint");
                target = null;
                ChangeState(EnemyState.Wander);
                return;
            }

            if (!target.TryGetComponent<Footprint>(out var currentFootprint))
            {
                Debug.Log("Wandering: no footprint script found on target");
                target = null;
                ChangeState(EnemyState.Wander);
                return;
            }
            
            float distanceToFootprint = Vector2.Distance(transform.position, target.transform.position);
            target = FollowFootprint(currentFootprint, distanceToFootprint);

            if (!target)
            {
                Debug.Log("Wandering: could not find footprint");
                ChangeState(EnemyState.Wander);
            }
        }
        #endregion

        #region Attack state

        private void AttackBehavior()
        {
            rb.linearVelocity = Vector2.zero;

            if (!target) return;

            Attack(target.transform.position);
        }

        private void AttackCheck()
        {
            if (!target)
            {
                ChangeState(EnemyState.Wander);
                return;
            }

            var distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);
            var isInLineOfSight = EntityScanner.IsInLineOfSight(
                transform.position, 
                target.transform.position, 
                Mathf.Min(distanceToPlayer, attackRange), 
                invisWallLayer, 
                playerLayer);

            if (!isInLineOfSight || distanceToPlayer > attackRange)
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
            var closestFootprint = EntityScanner.SearchForNearestGameObjectsWithTag(
                transform.position, 
                true, 
                RADAR_RADIUS, 
                footprintLayer, 
                invisWallLayer, 
                TagManager.GetTag(Tag.PlayerFootprint));
            
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

        private void ChangeState(EnemyState state, bool force = false)
        {
            if (behaviorState == state && !force) return;
            
            Debug.Log($"Switch state to {state}");
            
            behaviorState = state; 
            behaviorTimer.StopTimer();
            behaviorTimer.ClearLapListeners();
            behaviorTimer.ClearOffsetListeners();
            checkTimer.StopTimer();
            checkTimer.ClearLapListeners();
            checkTimer.ClearOffsetListeners();

            switch (state)
            {
                case EnemyState.Attack:
                    // These AddOffsetListener are used instead of AddLapListener to set execution at the start
                    //behaviorTimer.AddOffsetListener(AttackBehavior, 0f, EventTimer.OffsetListener.OffsetReferencePoint.FromStart);
                    //checkTimer.AddOffsetListener(AttackCheck, 0f, EventTimer.OffsetListener.OffsetReferencePoint.FromStart);
                    behaviorTimer.SetLapTime(fireRate);
                    behaviorTimer.AddLapListener(AttackBehavior);
                    checkTimer.AddLapListener(AttackCheck);
                    behaviorTimer.AddOffsetListener(() => AttackTelegraph?.Invoke(), ATTACK_TELEGRAPH_DURATION, EventTimer.OffsetListener.OffsetReferencePoint.FromEnd);
                    rb.linearVelocity = Vector2.zero; // Stop movement when attacking
                    break;

                case EnemyState.Chase:
                    //behaviorTimer.AddOffsetListener(ChaseBehavior, 0f, EventTimer.OffsetListener.OffsetReferencePoint.FromStart);
                    //checkTimer.AddOffsetListener(ChaseCheck, 0f, EventTimer.OffsetListener.OffsetReferencePoint.FromStart);
                    behaviorTimer.AddLapListener(ChaseBehavior);
                    checkTimer.AddLapListener(ChaseCheck);
                    behaviorTimer.SetLapTime(CHASE_BEHAVIOR_TIMER_MAX);
                    break;

                case EnemyState.Wander:
                    //behaviorTimer.AddOffsetListener(WanderBehavior, 0f, EventTimer.OffsetListener.OffsetReferencePoint.FromStart);
                    //checkTimer.AddOffsetListener(WanderCheck, 0f, EventTimer.OffsetListener.OffsetReferencePoint.FromStart);
                    behaviorTimer.AddLapListener(WanderBehavior);
                    checkTimer.AddLapListener(WanderCheck);
                    behaviorTimer.SetLapTime(WANDER_BEHAVIOR_TIMER_MAX);
                    break;
            }
            
            checkTimer.StartTimer();
            behaviorTimer.StartTimer();
        }

        /// <summary>
        /// Immediately moves enemy in given direction.
        /// </summary>
        /// <param name="direction">Direction to move in.</param>
        private void MoveInDirection(Vector2 direction)
        {
            var normalizedDirection = direction.normalized;
            rb.linearVelocity = normalizedDirection * moveSpeed;
            faceDirection.Vector = normalizedDirection;
        }
    }
}
