using UnityEngine;
using Flamenccio.Attack;

namespace Flamenccio.Powerup.Weapon
{
    public class WeaponMain : WeaponBase
    {
        // parent class of all main weapons
        [SerializeField] protected GameObject mainAttack;
        protected override void Startup()
        {
            weaponType = WeaponType.Main;
            base.Startup();
        }
        public int GetWeaponDamage()
        {
            return mainAttack.GetComponent<BulletControl>().Damage;
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
