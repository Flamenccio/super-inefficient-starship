using UnityEngine;
using System.Collections.Generic;

namespace Flamenccio.LevelObject.Stages
{
    public class SecondaryWall : MonoBehaviour
    {
        public bool Built { get; private set; }

        private void Awake()
        {
            Built = false;
        }
        public void BuildWall(WallLayout.WallAttributes layout)
        {
            if (Built) return;

            switch (layout.Shape)
            {
                case WallLayout.WallShape.Rectangle:
                    BuildWallRectangle(layout.Center, layout.SideLengths);
                    break;

                case WallLayout.WallShape.Polygon:
                    BuildWallPolygon(layout.Origin, layout.Vertices);
                    break;

                default:
                    Debug.LogError("Type of wall shape not defined!");
                    break;
            }

            Built = true;
        }
        private void BuildWallRectangle(Vector2 centerPosition, Vector2 sideLengths)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            transform.localPosition = centerPosition;
            collider.size = sideLengths;
        }
        private void BuildWallPolygon(Vector2 originPosition, List<Vector2> vertices)
        {
            if (vertices.Count < 3)
            {
                Debug.LogError("Not enough vertices to make a polygon!");
                return;
            }

            PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();
            List<Vector2> adjustedVertices = new(vertices);
            Vector2 firstVector = vertices[0];

            for (int i = 0; i < vertices.Count; i++)
            {
                adjustedVertices[i] -= firstVector; // move the entire shape so that the first vertex is at (0,0)
                adjustedVertices[i] += originPosition; // then move it back to specified position
            }

            collider.SetPath(0, adjustedVertices);
        }
    }
}
