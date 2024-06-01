using UnityEngine;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Attack.Player
{
    /// <summary>
    /// Base class for all player attacks.
    /// </summary>
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
                "Effect",
                "PlayerIntangible",
                "Items",
                "ItemBox",
                "PBullet",
                "Sensor",
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
