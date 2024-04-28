using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Test
{
    public class TestPolygonCreator : MonoBehaviour
    {
        public enum Shapes
        {
            Square,
            Rectangle,
            Polygon
        };

        [System.Serializable]
        public struct Polygon
        {
            public List<Vector2> Vertices;
            public Shapes Shape;
            public float SquareSize;
            public Vector2 RectangleSize;
            public Vector2 Origin;
            public Vector2 Center;
        }

        public List<Polygon> Polygons = new();
        public List<Vector2> Vertices = new();
        public Shapes Shape;
        public float SquareSize;
        public Vector2 RectangleSize;
        public Vector2 Origin;
        public Vector2 Center;
        private void Start()
        {
            PolygonCollider2D polycollider = gameObject.AddComponent<PolygonCollider2D>();
            polycollider.SetPath(0, Vertices);
        }
        private void Awake()
        {
        }

    }
}
