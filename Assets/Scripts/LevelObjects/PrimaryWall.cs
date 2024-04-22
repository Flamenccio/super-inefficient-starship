using UnityEngine;

namespace Flamenccio.LevelObject.Stages
{
    public class PrimaryWall : MonoBehaviour
    {
        private BoxCollider2D wallCollider;
        private const float BOX_LONG_LENGTH = 16.0f;
        private const float BOX_SHORT_LENGTH = 2.0f;
        public enum orientation
        {
            Horizontal,
            Vertical
        }
        private void Awake()
        {
            wallCollider = gameObject.GetComponent<BoxCollider2D>();
            wallCollider.enabled = false;

        }
        public void SpawnWall(orientation wallOrient)
        {
            // load correct wall
            if (wallOrient == orientation.Horizontal)
            {
                wallCollider.size = new Vector2(BOX_LONG_LENGTH, BOX_SHORT_LENGTH);
            }
            else
            {
                wallCollider.size = new Vector2(BOX_SHORT_LENGTH, BOX_LONG_LENGTH);
            }
            wallCollider.enabled = true;
        }
        // destroy the primary wall collider (not the script!)
        public void DestroyWall()
        {
            if (wallCollider != null)
            {
                Destroy(wallCollider);
            }

        }
    }
}
