using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Enemy
{
    public class StaticTurretBehavior : EnemyBehavior
    {
        [SerializeField] private float fireRate = 1.0f;
        [SerializeField] private GameObject bullet;
        [SerializeField] private List<float> attackAnglesDegrees = new();
        private float attackTimer = 0f;
        private bool telegraphed = false;

        protected override void Behavior()
        {
            attackTimer += Time.deltaTime;

            if (!telegraphed && attackTimer >= fireRate - ATTACK_TELEGRAPH_DURATION)
            {
                telegraphed = true;
                AttackTelegraph?.Invoke();
            }
            if (attackTimer >= fireRate)
            {
                telegraphed = false;
                attackTimer = 0f;

                foreach (var angle in attackAnglesDegrees)
                {
                    Fire(angle);
                }
            }
        }

        private void Fire(float angle)
        {
            Instantiate(bullet, transform.position, Quaternion.Euler(0f, 0f, angle));
        }
    }
}
