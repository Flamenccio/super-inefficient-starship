using UnityEngine;

namespace Flamenccio.Core
{
    public class EntitySpawner : MonoBehaviour
    {
        private EnemyList enemyList;

        private void Awake()
        {
            enemyList = gameObject.GetComponent<EnemyList>();
        }

        /// <summary>
        /// Spawns and places a random enemy.
        /// </summary>
        /// <param name="globalCoordinates">Where to place enemy.</param>
        /// <param name="gameLevel">The difficulty level of the game.</param>
        /// <returns>The enemy that was spawned.</returns>
        public GameObject SpawnRandomEnemy(Vector2 globalCoordinates, int gameLevel)
        {
            var enemy = enemyList.GetRandomEnemy(gameLevel);

            if (enemy == null) return null;

            Instantiate(enemy).transform.position = globalCoordinates;

            return enemy;
        }
    }
}
