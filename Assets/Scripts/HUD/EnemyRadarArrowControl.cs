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
        /*
        /// <summary>
        /// The current Transform this arrow is tracking.
        /// </summary>
        public Transform Target { get; set; }
        public float MaxDistanceFromEllipse { get; set; }
        
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

        private bool ready = false;
        private Camera thisCamera;

        private void Awake()
        {
            player = PlayerMotion.Instance.PlayerTransform;
            thisCamera = Camera.main;
            PATH_HALF_HEIGHT = thisCamera.pixelHeight / 4f;
            PATH_HALF_WIDTH = thisCamera.pixelWidth / 4f;
        }

        private void Update()
        {
            if (!Ready) return;

            if (!Target)
            {
                Destroy(gameObject);
                return;
            }

            var targetPosition = Target.position;

            var translatedTargetPosition = targetPosition -
               thisCamera.transform.position;

            var positionOnEllipse = GetPositionOnPath
                (translatedTargetPosition);
            
            var distanceFromEllipse = Vector2.Distance(
                positionOnEllipse, translatedTargetPosition);

            var distanceFromCenterToEllipsePoint = Vector2.Distance
                (Vector2.zero, positionOnEllipse);
            
            var distanceFromCenterToTarget = Vector2.Distance
                (Vector2.zero, thisCamera.WorldToScreenPoint(targetPosition));
            
            var insideEllipse = distanceFromCenterToEllipsePoint 
                >= distanceFromCenterToTarget;
            
            Debug.Log("1: " + distanceFromCenterToEllipsePoint);
            Debug.Log("2: " + distanceFromCenterToTarget);
            
            // When the distanceFromCenterToTarget is smaller than
            // the distanceFromCenterToEllipsePoint, the
            // translatedTargetPosition is inside the ellipse
            if (insideEllipse)
            {
                UpdateSprite(MaxDistanceFromEllipse);
            }
            else
            {
                UpdateSprite(distanceFromEllipse);
            }
            
            // If the distance from center to ellipse point is
            // larger than the distance from ellipse point to target,
            // "clamp" the distanceFromEllipse to 0
            if (!insideEllipse && distanceFromEllipse >= 
                MaxDistanceFromEllipse)
            {
                Destroy(gameObject);
                return;
            }
            
            // Update transform
            transform.SetLocalPositionAndRotation(
                positionOnEllipse, 
                Quaternion.Euler(
                    0f, 0f, GetRotationDegrees(targetPosition)));
        }

        private Vector2 GetPositionOnPath(Vector2 target)
        {
            return new Vector2(GetPositionOnEllipse(target.x, 
                target), GetPositionOnEllipse(target.y, target));
        }

        private float GetPositionOnEllipse(float component,
            Vector2 target)
        {
            return component * (PATH_HALF_WIDTH * PATH_HALF_HEIGHT) /
                Mathf.Sqrt(
                    Mathf.Pow(PATH_HALF_WIDTH, 2f) * Mathf.Pow(target
                    .y, 2f) +
                    Mathf.Pow(PATH_HALF_HEIGHT, 2f) * Mathf
                    .Pow(target.x, 2f)
                    );
        }

        private float GetRotationRadians(Vector2 target)
        {
            var targetOffset = target - (Vector2)thisCamera
                .transform.position;

            return Mathf.Atan2(targetOffset.y, targetOffset.x);
        }

        private float GetRotationDegrees(Vector2 target)
        {
            return Mathf.Rad2Deg * GetRotationRadians(target);
        }

        private void UpdateSprite(float distance)
        {
            var opacitySlope = -1f / MaxDistanceFromEllipse;
            var opacity = Mathf.Clamp01(opacitySlope * distance + 1);
            thisImage.color = new Color(
                thisImage.color.r, 
                thisImage.color.g, 
                thisImage.color.b, 
                opacity);
        }
        */
    }
}
