using UnityEngine;
using UnityEngine.InputSystem;
using Flamenccio.Utility;
using Flamenccio.HUD;

namespace Flamenccio.Core.Player
{
    public class AimAssist : MonoBehaviour
    {
        public GameObject Target { get; private set; }
        [SerializeField][Tooltip("Only the enemy layer.")] private LayerMask enemyLayer;
        [SerializeField][Tooltip("All layers that may obstruct the bullet's path")] private LayerMask obstacleLayers;
        [SerializeField] private AimAssistTarget aimAssistTarget;
        private const float RAY_A_RADIUS = 1f / 6f;
        private float maxDist = 6.5f; // default
        private const float STARTING_OFFSET = 1f / 3f;
        private AllAngle angleRayB = new();
        private AllAngle angleRayC = new();
        private Vector2 aimAngle = Vector2.zero; // where player is aiming
        private Vector2 offset; // displacement of starting position (to avoid clipping with wall if player is too close)
        private void Start()
        {
            CalculateConeAngles();
        }
        private void Update()
        {
            if (aimAngle == Vector2.zero)
            {
                aimAssistTarget.Hide();
                return;
            }

            offset = aimAngle * STARTING_OFFSET;
            CastRays();

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
            RaycastHit2D ray = Physics2D.CircleCast(transform.position + (Vector3)offset, RAY_A_RADIUS, aimAngle, maxDist, obstacleLayers);

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
            RaycastHit2D ray = CastAuxRays(maxDist, transform.position, offset, aimAngle, angleRayB, obstacleLayers);

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
            RaycastHit2D ray = CastAuxRays(maxDist, transform.position, offset, aimAngle, angleRayC, obstacleLayers);

            if (ray.collider != null && IsInEnemyLayer(ray.collider.gameObject.layer)) Target = ray.collider.gameObject;
            else Target = null;
        }
        public void OnAim(InputAction.CallbackContext callbackContext)
        {
            aimAngle = callbackContext.ReadValue<Vector2>();
        }
        /// <summary>
        /// Casts two rays both offsetAngle degrees away from the originAngle.
        /// </summary>
        /// <returns>Closest gameObject, or NULL if there are no gameObjects.</returns>
        private RaycastHit2D CastAuxRays(float distance, Vector2 originPosition, Vector2 originOffset, Vector2 originAngle, AllAngle offsetAngle, LayerMask layers)
        {
            AllAngle offsetHigh = new(); // offset high
            AllAngle offsetLow = new(); // offset low 
            offsetHigh.Radian = Mathf.Atan2(originAngle.y, originAngle.x) + offsetAngle.Radian;
            offsetLow.Radian = Mathf.Atan2(originAngle.y, originAngle.x) - offsetAngle.Radian;

            RaycastHit2D ray1 = Physics2D.Raycast((Vector3)originPosition + (Vector3)originOffset, offsetHigh.Vector, distance, layers);
            RaycastHit2D ray2 = Physics2D.Raycast((Vector3)originPosition + (Vector3)originOffset, offsetLow.Vector, distance, layers);

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
