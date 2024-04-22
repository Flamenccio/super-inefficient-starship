namespace Flamenccio.Powerup
{
    public class MovementSpeed : UnconditionalBuff
    {
        public MovementSpeed()
        {
            Name = "Thruster Upgrade";
            Level = 1;
            Class = BuffClass.Agility;
            static float f1(int level) => level * 0.10f;
            Desc = $"[LEVEL {Level}]: Move {f1(Level)}% faster.";
            buffs.Add(new StatBuff(PlayerAttributes.Attribute.MoveSpeed, f1));
        }
    }
}
