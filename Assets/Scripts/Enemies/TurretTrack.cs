using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Controls an enemy. Aims at player and fires normal enemy bullets continuously. Does not move.
    /// </summary>
    public class TurretTrack : EnemyShootBase, IEnemy
    {
        public int Tier { get => tier; }

        protected float faceAngle = 0.0f;
        protected float yDiff = 0.0f;
        protected float xDiff = 0.0f;

        protected override void Behavior()
        {
            if (fireTimer >= fireRate) // attack periodically
            {
                Attack();
            }

            AimAtPlayer();
        }

        private void AimAtPlayer()
        {
            if (player != null)
            {
                // face the player
                yDiff = player.position.y - transform.position.y;
                xDiff = player.position.x - transform.position.x;
                faceAngle = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, faceAngle, 0.08f)));
                base.Behavior();
            }
        }

        protected virtual void Attack()
        {
            if (player == null) return;
            if (Vector2.Distance(player.position, transform.position) < searchRadius)
            {
                Fire(player.position);
            }
        }
    }
}