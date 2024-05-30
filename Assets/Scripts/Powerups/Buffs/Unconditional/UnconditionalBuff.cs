namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// Base class for unconditional buffs.
    /// </summary>
    public class UnconditionalBuff : BuffBase
    {
        public UnconditionalBuff()
        {
            Type = BuffType.Unconditional;
            Level = 1;
        }
    }
}
