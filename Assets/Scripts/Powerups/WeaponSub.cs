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
    }
}
