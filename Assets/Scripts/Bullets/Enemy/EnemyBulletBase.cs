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
            ignoredTags = new List<string>()
            {
                "EBullet",
                "Enemy",
                "Background",
                "Star",
                "Footprint",
                "Heart",
                "MiniStar",
                "Effect"
            };
        }

        protected override void Trigger(Collider2D collider)
        {
            Destroy(this.gameObject);
        }
    }
}
