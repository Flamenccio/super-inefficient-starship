using System.Collections.Generic;
using System.Linq;

namespace Flamenccio.Utility
{
    public enum Tag
    {
        None,
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

    /// <summary>
    /// A list of all tags in use. <b>Does not include default Unity tags.</b>
    /// </summary>
    public static class TagManager
    {
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

        /// <summary>
        /// Returns a list of tags given a list of tag enums.
        /// </summary>
        /// <param name="tags">List of tag enums.</param>
        /// <returns>A list of string tags.</returns>
        public static List<string> GetTagCollection(List<Tag> tags)
        {
            return Tags
                .Where(x => tags.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();
        }

        /// <summary>
        /// A cleaner way of using Tags[tag].
        /// </summary>
        public static string GetTag(Tag tag)
        {
            return Tags[tag];
        }
    }
}