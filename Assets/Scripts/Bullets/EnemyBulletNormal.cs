using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Attack
{
    public class EnemyBulletNormal : BulletControl
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
                "Effects"
            };
        }
        protected override void Trigger(Collider2D collider)
        {
            Destroy(this.gameObject);
        }
    }
}
