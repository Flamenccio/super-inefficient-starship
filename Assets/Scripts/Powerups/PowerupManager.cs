namespace Flamenccio.Powerup
{
    public interface IPowerup
    {
        string Name { get; }
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