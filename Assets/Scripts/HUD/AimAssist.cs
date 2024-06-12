using Flamenccio.HUD;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Core.Player
{
    /// <summary>
    /// This class searches for enemies nearby the player's aim.
    /// <para>It uses an algorithm to find the best target to attack, and sets it as a TARGET, which other classes can reference.</para>
    /// </summary>
    public class AimAssist : MonoBehaviour
    {
        /// <summary>
        /// The current target found by aim assist.
        /// </summary>
        public GameObject Target { get; private set; }

        [SerializeField][Tooltip("Only the enemy layer.")] private LayerMask enemyLayer;
        [SerializeField][Tooltip("All layers that may obstruct the bullet's path")] private LayerMask obstacleLayers;
        [SerializeField] private AimAssistTarget aimAssistTarget;

        private float maxDist = 6.5f; // default
        private AllAngle angleRayB = new();
        private AllAngle angleRayC = new();
        private Vector2 offset; // displacement of starting position (to avoid clipping with wall if player is too close)
        private InputManager input;

        private const float RAY_A_RADIUS = 1f / 6f;
        private const float STARTING_OFFSET = 1f / 3f;

        private void Start()
        {
            input = InputManager.Instance;
            CalculateConeAngles();
        }

        private void Update()
        {
            if (input.AimInputVector == Vector2.zero)
            {
                aimAssistTarget.Hide();
                return;
            }

            offset = input.AimInputVector * STARTING_OFFSET;
            CastRays();
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (Target != null)
            {
                aimAssistTarget.Show(Target.transform);
            }
            else
            {
                aimAssistTarget.Hide();
            }
        }

        private void CastRays()
        {
            CastRayA();
        }

        private void CastRayA()
        {
            RaycastHit2D ray = Physics2D.CircleCast(transform.position + (Vector3)offset, RAY_A_RADIUS, input.AimInputVector, maxDist, obstacleLayers);

            if (ray.collider != null && IsInEnemyLayer(ray.collider.gameObject.layer))
            {
                Target = ray.collider.gameObject;
            }
            else
            {
                CastRayB();
            }
        }

        private void CastRayB()
        {
            RaycastHit2D ray = CastAuxRays(maxDist, transform.position, offset, input.AimInputVector, angleRayB, obstacleLayers);

            if (ray.collider != null && IsInEnemyLayer(ray.collider.gameObject.layer))
            {
                Target = ray.collider.gameObject;
            }
            else
            {
                CastRayC();
            }
        }

        private void CastRayC()
        {
            RaycastHit2D ray = CastAuxRays(maxDist, transform.position, offset, input.AimInputVector, angleRayC, obstacleLayers);

            if (ray.collider != null && IsInEnemyLayer(ray.collider.gameObject.layer)) Target = ray.collider.gameObject;
            else Target = null;
        }

        /// <summary>
        /// Casts two rays both offsetAngle degrees away from the originAngle.
        /// </summary>
        /// <returns>Closest gameObject, or NULL if there are no gameObjects.</returns>
        private RaycastHit2D CastAuxRays(float distance, Vector2 originPosition, Vector2 originOffset, Vector2 originAngle, AllAngle offsetAngle, LayerMask layers)
        {
            float rad = Mathf.Atan2(originAngle.y, originAngle.x);
            AllAngle offsetHigh = new()
            {
                Radian = rad + offsetAngle.Radian
            };
            AllAngle offsetLow = new()
            {
                Radian = rad - offsetAngle.Radian
            };
            Vector3 finalOrigin = originPosition + originOffset;
            RaycastHit2D ray1 = Physics2D.Raycast(finalOrigin, offsetHigh.Vector, distance, layers);
            RaycastHit2D ray2 = Physics2D.Raycast(finalOrigin, offsetLow.Vector, distance, layers);

            if (ray1.collider != null || ray2.collider != null) // if either exist,
            {
                if (ray1.collider != null && ray2.collider != null) // if both exist
                {
                    float dist = Vector2.Distance(originPosition, ray1.point) - Vector2.Distance(originPosition, ray2.point); // compare distance of player and both B rays

                    return (dist < 0) ? ray1 : ray2; // if dist < 0, then that means rayB1 is closer: return that. Otherwise, dist >= 0, which means rayB2 is closer: return that.
                }
                else
                {
                    return (ray1.collider != null) ? ray1 : ray2; // otherwise return the one that's not null
                }
            }

            return ray1;
        }

        public void UpdateWeaponRange(float range)
        {
            maxDist = range;
            CalculateConeAngles();
        }

        private void CalculateConeAngles()
        {
            angleRayB.Degree = 4f * Mathf.Log(maxDist);
            angleRayC.Degree = 5f / 3f * angleRayB.Degree;
        }

        private bool IsInLayerMask(int layer, LayerMask mask) // checks if layer integer value is in a layerMask
        {
            int temp = (1 << layer); // convert the layer integer to a bit map
            return mask == (temp | mask); // if the result of (temp | mask) is the same as mask, then we can say that layer is in mask.
        }

        private bool IsInEnemyLayer(int layer)
        {
            return IsInLayerMask(layer, enemyLayer);
        }
    }
}