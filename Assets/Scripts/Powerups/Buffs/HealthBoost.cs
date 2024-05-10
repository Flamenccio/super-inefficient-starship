namespace Flamenccio.Powerup
{
    public class HealthBoost : UnconditionalBuff
    {
        public HealthBoost()
        {
            Name = "Health boost";
            Desc = "Increases maximum HP by 1.";
            Level = 1;
            static float f(int level) =>  level;
            buffs.Add(new StatBuff(PlayerAttributes.Attribute.MaxHP, f));
        }
    }
}
