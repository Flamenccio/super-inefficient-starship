using UnityEngine;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;

namespace Flamenccio.Attack.Enemy
{
    /// <summary>
    /// Controls behavior of enemy bomb. Explodes upon touching player.
    /// </summary>
    public class Bomb : EnemyBulletBase
    {
        [SerializeField] private GameObject explosionHitbox;

        protected override void Startup()
        {
            base.Startup();
            collisionTags.Remove(TagManager.GetTag(Tag.Wall));
            collisionTags.Remove(TagManager.GetTag(Tag.Trigger));
        }

        protected override void Trigger(Collider2D collider)
        {
            EffectManager.Instance.SpawnEffect("EnemyExplosion", transform.position);
            Instantiate(explosionHitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>().EditProperties(0f, 2f, playerDamage, Hitbox.HitboxAffiliation.Neutral, knockbackMultiplier); // spawn the actual hitbox
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Extreme, transform.position);
            base.Trigger(collider);
        }
    }
}
