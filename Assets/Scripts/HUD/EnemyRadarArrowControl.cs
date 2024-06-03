using Flamenccio.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls the behavior of a HUD element that shows incoming bullets.
    /// </summary>
    public class EnemyRadarArrowControl : MonoBehaviour
    {
        /// <summary>
        /// The current Transform this arrow is tracking.
        /// </summary>
        public Transform Target { get; set; }
        public bool Ready
        {
            get => ready;
            set
            {
                if (!ready) ready = value;
            }
        }
        private Transform player;
        [SerializeField] private Image thisImage;

        // the width and height of the elliptical path
        private float PATH_HALF_WIDTH;
        private float PATH_HALF_HEIGHT;

        private float maxYDistance;
        private float minYDistance;

        private float maxXDistance;
        private float minXDistance;

        private bool ready = false;

        private void Awake()
        {
            player = PlayerMotion.Instance.PlayerTransform;
            PATH_HALF_HEIGHT = (Camera.main.pixelHeight) / 4f;
            PATH_HALF_WIDTH = (Camera.main.pixelWidth) / 4f;
        }

        private void Update()
        {
            if (!Ready) return;

            if (Target == null)
            {
                Destroy(gameObject);
                return; // return is absolutely redundant, but somehow fixes bugs (???)
            }

            Vector2 targetPosition = Target.position;
            float distance = Vector2.Distance(targetPosition, player.position);

            if (distance > maxXDistance)
            {
                Destroy(gameObject);
                return; // return is redundant
            }

            transform.SetLocalPositionAndRotation(GetPositionOnPath(targetPosition), Quaternion.Euler(0f, 0f, GetRotationDegrees(targetPosition))); // update transform
            UpdateSprite(targetPosition, player.position);
        }

        private Vector2 GetPositionOnPath(float radian)
        {
            return new(PATH_HALF_WIDTH * Mathf.Cos(radian), PATH_HALF_HEIGHT * Mathf.Sin(radian));
        }

        private Vector2 GetPositionOnPath(Vector2 target)
        {
            Vector2 targetOffset = target - (Vector2)Camera.main.transform.position;
            float angleRadians = Mathf.Atan2(targetOffset.y, targetOffset.x);

            return GetPositionOnPath(angleRadians);
        }

        private float GetRotationRadians(Vector2 target)
        {
            Vector2 targetOffset = target - (Vector2)Camera.main.transform.position;

            return Mathf.Atan2(targetOffset.y, targetOffset.x);
        }

        private float GetRotationDegrees(Vector2 target)
        {
            return Mathf.Rad2Deg * GetRotationRadians(target);
        }

        private void UpdateSprite(Vector2 target, Vector2 camera)
        {
            Vector2 difference = target - camera;
            difference = new(Mathf.Abs(difference.x), Mathf.Abs(difference.y));

            if (difference.x > difference.y) // use the larger difference to calculate opacity
            {
                UpdateSprite(difference.x, minXDistance, maxXDistance);
            }
            else
            {
                UpdateSprite(difference.y, minYDistance, maxYDistance);
            }
        }

        private void UpdateSprite(float distance, float min, float max)
        {
            if (distance < min)
            {
                thisImage.color = new(thisImage.color.r, thisImage.color.g, thisImage.color.b, 0f); // make completely transparent
                return;
            }

            float opacitySlope = -1f / (max - min);
            float opacity = Mathf.Clamp01((opacitySlope * distance) - (opacitySlope * max));
            thisImage.color = new(thisImage.color.r, thisImage.color.g, thisImage.color.b, opacity);
        }

        /// <summary>
        /// Update the distances between the player and enemy where this arrow will be visible.
        /// <para>Note that all minimums MUST be smaller than maximums, or this method will reject all inputs.</para>
        /// </summary>
        /// <param name="minX">Minimum distance on x-axis.</param>
        /// <param name="maxX">Maximum distance on x-axis.</param>
        /// <param name="minY">Minimum distance on y-axis.</param>
        /// <param name="maxY">Maximum distance on y-axis.</param>
        public void SetRange(float minX, float maxX, float minY, float maxY)
        {
            if (minX >= maxX || minY >= maxY) return;

            minXDistance = minX;
            maxXDistance = maxX;
            minYDistance = minY;
            maxYDistance = maxY;
        }
    }
}
