namespace Flamenccio.Powerup.Weapon
{
    public class WeaponSpecial : WeaponBase
    {
        // base class for all special weapons
        protected override void Startup()
        {
            base.Startup();
            weaponType = WeaponType.Special;
        }
    }
}
