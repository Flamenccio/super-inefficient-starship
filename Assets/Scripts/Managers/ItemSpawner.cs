using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Flamenccio.Core
{
    /// <summary>
    /// Manages spawning of items.
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        [System.Serializable]
        private struct ItemObject
        {
            [field: SerializeField, Tooltip("Must be all lowercase, no spaces.")] public string Name;
            [field: SerializeField] public GameObject Prefab;
        }

        private List<ItemObject> prefabs = new();

        private void Awake()
        {
            prefabs = LoadItems();
        }

        private List<ItemObject> LoadItems()
        {
            var gameObjects = Resources.LoadAll<GameObject>("Prefabs/Items");
            List<ItemObject> itemObjects = new();
            gameObjects
                .Select(x =>
                {
                    x.TryGetComponent<Item.Item>(out var i);
                    return i;
                })
                .Where(x => x != null && !string.IsNullOrEmpty(x.ItemName))
                .ToList()
                .ForEach(x =>
                {
                    ItemObject obj = new()
                    {
                        Name = x.ItemName,
                        Prefab = x.gameObject
                    };
                    itemObjects.Add(obj);
                });

            return itemObjects;
        }

        /// <summary>
        /// Spawns an item on given stage and coordinates.
        /// </summary>
        /// <param name="name">Name of item to spawn.</param>
        /// <param name="stage">Stage to spawn item in.</param>
        /// <param name="localCoordinate">Local coordinates on stage.</param>
        /// <returns>GameObject of item that was spawned. Returns null on failure.</returns>
        public GameObject SpawnItem(string name, Transform stage, Vector2 localCoordinate)
        {
            if (stage == null) return null;

            var prefab = GetGameObjectFromName(name);

            if (prefab == null) return null;

            prefab = Instantiate(prefab, stage, false);
            prefab.transform.localPosition = localCoordinate;

            return prefab;
        }

        /// <summary>
        /// Spawn an item at given coordinates.
        /// </summary>
        /// <param name="name">Name of item to spawn.</param>
        /// <param name="globalCoordinate">Where to spawn item.</param>
        /// <returns>Item that was spawned; null on failure.</returns>
        public GameObject SpawnItem(string name, Vector2 globalCoordinate)
        {
            var prefab = GetGameObjectFromName(name);

            if (prefab == null) return null;

            prefab = Instantiate(prefab, globalCoordinate, Quaternion.identity);

            return prefab;
        }

        public void SpawnItem(string name, Vector2 globalCoordinate, int repeats)
        {
            if (repeats < 1) return;

            var prefab = GetGameObjectFromName(name);

            if (prefab == null) return;

            for (int i = repeats; i > 0; i--)
            {
                Instantiate(prefab, globalCoordinate, Quaternion.identity);
            }
        }

        private GameObject GetGameObjectFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return prefabs.Find(x => x.Name.Equals(name)).Prefab;
        }

        /// <summary>
        /// Get the item ID of given item name.
        /// </summary>
        /// <param name="name">Name of item.</param>
        /// <returns>Item ID. -1 if invalid name.</returns>
        public int GetItemId(string name)
        {
            if (!prefabs.Exists(x => x.Name.Equals(name))) return -1;

            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].Name.Equals(name)) return i;
            }

            return -1;
        }
    }
}
