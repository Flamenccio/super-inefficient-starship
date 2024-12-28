using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility.Timer;

namespace Flamenccio.Enemy
{
    public class StaticTurretBehavior : EnemyBehavior
    {
        [SerializeField] private float fireRate = 1.0f;
        [SerializeField] private GameObject bullet;
        [SerializeField] private List<float> attackAnglesDegrees = new();
        private EventTimer attackEventTimer;

        private void Start()
        {
            // Set up attack timer
            attackEventTimer = new(fireRate, true);
            attackEventTimer.AddLapListener(AttackAtAngles);
            attackEventTimer.AddOffsetListener(() => AttackTelegraph?.Invoke(), ATTACK_TELEGRAPH_DURATION, EventTimer.OffsetListener.OffsetReferencePoint.FromEnd);
        }

        public override void Disable()
        {
            base.Disable();
            
            // Pause attack timer when player goes out of range
            attackEventTimer.PauseTimer();
        }

        public override void Enable()
        {
            base.Enable();
            
            // Resume attack timer when player enters range
            attackEventTimer.StartTimer();
        }

        public override void OnDeath()
        {
            if (!attributes.Alive) return;
            
            attackEventTimer.Destroy();
            base.OnDeath();
        }

        /// <summary>
        /// Fires bullets at all angles defined in attackAngleDegrees
        /// </summary>
        private void AttackAtAngles()
        {
            foreach (var angle in attackAnglesDegrees)
            {
                Fire(angle);
            }
        }

        private void Fire(float angle)
        {
            Instantiate(bullet, transform.position, Quaternion.Euler(0f, 0f, angle));
        }
    }
}
