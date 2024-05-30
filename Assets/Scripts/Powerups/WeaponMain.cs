using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.HUD;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Base class for all main weapons.
    /// </summary>
    public class WeaponMain : WeaponBase
    {
        public int ChargedCost { get => chargedCost; }
        [SerializeField] protected int chargedCost = 0;

        protected override void Startup()
        {
            weaponType = WeaponType.Main;
            base.Startup();
        }

        protected bool AttackReady()
        {
            if (cooldownTimer < Cooldown) return false;

            if (!consumeAmmo(Cost, PlayerAttributes.AmmoUsage.MainTap))
            {
                FloatingTextManager.Instance.DisplayText("AMMO LOW", transform.position, Color.yellow, 0.8f, 30f, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.ZoomIn);
                return false;
            }

            return true;
        }

        public int GetWeaponDamage()
        {
            return mainAttack.GetComponent<BulletControl>().PlayerDamage;
        }

        public float GetWeaponRange()
        {
            return mainAttack.GetComponent<BulletControl>().Range;
        }

        public float GetWeaponSpeed()
        {
            return mainAttack.GetComponent<BulletControl>().Speed;
        }

        public float GetWeaponCooldown()
        {
            return Cooldown;
        }
    }
}