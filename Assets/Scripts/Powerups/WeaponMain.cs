using UnityEngine;
using Flamenccio.Attack;

namespace Flamenccio.Powerup.Weapon
{
    public class WeaponMain : WeaponBase
    {
        public int ChargedCost { get => chargedCost; }
        [SerializeField] protected int chargedCost = 0;
        protected override void Startup()
        {
            weaponType = WeaponType.Main;
            base.Startup();
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
