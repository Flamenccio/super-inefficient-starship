using UnityEngine;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Attack.Player
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
                "PlayerIntangible",
                "Items",
                "ItemBox",
                "PBullet",
            };
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
            base.Trigger(collider);
        }
    }
}
