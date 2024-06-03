using UnityEngine;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;

namespace Flamenccio.Attack.Player
{
    /// <summary>
    /// Base class for all player attacks.
    /// </summary>
    public class PlayerBullet : BulletControl
    {
        protected override void Startup()
        {
            collisionTags = TagManager.GetTagCollection(new()
            {
                Tag.Enemy,
                Tag.EnemyBullet,
                Tag.InvisibleWall,
                Tag.Wall,
                Tag.PrimaryWall,
            });
        }

        protected override void Trigger(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(TagManager.GetTag(Tag.EnemyBullet)))
            {
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.BulletParry, transform.position);
            }
            else
            {
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.BulletImpact, transform.position);
            }

            base.Trigger(collider);
        }
    }
}
