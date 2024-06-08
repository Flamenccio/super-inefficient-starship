using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.HUD
{
    public class EnemyRadarControl : MonoBehaviour
    {
        [SerializeField] private DynamicHudControl dynamicHud;
        private CircleCollider2D circleCollider;
        private const float MIN_RADAR_RADIUS = 1.0f;
        private const float MAX_RADAR_RADIUS = 100.0f;
        private const float RADAR_RADIUS_SIZE = 10f; // value to add to camera's width to get the radar's radius

        private void Awake()
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(TagManager.GetTag(Tag.Enemy)))
            {
                CreateNewArrow(collision.transform);
            }
        }

        private void Update()
        {
            float height = Camera.main.orthographicSize;
            float newRadius = (height * Camera.main.aspect) + RADAR_RADIUS_SIZE;
            SetRadarRadius(newRadius);
        }

        private void CreateNewArrow(Transform bullet)
        {
            float minY = Camera.main.orthographicSize;
            float minX = minY * Camera.main.aspect;
            dynamicHud.DisplayBulletRadarArrow(bullet, minX, minY, circleCollider.radius);
        }

        private void SetRadarRadius(float radius)
        {
            circleCollider.radius = Mathf.Clamp(radius, MIN_RADAR_RADIUS, MAX_RADAR_RADIUS);
        }
    }
}
