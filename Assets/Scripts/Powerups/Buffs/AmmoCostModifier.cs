namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// Allows powerups to alter ammo costs.
    /// </summary>
    public class AmmoCostModifier
    {
        private int multiplier;
        private int offset;

        public AmmoCostModifier()
        {
            multiplier = 1;
            offset = 0;
        }

        public int GetFinalCost(int baseCost)
        {
            return (baseCost * multiplier) + offset;
        }

        public void SetMultiplier(int newMultiplier)
        {
            if (newMultiplier < 0) return;

            multiplier = newMultiplier;
        }

        public void SetOffset(int newOffset)
        {
            offset = newOffset;
        }

        public void ResetMultiplier()
        {
            multiplier = 1;
        }

        public void ResetOffset()
        {
            offset = 0;
        }
    }
}