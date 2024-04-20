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

        private AllAngle intentAngle = new(); // the direction the tracker wants to travel in
        private float intentTimer = 0; // how long the tracker will travel in direction specified by intentAngle
        private AllAngle correctAngle = new();
        private bool rerouting = false;
        private EnemyState behaviorState = EnemyState.Patrol;
        private float wanderTimer = 0f;
        private float behaviorTimer = 0f;
        private AllAngle travelDirection = new();

        // constants
        private const float VISION_RANGE = 1.0f;
        private const float FIRING_RANGE = 8.0f;
        private const float PATROL_TIME = 1.0f;
        private const float CIRCLE_CAST_RADIUS = 0.25f;
        private const float WANDER_TIMER_MAX = 5.0f;
        private const float WANDER_TIMER_MIN = 3.0f;
        private const float CHASE_TIMER_MAX = 0.25f;
        private const float RADAR_RADIUS = 10.0f; // radius of circle used to find player tracks

        // necessary classes
        private GameObject obstacle;
        private GameObject target = null; // the current object being followed
        private List<GameObject> ignoreList = new();

        protected override void Behavior()
        {
            Debug.Log($"Current behavior: {behaviorState}");

            switch (behaviorState) // TODO each state needs to be cleaned up
            {
                case EnemyState.Patrol:
                    //PatrolBehaviorPrecheck();
                    //PatrolBehavior();
                    WanderState();
                    break;
                case EnemyState.Pursuit:
                    //PursuitBehaviorPrecheck();
                    //PursuitBehavior();
                    ChaseState();
                    break;
                case EnemyState.Attack:
                    //AttackBehavior();
                    AttackState();
                    break;
            }

            behaviorTimer -= Time.deltaTime;
        }
        private RaycastHit2D LineOfSight(Vector2 direction, float distance, LayerMask lmask)
        {
            return Physics2D.CircleCast(transform.position, CIRCLE_CAST_RADIUS, direction, distance - CIRCLE_CAST_RADIUS, lmask);
        }
        private void PatrolBehavior()
        {
            if (behaviorState != EnemyState.Patrol) { return; }

            intentTimer += Time.deltaTime;

            if (intentTimer >= PATROL_TIME)
            { // if the timer runs out, choose another direction to travel in
                intentAngle.Degree = Random.Range(0f, 360f);
                intentTimer = 0f;
            }

            RaycastHit2D lineOfSight = LineOfSight(intentAngle.Vector, VISION_RANGE, wallsLayer);
            obstacle = LookForWalls(lineOfSight);

            if (obstacle != null) // if there is an obstacle in the way, find another path
            {
                if (!rerouting)
                {
                    rerouting = true;
                    correctAngle.Degree = CorrectPath(lineOfSight.normal, new Vector2(Mathf.Cos(rb.rotation), Mathf.Sin(rb.rotation)));
                }
                if (rerouting)
                {
                    float newAngle = (correctAngle.Degree + rb.rotation) % 360;
                    intentAngle.Vector = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    intentTimer = -1f;
                }
            }
            if (rerouting && (obstacle == null || obstacle != LookForWalls(lineOfSight))) // HACK ???
            {
                rerouting = false;
            }

            // wanders around the field, preferring not to destroy any walls unless necessary
            // choose a random direction and try to travel in that direction
            rb.rotation = Mathf.LerpAngle(rb.rotation, intentAngle.Degree, 0.1f);
            rb.velocity = transform.right * moveSpeed;
        }
        private void PatrolBehaviorPrecheck()
        {
            // this runs before PatrolBehavior()
            // searches for nearby player

            GameObject footprint = null;
            Vector2 trackDirection = Vector2.zero;
            float trackDistance = 0f;
            RaycastHit2D los;

            footprint = SearchFootprint(ignoreList);
            if (footprint == null) return;
            trackDirection = new Vector2(footprint.transform.position.x - transform.position.x, footprint.transform.position.y - transform.position.y);
            trackDistance = Vector2.Distance(footprint.transform.position, transform.position);

            los = LineOfSight(trackDirection, trackDistance, invisWallLayer);

            if (los.collider == null)
            {
                ignoreList.Clear();
                target = footprint;
                behaviorState = EnemyState.Pursuit;
            }
            else
            {
                ignoreList.Add(footprint);
            }
        }
        private void PursuitBehavior()
        {
            if (behaviorState != EnemyState.Pursuit) { return; }
            ClearPath();
            if (target != null)
            {
                AllAngle trackedAngle = new()
                {
                    Vector = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y)
                };
                float trackedDistance = Vector2.Distance(transform.position, target.transform.position);
                RaycastHit2D lineOfSight = LineOfSight(trackedAngle.Vector, trackedDistance, invisWallLayer);

                if (lineOfSight.collider == null)
                {
                    rb.rotation = Mathf.LerpAngle(rb.rotation, trackedAngle.Degree, 0.1f);
                    rb.velocity = rb.transform.right * moveSpeed;

                    if (target.tag.Equals("Player") && trackedDistance <= FIRING_RANGE)
                    {
                        behaviorState = EnemyState.Attack;
                        return;
                    }
                    if (target.tag.Equals("Footprint") && trackedDistance <= VISION_RANGE)
                    {
                        target = target.GetComponent<Footprint>().NextFootprint.gameObject;
                        return;
                    }
                    return;
                }
            }
            GameObject potentialTarget = null;
            potentialTarget = SearchPlayer();
            if (potentialTarget == null) potentialTarget = SearchFootprint(potentialTarget);
            if (potentialTarget == null)
            {
                behaviorState = EnemyState.Patrol;
                return;
            }

            target = potentialTarget;
        }
        private void PursuitBehaviorPrecheck()
        {
            // this is called before PursuitBehavior() is run
            // check if the player is within firing range. If so, change to attacking mode
            GameObject tempTracked = SearchPlayer();
            if (tempTracked != null)
            {
                float tempTrackDistance = Vector2.Distance(transform.position, tempTracked.transform.position);
                Vector2 tempTrackDirection = new Vector2(tempTracked.transform.position.x - transform.position.x, tempTracked.transform.position.y - transform.position.y);
                RaycastHit2D tempRay = LineOfSight(tempTrackDirection, tempTrackDistance, invisWallLayer);

                if (tempRay.collider == null && tempTrackDistance <= FIRING_RANGE)
                {
                    target = tempTracked;
                    behaviorState = EnemyState.Attack;
                }
            }
        }
        private void AttackBehavior()
        {
            // we assume that the tracked object is the player
            AllAngle trackedAngle = new AllAngle();
            trackedAngle.Vector = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);
            float trackedDistance = Vector2.Distance(transform.position, target.transform.position);
            RaycastHit2D lineOfSight = LineOfSight(trackedAngle.Vector, trackedDistance, invisWallLayer);

            rb.rotation = Mathf.LerpAngle(rb.rotation, trackedAngle.Degree, 0.1f);
            rb.velocity = Vector2.zero;

            if (trackedDistance <= FIRING_RANGE && !lineOfSight)
            {
                // attack stuff
                Attack(target.transform.position);
            }
            else
            {
                target = null;
                fireTimer = 0f;
                behaviorState = EnemyState.Pursuit;
            }
        }
        private void Attack(Vector2 position)
        {
            if (fireTimer >= fireRate)
            {
                Fire(position);
            }
            base.Behavior();
        }
        /// <summary>
        /// finds and returns the closest footprint while avoiding those in the ignore list
        /// </summary>
        private GameObject SearchFootprint(List<GameObject> ignoreList)
        {
            // look for footprints
            Collider2D[] targetList = Physics2D.OverlapCircleAll(transform.position, searchRadius, footprintLayer);
            Footprint currentFP = null;

            foreach (Collider2D collider in targetList)
            {
                if (IsIgnored(ignoreList, collider.gameObject)) continue;

                if (currentFP == null) currentFP = collider.gameObject.GetComponent<Footprint>();

                // if the distance between this and the enemy is smaller, update the currentFP
                if (Vector2.Distance(transform.position, collider.gameObject.transform.position) < Vector2.Distance(transform.position, currentFP.gameObject.transform.position))
                {
                    currentFP = collider.gameObject.GetComponent<Footprint>();
                }
            }
            if (currentFP == null) return null;
            return currentFP.gameObject;
        }
        private GameObject SearchFootprint(GameObject currentTarget)
        {
            List<GameObject> list = new List<GameObject>
        {
            currentTarget
        };
            return SearchFootprint(list);
        }
        private bool IsIgnored(List<GameObject> list, GameObject obj)
        {
            if (list == null) return false;
            if (list.IndexOf(null) != -1) return false;
            foreach (GameObject thing in list)
            {
                if (thing.GetInstanceID() == obj.GetInstanceID()) return true;
            }
            return false;
        }
        private GameObject LookForWalls(RaycastHit2D raycastHit)
        {
            GameObject gameObj = null;

            if (!raycastHit) return null;
            gameObj = raycastHit.collider.gameObject;

            return gameObj;
        }
        /// <summary>
        /// returns a degree value
        /// </summary>
        /// <param name="normalVector"></param>
        /// <param name="rigidbodyVector"></param>
        /// <returns></returns>
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
            // destroy all walls around the enemy
            Collider2D[] walls = new Collider2D[12];

            walls = Physics2D.OverlapCircleAll(transform.position, 2.0f, wallLayer);
            foreach (Collider2D wall in walls)
            {
                wall.GetComponent<Wall>().Die();
            }
        }
        private void WanderState()
        {
            rb.velocity = travelDirection.Vector.normalized * moveSpeed;
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0)
            {
                travelDirection.Degree = Random.Range(0f, 360f);
                wanderTimer = Random.Range(WANDER_TIMER_MIN, WANDER_TIMER_MAX);
            }

            if (behaviorTimer > 0f) return; // if not ready to seek yet, don't do anything

            behaviorTimer = WANDER_TIMER_MIN;
            List<Collider2D> scan = new(Physics2D.OverlapCircleAll(transform.position, RADAR_RADIUS, playerLayer | footprintLayer));

            if (scan.Count != 0)
            {
                foreach (Collider2D col in scan)
                {
                    float distanceToTarget = Vector2.Distance(col.transform.position, transform.position);
                    Vector2 targetDirection = new(col.transform.position.x - transform.position.x, col.transform.position.y - transform.position.y);
                    RaycastHit2D ray = Physics2D.Raycast(transform.position, targetDirection, distanceToTarget, playerLayer | footprintLayer);

                    if (ray.collider == null) continue;

                    target = ray.collider.gameObject;
                    behaviorTimer = 0f;
                    behaviorState = EnemyState.Pursuit;
                    break;
                }
            }

            RaycastHit2D obstacles = Physics2D.CircleCast(transform.position, CIRCLE_CAST_RADIUS, travelDirection.Vector, wallsLayer); // search for walls in front of tracker

            if (obstacles.collider != null) CorrectPath(obstacles.normal, travelDirection.Vector);
        }
        private void ChaseState() // URGENT a little broken
        {
            travelDirection.Vector = target.transform.position - transform.position;
            rb.velocity = travelDirection.Vector.normalized * moveSpeed;

            if (behaviorTimer > 0f) return;

            behaviorTimer = CHASE_TIMER_MAX;
            ClearPath();
            List<Collider2D> playerTracks = new(Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer | footprintLayer));

            if (playerTracks.Count == 0)
            {
                behaviorTimer = 0f;
                behaviorState = EnemyState.Patrol;
                return;
            }

            float largestDistance = 0f;
            Transform farthestFootprint = null;

            foreach (Collider2D col in playerTracks)
            {
                float distanceToTarget = Vector2.Distance(transform.position, col.transform.position);

                if (!IsInLineOfSight(transform, col.transform, distanceToTarget, invisWallLayer)) continue;

                if (col.gameObject.CompareTag("Player"))
                {
                    target = col.gameObject;
                    behaviorTimer = 0f;
                    behaviorState = EnemyState.Attack;
                    return;
                }

                if (target == null && col.gameObject.CompareTag("Footprint")) // only update if not currently tracking a footprint; otherwise just follow footprints
                {
                    if (distanceToTarget > largestDistance)
                    {
                        farthestFootprint = col.transform;
                        largestDistance = distanceToTarget;
                    }
                }
            }

            if (farthestFootprint == null)
            {
                target = null;
                behaviorTimer = 0f;
                behaviorState = EnemyState.Patrol;
                return;
            }

            target = farthestFootprint.gameObject;
        }
        private void AttackState()
        {
            rb.velocity = Vector2.zero;

            if (behaviorTimer > 0f) return;

            behaviorTimer = fireRate;
            float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);

            if (!IsInLineOfSight(transform, target.transform, attackRange, invisWallLayer) || distanceToPlayer > attackRange)
            {
                behaviorTimer = 0f;
                behaviorState = EnemyState.Pursuit;
                return;
            }

            Fire(target.transform.position);
        }
        private bool IsInLineOfSight(Transform origin, Transform target, float maxDist, LayerMask obstructingLayers)
        {
            Vector2 dir = target.position - origin.position;
            return Physics2D.Raycast(origin.position, dir.normalized, maxDist, obstructingLayers).collider == null;
        }
    }
}
