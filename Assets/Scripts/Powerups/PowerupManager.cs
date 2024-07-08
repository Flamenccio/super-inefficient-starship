namespace Flamenccio.Powerup
{
    public interface IPowerup
    {
        string Name { get; }
        string Desc { get; }
        int Level { get; }
        PowerupRarity Rarity { get; }

        void Run();
    }

    public enum PowerupRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    };
}