using System.Collections;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls a game object that shows the player where the next star is located relative to them.
    /// </summary>
    public class GoalArrowControl : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRen;
        [SerializeField] private GameObject player;
        [SerializeField] private Camera cam;
        private const float moveSpeed = 0.1f;
        private Vector2 targetPosition = Vector2.zero;
        private const float NEAR_LIMIT = 5f; // the distance between the player and star where the goal arrow is completely transparent
        private const float FAR_LIMIT = 15f; // the minimum distance that the player must be in order for the arrow to be at full opacity
        private AllAngle pointAngle = new();

        private void Awake()
        {
            Hide();
        }

        public void PointAt(Vector3 pos)
        {
            targetPosition = pos;
        }

        private void FixedUpdate()
        {
            float distance = cam.orthographicSize - 3.0f;

            // calculate the distance between the target and player
            float playerTargetDistance = Vector2.Distance(targetPosition, player.transform.position);

            ChangeTransparency(playerTargetDistance);

            pointAngle.Vector = new Vector2(targetPosition.x - player.transform.position.x, targetPosition.y - player.transform.position.y);

            // now set "newPosition" "distance" units away from the player in the direction of the target
            Vector2 newPosition = new(player.transform.position.x + (pointAngle.Vector.normalized.x * distance), player.transform.position.y + (pointAngle.Vector.normalized.y * distance));

            // smoothly move from the player's position to "newPosition" defined by the PointAt method
            var finalPosition = new Vector2(Mathf.Lerp(transform.position.x, newPosition.x, moveSpeed), Mathf.Lerp(transform.position.y, newPosition.y, moveSpeed));

            transform.SetPositionAndRotation(finalPosition, Quaternion.Euler(0f, 0f, pointAngle.Degree));
        }

        public void PointAt(GameObject pos)
        {
            PointAt(pos.transform.position);
        }

        public void Hide()
        {
            spriteRen.color = new Color(255f, 255f, 255f, 0f);
            transform.position = player.transform.position;
        }

        public void Display()
        {
            spriteRen.color = Color.white;
        }

        private void ChangeTransparency(float distance)
        {
            float scaledDistance = (distance - NEAR_LIMIT) / (FAR_LIMIT - NEAR_LIMIT);

            if (distance < NEAR_LIMIT)
            {
                Hide();
                return;
            }

            if (distance > FAR_LIMIT)
            {
                Display();
            }

            if (distance <= FAR_LIMIT && distance >= NEAR_LIMIT)
            {
                spriteRen.color = new Color(1f, 1f, 1f, scaledDistance);
            }
        }
    }
}