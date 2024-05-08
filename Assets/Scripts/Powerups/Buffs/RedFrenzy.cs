namespace Flamenccio.Powerup
{
    public class RedFrenzy : UnconditionalBuff
    {
        public RedFrenzy()
        {
            Name = "Red Frenzy";
            Desc = "Successfully slaying enemies in quick succession increaes movement speed.\nGetting hit with Red Frenzy causes you to lose all stacks.\n";
            Level = 1;
            static float f(int level) => level * 0.08f;
            buffs.Add(new StatBuff(PlayerAttributes.Attribute.MoveSpeed, f));
        }
    }
}
