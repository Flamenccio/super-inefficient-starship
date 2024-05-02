using Flamenccio.Effects.Visual;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    public class LemniscaticWindCyclingBullet : PlayerBullet
    {
        public int EnemiesHit { get; private set; }
        private float timer = 0f;
        private readonly float lifeTimer = 0.10f;

        protected override void DeathTimer()
        {
            timer += Time.deltaTime;

            if (timer >= lifeTimer)
            {
                Destroy(gameObject);
            }
        }
        protected override void Launch()
        {
            // this bullet doesn't launch; it follows the player
        }
        protected override void Trigger(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("EBullet"))
            {
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.BulletParry, transform.position);
            }
            else
            {
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.BulletImpact, transform.position);
            }
            if (collider.gameObject.CompareTag("Enemy"))
            {
                EnemiesHit++;
            }
        }
    }
}
