using Flamenccio.HUD;
using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Base class for all sub weapons.
    /// </summary>
    public class WeaponSub : WeaponBase
    {
        protected override void Startup()
        {
            base.Startup();
            weaponType = WeaponType.Sub;
        }

        protected override bool AttackReady()
        {
            if (cooldownTimer < Cooldown) return false;

            if (!consumeAmmo(Cost, PlayerAttributes.AmmoUsage.MainTap))
            {
                FloatingTextManager.Instance.DisplayText("AMMO LOW", transform.position, Color.yellow, 0.8f, 30f, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.ZoomIn, true);
                return false;
            }

            return true;
        }
    }
}
