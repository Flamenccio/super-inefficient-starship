using Flamenccio.Objects;
using Flamenccio.LevelObject.Stages;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace Flamenccio.Core
{
    public interface IObject
    {
        string GetObjectName { get; }
    }
    /// <summary>
    /// Manages the spawning of objects.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {
        public struct ObjectId
        {
            public string Name { get; set; }
            public GameObject Prefab { get; set; }
        }

        public int WallCount { get; private set; }
        private const int MAX_WALLS = 1000;
        private List<ObjectId> objectPrefabs = new();
        private GameObject wallPrefab;

        private void Awake()
        {
            objectPrefabs = LoadObjects();
            wallPrefab = GetObject("wall"); // walls are spawned pretty frequently, so cache their prefab
        }

        private List<ObjectId> LoadObjects()
        {
            List<ObjectId> prefabs = new();
            Resources.LoadAll<GameObject>("Prefabs/Objects")
                .Select(x =>
                {
                    x.TryGetComponent<IObject>(out var i);
                    return new Tuple<IObject, GameObject>(i, x);
                })
                .Where(x => x.Item2 != null && !string.IsNullOrEmpty(x.Item1.GetObjectName))
                .ToList()
                .ForEach(x =>
                {
                    ObjectId id = new()
                    {
                        Name = x.Item1.GetObjectName,
                        Prefab = x.Item2
                    };
                    prefabs.Add(id);
                });

            return prefabs;
        }

        private GameObject GetObject(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return objectPrefabs.Find(x => x.Name.Equals(name)).Prefab;
        }

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
            instance.OnDeath = () => WallCount--;

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
            if (stage1 == null || stage2 == null || stage1.GetInstanceID() == stage2.GetInstanceID()) return;

            var prefab = GetObject("portal");
            var instance1 = Instantiate(prefab, stage1.transform).GetComponent<Portal>();
            var instance2 = Instantiate(prefab, stage2.transform).GetComponent<Portal>();
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
