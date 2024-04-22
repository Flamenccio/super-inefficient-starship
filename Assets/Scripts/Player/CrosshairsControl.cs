using UnityEngine.InputSystem;
using UnityEngine;
using Flamenccio.Core.Player;

namespace Flamenccio.HUD
{
    public class CrosshairsControl : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite lockedonSprite;
        [SerializeField] private LayerMask destructableLayers; // objects that the bullet can destroy
        [SerializeField] private LayerMask obstacleLayers; // objects that the bullet can't destroy, but will be blocked by
        private SpriteRenderer spriteRen;
        private Vector2 aimDir;
        private Color visible = Color.white;
        private Color invisible = new(1f, 1f, 1f, 0f);
        private float maxDistance = 6.5f; // distance to keep from player (default distance)
        private float newDistance;
        private const float AIM_SMOOTHING = 0.6f;
        private const float CIRCLE_CAST_RADIUS = 1f / 3f;
        private const float STARTING_OFFSET = 1f / 3f;

        private void Awake()
        {
            newDistance = maxDistance;
            spriteRen = gameObject.GetComponent<SpriteRenderer>();
            spriteRen.sprite = inactiveSprite;
        }
        private void Update()
        {
            Vector2 offset = aimDir * STARTING_OFFSET;
            RaycastHit2D circleCast = Physics2D.CircleCast(player.position + (Vector3)offset, CIRCLE_CAST_RADIUS, aimDir, maxDistance, destructableLayers | obstacleLayers); // cast a circle in the direction that the player is aiming

            if (circleCast && aimDir != Vector2.zero && newDistance <= maxDistance) // if the circleCast exists AND the player is aiming AND the current distance is less than or equal to max distance:
            {
                if (IsInLayerMask(circleCast.collider.gameObject.layer, destructableLayers)) // if the targetted gameObject is destructable,
                {
                    if (circleCast.collider.gameObject.CompareTag("Enemy")) spriteRen.sprite = lockedonSprite;
                    else spriteRen.sprite = activeSprite; // activate sprite
                }
                else
                {
                    spriteRen.sprite = inactiveSprite;
                }

                float targetDist = Vector2.Distance(circleCast.point, player.position); // distance between target and player
                if (targetDist <= maxDistance) newDistance = targetDist;
            }
            else
            {
                // otherwise, revert back to the inactive sprite
                spriteRen.sprite = inactiveSprite;
                newDistance = maxDistance;
            }

            if (aimDir != Vector2.zero)
            {
                // move the crosshairs to the direction that the player is facing
                float newXPosition = Mathf.LerpAngle(transform.position.x, player.position.x + (aimDir.x * newDistance), AIM_SMOOTHING);
                float newYPosition = Mathf.LerpAngle(transform.position.y, player.position.y + (aimDir.y * newDistance), AIM_SMOOTHING);
                transform.position = new Vector2(newXPosition, newYPosition);
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
        public void UpdateWeaponRange(float range)
        {
            maxDistance = range;
        }
    }
}
