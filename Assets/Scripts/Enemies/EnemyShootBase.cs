using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Enemy
{
    public class EnemyShootBase : EnemyBase
    {
        [SerializeField] protected GameObject enemyBullet;
        [SerializeField] protected float searchRadius;
        [SerializeField] protected float fireRate;
        private AllAngle aaFireDirection = new();
        protected float fireTimer = 0f;
        private const float BULLET_OFFSET = 0.5f;
        protected float attackRange;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            attackRange = enemyBullet.GetComponent<BulletControl>().Range;
        }
        protected void Fire(Vector2 target)
        {
            float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
            Fire(angle);
        }
        protected void Fire(float direction)
        {
            animator.SetBool("attack", false); // disable attack telegraph
            aaFireDirection.Degree = direction; // convert angle from degrees to vector
            Vector3 offset = new(aaFireDirection.Vector.x * BULLET_OFFSET, aaFireDirection.Vector.y * BULLET_OFFSET); // calculate an offset so the bullet doesn't spawn inside the enemy
            Instantiate(enemyBullet, transform.position + offset, Quaternion.Euler(0f, 0f, direction));
        }
        protected override void Behavior()
        {
            if (fireTimer >= fireRate - ATTACK_TELEGRAPH_DURATION)
            {
                StartCoroutine(AttackTelegraph());
            }
            if (fireTimer >= fireRate)
            {
                fireTimer = 0f;
            }
            fireTimer += Time.deltaTime;
        }
        protected GameObject SearchPlayer()
        {
            Collider2D[] target = Physics2D.OverlapCircleAll(transform.position, searchRadius, playerLayer);

            if (target.Length < 0) return null;

            foreach (Collider2D collider in target)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    return collider.gameObject;
                }
            }

            return null;
        }
    }
}
