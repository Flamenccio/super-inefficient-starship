using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Utility
{
    public enum Layer
    {
        Enemy,
        Player,
        Item,
        Wall,
        InvisibleWall,
        Footprint,
        PlayerBullet,
        StageLink,
        RaycastTest,
        Effect,
        PlayerIntangible,
        Sensor,
    }

    /// <summary>
    /// A class that manages all game-specific layers. <b>Does not include default Unity layers.</b>
    /// </summary>
    public static class LayerManager
    {
        public static Dictionary<Layer, LayerMask> Layers { get; } = new()
        {
            { Layer.Enemy, LayerMask.NameToLayer("Enemies") },
            { Layer.Player, LayerMask.NameToLayer("Player") },
            { Layer.Item, LayerMask.NameToLayer("Items") },
            { Layer.Wall, LayerMask.NameToLayer("Wall") },
            { Layer.InvisibleWall, LayerMask.NameToLayer("InvisWall") },
            { Layer.Footprint, LayerMask.NameToLayer("Footprint") },
            { Layer.PlayerBullet, LayerMask.NameToLayer("PlayerBullet") },
            { Layer.StageLink, LayerMask.NameToLayer("StageLink") },
            { Layer.RaycastTest, LayerMask.NameToLayer("RaycastTest") },
            { Layer.Effect, LayerMask.NameToLayer("Effects") },
            { Layer.PlayerIntangible, LayerMask.NameToLayer("PlayerIntangible") },
            { Layer.Sensor, LayerMask.NameToLayer("Sensor") },
        };

        /// <summary>
        /// Calculates a layer mask from a collection of layers.
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static LayerMask GetLayerMask(List<Layer> layers)
        {
            LayerMask mask = new();
            layers
                .ToList()
                .ForEach(x => mask += Layers[x] << 1);

            return mask;
        }

        /// <summary>
        /// Same as Layers[layer].
        /// </summary>
        public static LayerMask GetLayerMask(Layer layer)
        {
            return Layers[layer];
        }
    }
}
