using System.Collections.Generic;
using System.Linq;

namespace Flamenccio.Utility
{
    /// <summary>
    /// A list of all tags in use. <b>Does not include default Unity tags.</b>
    /// </summary>
    public static class TagManager
    {
        public enum Tag
        {
            Player,
            Wall,
            Star,
            PlayerBullet,
            Stage,
            InvisibleWall,
            Enemy,
            EnemyBullet,
            PlayerFootprint,
            Heart,
            StarShard,
            PrimaryWall,
            Trigger,
            Effect,
            NeutralBullet,
            ItemBox,
            Portal,
            Sensor
        };

        public static Dictionary<Tag, string> Tags { get; } = new()
        {
            {Tag.Player, "Player" },
            {Tag.Wall, "Wall" },
            {Tag.Star, "Star" },
            {Tag.PlayerBullet, "PBullet" },
            {Tag.Stage, "Background" },
            {Tag.InvisibleWall, "InvisibleWall" },
            {Tag.Enemy, "Enemy"},
            {Tag.EnemyBullet, "EBullet" },
            {Tag.PlayerFootprint, "Footprint" },
            {Tag.Heart, "Heart" },
            {Tag.StarShard, "MiniStar" },
            {Tag.PrimaryWall, "PrimaryWall" },
            {Tag.Trigger, "Trigger" },
            {Tag.Effect, "Effect" },
            {Tag.NeutralBullet, "NBullet" },
            {Tag.ItemBox, "ItemBox" },
            {Tag.Portal, "Portal" },
            {Tag.Sensor, "Sensor" }
        };

        public static List<string> GetTagCollection(List<Tag> tags)
        {
            return Tags
                .Where(x => tags.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();
        }
    }
}
