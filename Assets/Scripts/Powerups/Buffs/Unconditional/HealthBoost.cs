namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// Increases maximum HP by 1 every level.
    /// </summary>
    public class HealthBoost : UnconditionalBuff
    {
        public HealthBoost()
        {
            WeaponID = "Health boost";
            static float f(int level) =>  level;
            buffs.Add(new StatBuff(PlayerAttributes.Attribute.MaxHP, f));
        }
    }
}
