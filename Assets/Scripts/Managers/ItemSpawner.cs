using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Core
{
    public class ItemSpawner : MonoBehaviour
    {
        // TODO make system to automatically retrieve items in folder.
        // * Each item class will have a "nameID" parameter that corresponds to the Name property in the Item struct.
        // * nameID will be editable in the editor.

        [System.Serializable]
        public struct Item
        {
            [field: SerializeField, Tooltip("Must be all lowercase, no spaces.")] public string Name;
            [field: SerializeField] public GameObject Prefab;
        }

        [SerializeField] private List<Item> prefabs = new();

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
        /// Spawns an item on given stage and coordinates.
        /// </summary>
        /// <param name="item">Item to spawn.</param>
        /// <param name="stage">Stage to spawn item in.</param>
        /// <param name="localCoordinate">Local coordinates on stage.</param>
        /// <returns>GameObject of item that was spawned. Returns null on failure.</returns>
        public GameObject SpawnItem(Item item, Transform stage, Vector2 localCoordinate)
        {
            if (stage == null) return null;

            if (item.Prefab == null) return null;

            var spawn = Instantiate(item.Prefab, stage, false);
            spawn.transform.localPosition = localCoordinate;

            return spawn;
        }

        /// <summary>
        /// Spawns an item on given stage and coordinates.
        /// </summary>
        /// <param name="item">Item to spawn.</param>
        /// <param name="stage">Stage to spawn item in.</param>
        /// <param name="localCoordinate">Local coordinates on stage.</param>
        /// <returns>GameObject of item that was spawned. Returns null on failure.</returns>
        public GameObject SpawnItem(Item item, Vector2 globalCoordinate)
        {
            if (item.Prefab == null) return null;

            return Instantiate(item.Prefab, globalCoordinate, Quaternion.identity);
        }

        /// <summary>
        /// Tries to get an Item from name.
        /// </summary>
        /// <param name="name">Name of item.</param>
        /// <returns>Item struct; an empty item struct on failure.</returns>
        public Item GetItem(string name)
        {
            if (!prefabs.Exists(x => x.Name.Equals(name))) return new();

            return prefabs.Find(x => x.Name.Equals(name));
        }

        private GameObject GetGameObjectFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return prefabs.Find(x => x.Name.Equals(name)).Prefab;
        }
    }
}
