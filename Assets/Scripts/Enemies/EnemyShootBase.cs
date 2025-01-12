using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Utility;

namespace Enemy
{
    /// <summary>
    /// Base class for all enemies that fire a projectile.
    /// </summary>
    public class EnemyShootBase : EnemyBase
    {
        [SerializeField] protected GameObject enemyBullet;
        [SerializeField] protected float searchRadius;
        [SerializeField] protected float fireRate;
        protected float fireTimer = 0f;
        protected float attackRange;
        private AllAngle aaFireDirection = new();
        private const float BULLET_OFFSET = 0.5f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            attackRange = enemyBullet.GetComponent<BulletControl>().Range;
        }

        /// <summary>
        /// Fire a projectile at a target coordinate.
        /// </summary>
        protected void Fire(Vector2 target)
        {
            float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
            Fire(angle);
        }

        /// <summary>
        /// Fire a projectile at some angle.
        /// </summary>
        protected void Fire(float direction)
        {
            animator.SetBool("attack", false); // disable attack telegraph
            aaFireDirection.Degree = direction; // convert angle from degrees to vector
            Vector3 offset = new(aaFireDirection.Vector.x * BULLET_OFFSET, aaFireDirection.Vector.y * BULLET_OFFSET); // calculate an offset so the bullet doesn't spawn inside the enemy
            Instantiate(enemyBullet, transform.position + offset, Quaternion.Euler(0f, 0f, direction));
        }

        protected override void Behavior()
        {
            if (!telegraphed && fireTimer >= fireRate - ATTACK_TELEGRAPH_DURATION)
            {
                telegraphed = true;
                StartCoroutine(AttackTelegraph());
            }
            if (fireTimer >= fireRate)
            {
                telegraphed = false;
                fireTimer = 0f;
            }

            fireTimer += Time.deltaTime;
        }
    }
}