using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    public class WeaponDefense : WeaponBase
    {
        protected override void Startup()
        {
            base.Startup();
            weaponType = WeaponType.Defense;
        }
    }
}
