using Flamenccio.Effects.Visual;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    public class Grenade : PlayerBullet
    {
        [SerializeField] private GameObject hitbox;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private float explosionRadius;
        private float lifeTimer = 0f;
        private float maxLife = 1.5f;
        [SerializeField] private Animator animator;

        protected override void Startup()
        {
            base.Startup();
            ignoredTags.Add("EBullet");
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
            if (lifeTimer >= maxLife)
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
            Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>().EditProperties(0f, explosionRadius, playerDamage, Hitbox.AttackType.Player, knockbackMultiplier);
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
