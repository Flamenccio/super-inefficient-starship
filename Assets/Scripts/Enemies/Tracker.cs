using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Objects;
using Flamenccio.Core.Player;
using System;

namespace Enemy
{
    /// <summary>
    /// Controls an enemy. Searches for the player and follows them if they are in range.
    /// <para>When the player is in line of sight and in range, fires periodically.</para>
    /// </summary>
    public class Tracker : EnemyShootBase, IEnemy
    {
        public int Tier { get => tier; }

        private enum EnemyState
        {
            Wander,
            Chase,
            Attack
        }

        // fields
        [SerializeField] private LayerMask wallsLayer;

        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private LayerMask invisWallLayer;
        [SerializeField] private LayerMask footprintLayer;

        private EnemyState behaviorState = EnemyState.Wander;
        private float behaviorTimer = 0f;
        private float checkTimer = 0f;
        private AllAngle travelDirection = new();
        private AllAngle faceDirection = new();
        private Action StateBehavior;

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
        private GameObject target = null; // the current object being followed

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ChangeState(EnemyState.Wander);
        }

        protected override void Behavior()
        {
            StateBehavior?.Invoke();
            rb.rotation = Mathf.LerpAngle(rb.rotation, faceDirection.Degree, 0.2f);
            behaviorTimer -= Time.deltaTime;
            checkTimer -= Time.deltaTime;
        }

        private void Attack(Vector2 position)
        {
            if (fireTimer >= fireRate) Fire(position);
            base.Behavior();
        }

        #region States
        #region Wander state
        private void WanderState()
        {
            WanderCheck();
            WanderBehavior();
        }

        private void WanderBehavior()
        {
            rb.velocity = travelDirection.Vector.normalized * moveSpeed;
            faceDirection.Degree = travelDirection.Degree;

            if (behaviorTimer > 0f || behaviorState != EnemyState.Wander) return;

            behaviorTimer = UnityEngine.Random.Range(WANDER_BEHAVIOR_TIMER_MIN, WANDER_BEHAVIOR_TIMER_MAX);
            travelDirection.Degree = UnityEngine.Random.Range(0f, 360f);
        }

        private void WanderCheck()
        {
            if (checkTimer > 0f || behaviorState != EnemyState.Wander) return;

            checkTimer = CHECK_TIMER_MAX;
            RaycastHit2D obstacle = Physics2D.CircleCast(transform.position, OBSTACLE_SCAN_RADIUS, travelDirection.Vector, OBSTACLE_SCAN_DISTANCE, wallsLayer);

            if (obstacle.collider != null)
            {
                float random = UnityEngine.Random.Range(PATH_CORRECTION_RANDOMIZER_MIN, PATH_CORRECTION_RANDOMIZER_MAX);
                travelDirection.Degree = random + CorrectPath(obstacle.normal, travelDirection.Vector);
            }

            GameObject track = SearchForGameObjectsWithTag(playerTrackTags, true, RADAR_RADIUS, playerLayer | footprintLayer, invisWallLayer);

            if (track == null) return;

            target = track;

            if (track.CompareTag(TagManager.GetTag(Tag.Player)))
            {
                ChangeState(EnemyState.Attack);
            }
            else if (track.CompareTag(TagManager.GetTag(Tag.PlayerFootprint)))
            {
                ChangeState(EnemyState.Chase);
            }
        }
        #endregion

        #region Chase state
        private void ChaseState()
        {
            ChaseCheck();
            ChaseBehavior();
        }

        private void ChaseBehavior()
        {
            if (behaviorTimer > 0f || behaviorState != EnemyState.Chase || target == null) return;

            ClearPath();
            behaviorTimer = CHASE_BEHAVIOR_TIMER_MAX;
            travelDirection.Vector = target.transform.position - transform.position;
            faceDirection.Degree = travelDirection.Degree;
            rb.velocity = travelDirection.Vector.normalized * moveSpeed;
        }

        private void ChaseCheck()
        {
            if (checkTimer > 0f || behaviorState != EnemyState.Chase) return;

            checkTimer = CHECK_TIMER_MAX;
            GameObject playerScan = SearchForGameObjectsWithTag(new() { TagManager.GetTag(Tag.Player) }, true, attackRange, playerLayer, invisWallLayer);

            if (playerScan != null)
            {
                ChangeState(EnemyState.Attack);
                target = playerScan;

                return;
            }

            // Try to get the Footprint script from target.
            if (target == null || !target.TryGetComponent<Footprint>(out var currentFootprint))
            {
                ChangeState(EnemyState.Wander);
                target = null;

                return;
            }

            float distanceToFootprint = Vector2.Distance(transform.position, target.transform.position);
            target = FollowFootprint(currentFootprint, distanceToFootprint);

            if (target == null) ChangeState(EnemyState.Wander);
        }
        #endregion

