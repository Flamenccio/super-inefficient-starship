namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// Increases movement speed by 10% every level.
    /// </summary>
    public class MovementSpeed : UnconditionalBuff
    {
        public MovementSpeed()
        {
            Name = "Thruster Upgrade";
            Class = BuffClass.Agility;
            static float f1(int level) => level * 0.10f;
            buffs.Add(new StatBuff(PlayerAttributes.Attribute.MoveSpeed, f1));
        }
    }
}
