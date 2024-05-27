using UnityEngine;
using Flamenccio.Attack;

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
            return cooldownTimer >= Cooldown && consumeAmmo(Cost, PlayerAttributes.AmmoUsage.MainTap);
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