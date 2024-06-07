using Flamenccio.LevelObject;
using Flamenccio.LevelObject.Walls;
using Flamenccio.LevelObject.Stages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Core
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject portalPrefab;
        public int WallCount { get; private set; }
        private const int MAX_WALLS = 1000;

        /// <summary>
        /// Spawns a places a wall.
        /// </summary>
        /// <param name="stage">The stage to base the wall's position.</param>
        /// <param name="localCoordinates">Where to place the wall.</param>
        public void SpawnWall(Transform stage, Vector2 localCoordinates, int level)
        {
            if (stage == null || WallCount >= MAX_WALLS) return;

            WallCount++;
            var instance = Instantiate(wallPrefab, stage).GetComponent<Wall>();
            instance.transform.localPosition = localCoordinates;

            if (level == 2) instance.Upgrade();
        }

        /// <summary>
        /// Spawns two portals with randomized colors. The given stages must be different and must not be null.
        /// </summary>
        /// <param name="stage1">The stage to spawn the first portal.</param>
        /// <param name="stage2">The stage to spawn the second portal.</param>
        /// <param name="local1">Local coordinates to place the first portal.</param>
        /// <param name="local2">Local coordinates to place the second portal.</param>
        public void SpawnPortal(Stage stage1, Stage stage2, Vector2 local1, Vector2 local2)
        {
            if (stage1 == null || stage2 == null || stage1 == stage2) return;

            var instance1 = Instantiate(portalPrefab, stage1.transform).GetComponent<Portal>();
            var instance2 = Instantiate(portalPrefab, stage2.transform).GetComponent<Portal>();
            Color color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            instance1.PortalColor = color;
            instance2.PortalColor = color;
            instance1.transform.localPosition = local1;
            instance2.transform.localPosition = local2;
            instance1.SetDestination(instance2);
            instance2.SetDestination(instance1);
            stage1.AddPortal(instance1);
            stage2.AddPortal(instance2);
        }
    }
}
