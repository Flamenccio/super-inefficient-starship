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
        [SerializeField] protected string bulletParryVfx = "m_bullet_parry";
        [SerializeField] protected string bulletImpactVfx = "m_bullet_impact";

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
                EffectManager.Instance.SpawnEffect(bulletParryVfx, transform.position);
            }
            else
            {
                EffectManager.Instance.SpawnEffect(bulletImpactVfx, transform.position);
            }

            base.Trigger(collider);
        }
    }
}
