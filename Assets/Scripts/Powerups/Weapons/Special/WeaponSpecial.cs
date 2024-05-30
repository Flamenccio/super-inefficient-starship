using UnityEngine;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Base class for all special weapons.
    /// </summary>
    public class WeaponSpecial : WeaponBase
    {
        // base class for all special weapons
        [SerializeField] protected int maxSpecialCharges = 1;

        protected override void Startup()
        {
            base.Startup();
            weaponType = WeaponType.Special;
        }

        protected override void Start()
        {
            base.Start();
            playerAtt.SetCharges(maxSpecialCharges, cooldown);
        }

        protected override bool AttackReady()
        {
            return playerAtt.UseCharge(Cost);
        }
    }
}