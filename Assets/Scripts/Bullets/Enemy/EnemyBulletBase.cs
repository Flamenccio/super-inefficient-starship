using Flamenccio.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Attack.Enemy
{
    /// <summary>
    /// Base class for all enemy attacks.
    /// </summary>
    public class EnemyBulletBase : BulletControl
    {
        protected override void Startup()
        {
            collisionTags = TagManager.GetTagCollection(new()
            {
                Tag.Wall,
                Tag.InvisibleWall,
                Tag.Player,
                Tag.PlayerBullet,
                Tag.NeutralBullet,
                Tag.PrimaryWall,
            });
        }

        protected override void Trigger(Collider2D collider)
        {
            Destroy(this.gameObject);
        }
    }
}
