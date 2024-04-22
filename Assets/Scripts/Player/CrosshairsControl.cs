using UnityEngine.InputSystem;
using UnityEngine;
using Flamenccio.Core.Player;

namespace Flamenccio.HUD
{
    public class CrosshairsControl : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private PlayerActions playerActions;
        private SpriteRenderer spriteRen;

        private Vector2 aimDir;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite lockedonSprite;
        [SerializeField] private LayerMask destructableLayers; // objects that the bullet can destroy
        [SerializeField] private LayerMask obstacleLayers; // objects that the bullet can't destroy, but will be blocked by
        private Color visible = Color.white;
        private Color invisible = new Color(1f, 1f, 1f, 0f);
        private RaycastHit2D raycast1;
        private RaycastHit2D raycast2;
        private RaycastHit2D circleCast;
        private Vector2 offset;

        private float aimSmooth = 0.6f;
        private float circleCastRadius = 1f / 3f;

        private const float MAX_DISTANCE = 6.5f; // distance to keep from player
        private const float STARTING_OFFSET = 1f / 3f;
        private float newDistance = MAX_DISTANCE;
        private Vector2 raycastOffset1 = new(0f, 0.25f);
        private Vector2 raycastOffset2 = new(0f, -0.25f);

        private void Awake()
        {
            spriteRen = gameObject.GetComponent<SpriteRenderer>();
            spriteRen.sprite = inactiveSprite;
        }

        private void Update()
        {
            float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x); // calculate aim in degrees
            offset = aimDir * STARTING_OFFSET;
            circleCast = Physics2D.CircleCast(player.position + (Vector3)offset, circleCastRadius, aimDir, MAX_DISTANCE, destructableLayers | obstacleLayers); // cast a circle in the direction that the player is aiming

            if (circleCast && aimDir != Vector2.zero && newDistance <= MAX_DISTANCE) // if the circleCast exists AND the player is aiming AND the current distance is less than or equal to max distance:
            {
                if (IsInLayerMask(circleCast.collider.gameObject.layer, destructableLayers)) // if the targetted gameObject is destructable,
                {
                    if (circleCast.collider.gameObject.CompareTag("Enemy"))
                    {
                        spriteRen.sprite = lockedonSprite;
                    }
                    else
                    {
                        spriteRen.sprite = activeSprite; // activate sprite
                    }
                }
                else
                {
                    spriteRen.sprite = inactiveSprite;
                }

                float targetDist = Vector2.Distance(circleCast.point, player.position); // distance between target and player
                if (targetDist <= MAX_DISTANCE)
                {
                    newDistance = targetDist;
                }
            }
            else
            {
                // otherwise, revert back to the inactive sprite
                spriteRen.sprite = inactiveSprite;
                newDistance = MAX_DISTANCE;
            }

            if (aimDir != Vector2.zero)
            {
                // move the crosshairs to the direction that the player is facing
                transform.position = new Vector2(Mathf.LerpAngle(transform.position.x, player.position.x + aimDir.x * newDistance, aimSmooth), Mathf.LerpAngle(transform.position.y, player.position.y + aimDir.y * newDistance, aimSmooth));
                spriteRen.color = visible;
            }
            else
            {
                spriteRen.color = invisible;
                transform.position = player.transform.position;
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            aimDir = context.ReadValue<Vector2>();
        }
        private bool IsInLayerMask(int layer, LayerMask mask) // checks if layer integer value is in a layerMask
        {
            int temp = (1 << layer); // convert the layer integer to a bit map
            if (mask == (temp | mask)) return true; // if the result of (temp | mask) is the same as mask, then we can say that layer is in mask.
            return false;
        }
    }
}
