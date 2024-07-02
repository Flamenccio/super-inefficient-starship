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
        private const float ACCELERATION = 0.5f;

        protected override void Behavior()
        {
            moveSpeed += ACCELERATION;
            if (target != null)
            {
                FlyToTarget();
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
            rb.rotation = Mathf.Lerp(rb.rotation, angle.Degree, 0.1f);
            rb.velocity = transform.right * moveSpeed;
        }

        private void SearchForTarget()
        {
            rb.velocity = transform.right * moveSpeed;
            var found = Physics2D.OverlapCircle(transform.position, searchRadius, LayerManager.GetLayerMask(Layer.Enemy));
            Debug.Log(found);

            if (found != null && found.CompareTag(TagManager.GetTag(Tag.Enemy)))
            {
                SetTarget(found.transform);
            }
        }

        protected override void Trigger(Collider2D collider)
        {
            if (!collider.CompareTag(TagManager.GetTag(Tag.EnemyBullet)) && !collider.CompareTag(TagManager.GetTag(Tag.NeutralBullet)))
            {
                Instantiate(explosionHitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>()
                    .EditProperties(0f, explosionRadius, playerDamage, Hitbox.HitboxAffiliation.Player);
                EffectManager.Instance.SpawnEffect(explosionVfx, transform.position);
                Destroy(gameObject);
            }
        }

    }
}
