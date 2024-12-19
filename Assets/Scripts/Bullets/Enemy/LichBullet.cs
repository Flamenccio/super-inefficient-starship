using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.Attack.Enemy
{
    /// <summary>
    /// Controls an enemy bullet. Continuously spawns phantom bullets to the left and right (relative to its direction of travel).
    /// </summary>
    public class LichBullet : EnemyBulletBase
    {
        [SerializeField] private GameObject phantomBullet;

        private int direction = 1;
        private const float SPAWN_FREQUENCY = 20f / 60f;
        private float spawnTimer = 0.0f;

        protected override void Behavior()
        {
            if (spawnTimer >= SPAWN_FREQUENCY)
            {
                AllAngle fireAngle = new()
                {
                    Vector = Vector2.Perpendicular(rb.linearVelocity * direction)
                };
                spawnTimer = 0.0f;
                Instantiate(phantomBullet, transform.position, Quaternion.Euler(0f, 0f, fireAngle.Degree));
                direction *= -1;
            }

            spawnTimer += Time.deltaTime;
        }
    }
}
