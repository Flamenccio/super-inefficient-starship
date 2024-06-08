using UnityEngine;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Controls the (at most) four walls around a stage.
    /// </summary>
    public class PrimaryWall : MonoBehaviour
    {
        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        private BoxCollider2D wallCollider;
        private const float BOX_LONG_LENGTH = 16.0f;
        private const float BOX_SHORT_LENGTH = 2.0f;

        private void Awake()
        {
            wallCollider = gameObject.GetComponent<BoxCollider2D>();
            wallCollider.enabled = false;
        }

        /// <summary>
        /// Spawn this wall with the given orientaiton.
        /// </summary>
        public void SpawnWall(Orientation wallOrient)
        {
            // load correct wall
            if (wallOrient == Orientation.Horizontal)
            {
                wallCollider.size = new Vector2(BOX_LONG_LENGTH, BOX_SHORT_LENGTH);
            }
            else
            {
                wallCollider.size = new Vector2(BOX_SHORT_LENGTH, BOX_LONG_LENGTH);
            }

            wallCollider.enabled = true;
        }

        /// <summary>
        /// Destroy this wall.
        /// </summary>
        public void DestroyWall()
        {
            if (wallCollider != null)
            {
                Destroy(wallCollider);
            }
        }
    }
}