using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// A "blueprint" that tells where to place stage walls.
    /// </summary>
    [CreateAssetMenu(fileName = "Empty Layout", menuName = "Invisible Wall Layout", order = 0)]
    public class WallLayout : ScriptableObject
    {
        [System.Serializable] public enum WallShape
        {
            Rectangle,
            Polygon,
        };
        [System.Serializable] public struct WallAttributes
        {
            public WallShape Shape;
            public Vector2 SideLengths;
            public Vector2 Center;
            public List<Vector2> Vertices;
            public Vector2 Origin;
        }
        public List<WallAttributes> Layout = new();
    }
}