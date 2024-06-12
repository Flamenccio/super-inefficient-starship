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
        Stage,
        NeutralAttack,
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
            { Layer.Stage, LayerMask.NameToLayer("Background") },
            { Layer.Stage, LayerMask.NameToLayer("NeutralAttack") },
        };

        /// <summary>
        /// Calculates a layer mask from a collection of layers.
        /// </summary>
        /// <param name="layers">All layers to convert to a list of LayerMasks.</param>
        /// <returns>A list of LayerMasks.</returns>
        public static LayerMask GetLayerMask(List<Layer> layers)
        {
            LayerMask mask = 0;
            layers
                .ToList()
                .ForEach(x => mask |= GetLayerMask(x));

            return mask;
        }

        /// <summary>
        /// Return's the given layer's corresponding LayerMask.
        /// </summary>
        public static LayerMask GetLayerMask(Layer layer)
        {
            return 1 << (int)Layers[layer];
        }

        /// <summary>
        /// Returns the given layer's number. <b>Note: not the same as a LayerMask.</b>
        /// </summary>
        public static int GetLayer(Layer layer)
        {
            return (int)Layers[layer];
        }
    }
}
