using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Defines the shape of a stage segment.
    /// </summary>
    public class StageShape : MonoBehaviour
    {
        public Vector2 Extents { get => GetRotatedExtents(faceDirection); }
        public Vector2 Center { get => GetRotatedCenter(faceDirection); }
        public Sprite StageSprite
        {
            get { return stageSprite; }
            set
            {
                stageSprite = value;
                spriteRenderer.sprite = stageSprite;
                UpdateShape();
            }
        }
        private Sprite stageSprite;
        private SpriteRenderer spriteRenderer;
        private PolygonCollider2D polygonCollider;
        private Mesh polygonMesh;
        private Directions.CardinalValues faceDirection = Directions.CardinalValues.North; // North by default.

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            polygonCollider = GetComponent<PolygonCollider2D>();
            polygonMesh = polygonCollider.CreateMesh(true, true);
        }

        public void FaceDirection(Directions.CardinalValues face)
        {
            faceDirection = face;
            transform.rotation = Quaternion.Euler(0f, 0f, (int)faceDirection * -90f);
        }

        private void UpdateShape()
        {
            Destroy(polygonCollider);
            Destroy(polygonMesh);
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            polygonMesh = polygonCollider.CreateMesh(true, true);
            polygonCollider.isTrigger = true;
        }

        /// <summary>
        /// Rotates polygonMesh.bounds.center and returns the result.
        /// Assumes that the original direction is North.
        /// </summary>
        private Vector2 GetRotatedCenter(Directions.CardinalValues newDirection)
        {
            var originalVector = polygonMesh.bounds.center;

            for (int i = 0; i < (int)newDirection; i++)
            {
                originalVector = new(originalVector.y, -originalVector.x);
            }

            return originalVector;
        }

        /// <summary>
        /// Rotates polygonMesh.bounds.extents and returns the result.
        /// Assumes that the original direction is North.
        /// <para>Note: both x and y components are kept positive.</para>
        /// </summary>
        private Vector2 GetRotatedExtents(Directions.CardinalValues newDirection)
        {
            var originalBounds = polygonMesh.bounds.extents;

            for (int i = 0; i < (int)newDirection; i++)
            {
                originalBounds = new(originalBounds.y, originalBounds.x);
            }

            return originalBounds;
        }
    }
}
