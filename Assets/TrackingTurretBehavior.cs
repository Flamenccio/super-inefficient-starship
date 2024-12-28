using UnityEngine;
using Flamenccio.Utility.Timer;
using Flamenccio.Effects;

namespace Flamenccio.Enemy
{
    public class TrackingTurretBehavior : EnemyBehavior
    {
        [SerializeField] private float fireRate = 1.0f;
        [SerializeField] private float range = 1.0f;
        [SerializeField] private GameObject bullet;
        
        private Transform player;
        private EventTimer attackEventTimer;

        private void Start()
        {
            // Set up attack timer
            attackEventTimer = new(fireRate, true);
            attackEventTimer.AddLapListener(Attack);
            attackEventTimer.AddOffsetListener(() => AttackTelegraph?.Invoke
                (), ATTACK_TELEGRAPH_DURATION, EventTimer.OffsetListener.OffsetReferencePoint.FromEnd);
            
            // Get player transform
            player = PlayerMotion.Instance.PlayerTransform;
        }
        
        protected override void Behavior()
        {
            CheckPlayerDistance();
            AimAtPlayer();
        }

        public override void OnDeath()
        {
            if (!attributes.Alive) return;
            
            base.OnDeath();
            attackEventTimer.Destroy();
        }

        private void AimAtPlayer()
        {
            // Face the player
            var positionDifference = player.position - transform.position;
            var faceAngle = Mathf.Atan2(positionDifference.y, 
                positionDifference.x) * Mathf
                .Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, faceAngle, 0.08f)));
            base.Behavior();
        }

        private void CheckPlayerDistance()
        {
            var playerDistance = Vector3.Distance(player.position, transform.position);

            if (attackEventTimer.Paused && playerDistance < range)
            {
                attackEventTimer.StartTimer();
            }
            else if (!attackEventTimer.Paused && playerDistance >= range)
            {
                attackEventTimer.StopTimer();
            }
        }

        protected virtual void Attack()
        {
            Fire(player.position);
        }

        private void Fire(Vector2 position)
        {
            var positionDifference = position - (Vector2)transform.position;
            var fireAngle = Mathf.Atan2(positionDifference.y, positionDifference.x) * Mathf.Rad2Deg;
            Instantiate(bullet, transform.position, Quaternion.Euler(0f, 0f, 
                fireAngle));
        }
    }
}
