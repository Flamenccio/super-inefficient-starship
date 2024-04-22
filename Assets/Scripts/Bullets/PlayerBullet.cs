using UnityEngine;

namespace Flamenccio.Attack
{
    public class PlayerBullet : BulletControl
    {
        protected override void Startup()
        {
            ignoredTags = new()
            {
                "Player",
                "Star",
                "Background",
                "Footprint",
                "Heart",
                "MiniStar",
                "Effects",
                "PlayerIntangible"
            };
        }
    }
}
