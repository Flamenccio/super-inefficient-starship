using Flamenccio.Effects.Audio;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    /// <summary>
    /// Controls a player sub weapon. Explodes upon contact or when its lifetime runs out. Ignores collisions with enemy attacks.
    /// </summary>
    public class Grenade : PlayerBullet
    {
        public float MaxLifetime { get => MAX_LIFE_TIME; }
        [SerializeField] private GameObject hitbox;
        [SerializeField] private string explosionEffect;
        [SerializeField] private string explosionSfx;
        [SerializeField] private float explosionRadius;
        [SerializeField] private Animator animator;
        [SerializeField] private int blastDamage;

        private float lifeTimer = 0f;
        private const float MAX_LIFE_TIME = 1.5f;

        protected override void Startup()
        {
            base.Startup();
            collisionTags.Remove(TagManager.GetTag(Tag.EnemyBullet));
        }

        protected override void Launch()
        {
            rb.AddForce(transform.right * Speed, ForceMode2D.Impulse);
        }

        protected override void Trigger(Collider2D collider)
        {
            Detonate();
        }

        protected override void Collide(Collision2D collision)
        {
            Detonate();
        }

        protected override void Behavior()
        {
            if (lifeTimer >= MAX_LIFE_TIME)
            {
                Detonate();
            }
            else
            {
                lifeTimer += Time.deltaTime;
                animator.speed = lifeTimer;
            }
        }

        private void Detonate()
        {
            cameraEff.ScreenShake(CameraEffects.ScreenShakeIntensity.Extreme, transform.position);
            Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>().EditProperties(0f, explosionRadius, blastDamage, Hitbox.HitboxAffiliation.Player, knockbackMultiplier);
            EffectManager.Instance.SpawnEffect(explosionEffect, transform.position);
            AudioManager.Instance.PlayOneShot(explosionSfx, transform.position);
            Destroy(gameObject);
        }
    }
}