namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Base class for all defensive weapons.
    /// </summary>
    public class WeaponDefense : WeaponBase
    {
        protected override void Startup()
        {
            base.Startup();
            weaponType = WeaponType.Defense;
        }
    }
}
