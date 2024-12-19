namespace Flamenccio.Powerup
{
    public interface IPowerup
    {
        string WeaponID { get; }
        int Level { get; }
        PowerupRarity Rarity { get; }

        void Run();
    }

    public enum PowerupRarity
    {
        Common,
        Rare,
        Legendary,
        Relic
    };
}