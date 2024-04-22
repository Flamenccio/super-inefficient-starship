using UnityEngine;
using UnityEngine.InputSystem;
using Flamenccio.Utility;
using Flamenccio.HUD;

namespace Flamenccio.Core.Player
{
    public class AimAssist : MonoBehaviour
    {
        // make sure to dynamically change these values
        private float circleCastRadius = 1f / 6f;
        private float maxDist = 6.5f;
        private const float STARTING_OFFSET = 1f / 3f;
        private AllAngle angleRayB = new();
        private AllAngle angleRayC = new();

        public GameObject Target { get; private set; }
        private Vector2 aimAngle = Vector2.zero; // where player is aiming
        private Vector2 offset; // displacement of starting position (to avoid clipping with wall if player is too close)
        private RaycastHit2D ray; // raycast

        public static AimAssist instance;

        [SerializeField][Tooltip("Only the enemy layer.")] private LayerMask enemyLayer;
        [SerializeField][Tooltip("All layers that may obstruct the bullet's path")] private LayerMask obstacleLayers;
        private void Awake()
        {
            // URGENT this class does not need to be a singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            CalculateConeAngles();
        }
        private void Update()
        {
            // TODO increase readability
            if (aimAngle == Vector2.zero)
            {
                AimAssistTarget.instance.Hide();
                return;
            }

            offset = aimAngle * STARTING_OFFSET;
            ray = Physics2D.CircleCast(transform.position + (Vector3)offset, circleCastRadius, aimAngle, maxDist, obstacleLayers); // cast ray
            /*
             * The jist of the rest of the Update() method is this:
             * Cast a circle cast. If it hits an enemy object, set Target to that object. Otherwise:
             * Cast two raycasts like a cone. If it hits an enemy object, set Target to that object. Otherwise:
             * Cast another two raycasts, this time a little further apart. If it hits an enemy object, set Target to that object. Otherwise, Target is null.
             * 
             * Then after all that, if Target isn't null lockon to the enemy object.
             * Otherwise, do not lockon.
             */
            if (ray.collider != null && IsInLayerMask(ray.collider.gameObject.layer, enemyLayer))
                Target = ray.collider.gameObject;
            else
            {
                ray = CastAuxRays(maxDist, transform.position, offset, aimAngle, angleRayB, obstacleLayers);
                if (ray.collider != null && IsInLayerMask(ray.collider.gameObject.layer, enemyLayer))
                    Target = ray.collider.gameObject;
                else
                {
                    ray = CastAuxRays(maxDist, transform.position, offset, aimAngle, angleRayC, obstacleLayers);
                    if (ray.collider != null && IsInLayerMask(ray.collider.gameObject.layer, enemyLayer))
                        Target = ray.collider.gameObject;
                    else
                        Target = null;
                }
            }

            if (Target != null)
            {
                AimAssistTarget.instance.Show(Target.transform);
            }
            else
            {
                AimAssistTarget.instance.Hide();
            }
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
            AllAngle offsetH = new(); // offset high
            AllAngle offsetL = new(); // offset low 
            offsetH.Radian = Mathf.Atan2(originAngle.y, originAngle.x) + offsetAngle.Radian; // calculate the "high" angle (the angle pointing right of the player)
            offsetL.Radian = Mathf.Atan2(originAngle.y, originAngle.x) - offsetAngle.Radian; // calculate the "low" angle (the angle pointing left of the player)

            RaycastHit2D ray1 = Physics2D.Raycast((Vector3)originPosition + (Vector3)originOffset, offsetH.Vector, distance, layers); // cast leftmost ray
            RaycastHit2D ray2 = Physics2D.Raycast((Vector3)originPosition + (Vector3)originOffset, offsetL.Vector, distance, layers); // cast rightmost ray

            if (ray1.collider != null || ray2.collider != null) // if either exist,
            {
                if (ray1.collider != null && ray2.collider != null) // if both exist
                {
                    float dist = Vector2.Distance(originPosition, ray1.point) - Vector2.Distance(originPosition, ray2.point); // compare distance of player and both B rays
                    return (dist < 0) ? ray1 : ray2; // if dist < 0, then that means rayB1 is closer: return that. Otherwise, dist >= 0, which means rayB2 is closer: return that.
                }
                else return (ray1.collider != null) ? ray1 : ray2; // otherwise return the one that's not null
            }
            return ray1; // just return the first ray
        }
        private void CalculateConeAngles()
        {
            angleRayB.Degree = 4f * Mathf.Log(maxDist);
            angleRayC.Degree = 5f / 3f * angleRayB.Degree;
        }
        private bool IsInLayerMask(int layer, LayerMask mask) // checks if layer integer value is in a layerMask
        {
            int temp = (1 << layer); // convert the layer integer to a bit map
            return (mask == (temp | mask)); // if the result of (temp | mask) is the same as mask, then we can say that layer is in mask.
        }
    }
}
