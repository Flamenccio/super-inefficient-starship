using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    /// <summary>
    /// Controls a special attack. A dash attack that deals damage to obstacles and enemies in path.
    /// </summary>
    public class LemniscaticWindCyclingBullet : PlayerBullet
    {
        public int EnemiesHit { get; private set; }
        private float timer = 0f;
        private const float MAX_LIFE_TIMER = 0.10f;

        protected override void DeathTimer()
        {
            timer += Time.deltaTime;

            if (timer >= MAX_LIFE_TIMER)
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
            if (collider.gameObject.CompareTag(TagManager.GetTag(Tag.EnemyBullet)))
            {
                EffectManager.Instance.SpawnEffect("m_bullet_parry", transform.position);
            }
            else
            {
                EffectManager.Instance.SpawnEffect("m_bullet_impact", transform.position);
            }
            if (collider.gameObject.CompareTag(TagManager.GetTag(Tag.Enemy)))
            {
                EnemiesHit++;
            }
        }
    }
}