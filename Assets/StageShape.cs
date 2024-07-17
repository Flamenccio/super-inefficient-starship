using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Defines the shape of a stage segment.
    /// </summary>
    public class StageShape : MonoBehaviour
    {
        public Vector2 Extents { get => polygonMesh.bounds.extents; }
        public Vector2 Center { get => polygonMesh.bounds.center; }
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

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            polygonCollider = GetComponent<PolygonCollider2D>();
            polygonMesh = polygonCollider.CreateMesh(true, true);
        }

        public void FaceDirection(Directions.CardinalValues face)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, (int)face * -90f);
        }

        private void UpdateShape()
        {
            Destroy(polygonCollider);
            Destroy(polygonMesh);
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            polygonMesh = polygonCollider.CreateMesh(true, true);
            polygonCollider.isTrigger = true;
        }
    }
}