        #region Attack state
        private void AttackState()
        {
            AttackCheck();
            AttackBehavior();
        }

        // Unlike other states, this one is run every frame.
        private void AttackBehavior()
        {
            rb.velocity = Vector2.zero;

            if (behaviorState != EnemyState.Attack || target == null) return;

            faceDirection.Vector = target.transform.position - transform.position;
            Attack(target.transform.position);
        }

        private void AttackCheck()
        {
            if (checkTimer > 0f || behaviorState != EnemyState.Attack) return;

            if (target == null)
            {
                ChangeState(EnemyState.Wander);
                return;
            }

            checkTimer = CHECK_TIMER_MAX;
            float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);

            if (!IsInLineOfSight(transform, target.transform, attackRange, invisWallLayer, playerLayer)
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
            List<Collider2D> walls = new(Physics2D.OverlapCircleAll(transform.position, 2.0f, wallLayer));

            foreach (Collider2D wall in walls)
            {
                wall.GetComponent<Wall>().Die();
            }
        }

        /// <summary>
        /// Returns a nearby GameObject with given tag.
        /// </summary>
        /// <param name="tags">The possible tags that valid GameObjects can have.</param>
        /// <param name="inLineOfSight">Do these GameObjects have to be within line of sight?</param>
        /// <param name="searchRadius">How far to search.</param>
        /// <param name="obstructingLayers">The layers that may obstruct line of sight.</param>
        /// <param name="targetLayers">The layers where the target GameObjects reside.</param>
        /// <returns>A GameObject. Null if there are no such GameObjects.</returns>
        private GameObject SearchForGameObjectsWithTag(List<string> tags, bool inLineOfSight, float searchRadius, LayerMask targetLayers, LayerMask obstructingLayers)
        {
            List<Collider2D> playerTracks = new(Physics2D.OverlapCircleAll(transform.position, searchRadius, targetLayers));

            return playerTracks
                .ConvertAll(a => a.gameObject)
                .Find(x =>
                {
                    float distanceToTrack = Vector2.Distance(transform.position, x.transform.position);
                    bool tagMatch = tags.Contains(x.tag);
                    bool isInLineOfSight = IsInLineOfSight(transform, x.transform, distanceToTrack, obstructingLayers, targetLayers);

                    return tagMatch && (!inLineOfSight || isInLineOfSight);
                });
        }

        #region Footprints
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

        /// <summary>
        /// Follows the trail of footprints, either moving to the next footprint or previous footprint based on the distance.
        /// </summary>
        /// <param name="currentFootprint">The current target footprint.</param>
        /// <param name="distanceToFootprint">Distance to the current target footprint.</param>
        /// <returns>The next footprint in the trail.</returns>
        private GameObject FollowFootprint(Footprint currentFootprint, float distanceToFootprint)
        {
            if (currentFootprint == null)
            {
                return null;
            }
            else if (!IsInLineOfSight(transform, target.transform, distanceToFootprint, invisWallLayer, footprintLayer))
            {
                return target = GetPreviousFootprint(currentFootprint);
            }
            else if (distanceToFootprint <= FOOTPRINT_DISTANCE_MIN)
            {
                return currentFootprint.NextFootprint.gameObject;
            }
            else if (distanceToFootprint >= FOOTPRINT_DISTANCE_MAX)
            {
                return GetPreviousFootprint(currentFootprint);
            }
            else
            {
                return currentFootprint.gameObject;
            }
        }

        /// <summary>
        /// Safely retrives and returns the previous footprint of the current footprint.
        /// </summary>
        /// <param name="currentFootprint">The current footprint that's being followed.</param>
        /// <returns>The GameObject of the previous footprint; null if the current footprint is the oldest.</returns>
        private GameObject GetPreviousFootprint(Footprint currentFootprint)
        {
            if (currentFootprint == null)
            {
                return null;
            }
            else
            {
                return currentFootprint.PrevFootprint != null ? currentFootprint.PrevFootprint.gameObject : null;
            }
        }
        #endregion

        private void ChangeState(EnemyState state)
        {
            behaviorTimer = 0f;
            behaviorState = state;

            switch (state)
            {
                case EnemyState.Attack:
                    StateBehavior = AttackState;
                    break;

                case EnemyState.Chase:
                    StateBehavior = ChaseState;
                    break;

                case EnemyState.Wander:
                    StateBehavior = WanderState;
                    break;
            }
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