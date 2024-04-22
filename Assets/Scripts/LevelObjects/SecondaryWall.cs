using UnityEngine;

namespace Flamenccio.LevelObject.Stages
{
    public class SecondaryWall : MonoBehaviour
    {
        public Vector2 relativePosition = Vector2.zero;
        public Vector2 stagePosition = Vector2.zero;
        [SerializeField] public float xSize = 1f;
        [SerializeField] public float ySize = 1f;
        [SerializeField] private BoxCollider2D boxCol;

        private void Awake()
        {
            boxCol = gameObject.GetComponent<BoxCollider2D>();
            boxCol.size = new Vector2(xSize, ySize);
            gameObject.transform.position = relativePosition;
        }
        public void UpdateAttributes()
        {
            boxCol.size = new Vector2(xSize, ySize);
            gameObject.transform.position = (Vector2)transform.parent.position + relativePosition;
        }
    }
}
