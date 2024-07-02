using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    public class PlayerMissile : PlayerBullet
    {
        [SerializeField] private float searchRadius;
        [SerializeField] private float explosionRadius;
        [SerializeField] private string explosionVfx;
        [SerializeField] private GameObject explosionHitbox;
        private Transform target;
        private const float ACCELERATION = 1f;
        private float turningSpeed = 0.1f;

        protected override void Startup()
        {
            base.Startup();

            collisionTags.Remove(TagManager.GetTag(Tag.Wall));
            collisionTags.Remove(TagManager.GetTag(Tag.NeutralBullet));
        }
        protected override void Behavior()
        {
            moveSpeed += ACCELERATION;
            if (target != null)
            {
                canIgnoreStageEdge = true; // When the missile is locked on to an enemy, it can ignore stage edges.
                FlyToTarget();
            }
            else
            {
                canIgnoreStageEdge = false; // Otherwise, the missile will explode on contact.
            }
        }

        /// <summary>
        /// Set this missile's target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void SetTarget(Transform target)
        {
            if (this.target == null)
            {
                this.target = target;
            }
        }

        private void FlyToTarget()
        {
            AllAngle angle = new()
            {
                Vector = target.position - transform.position,
            };
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle.Degree, turningSpeed);
            rb.velocity = transform.right * moveSpeed;
            turningSpeed = Mathf.Clamp01(turningSpeed + Time.deltaTime);
        }

        protected override void Death()
        {
            Instantiate(explosionHitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>()
                .EditProperties(0f, explosionRadius, playerDamage, Hitbox.HitboxAffiliation.Player);
            EffectManager.Instance.SpawnEffect(explosionVfx, transform.position);
            base.Death();
        }

    }
}
