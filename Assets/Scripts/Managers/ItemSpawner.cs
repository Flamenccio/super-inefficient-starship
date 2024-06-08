using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Core
{
    public class ItemSpawner : MonoBehaviour
    {
        public enum Item
        {
            Star,
            Heart,
            StarShard,
        };

        [SerializeField] private GameObject starPrefab;
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private GameObject starShardPrefab;

        private Dictionary<Item, GameObject> prefabs = new();

        private void Awake()
        {
            prefabs = new()
            {
                { Item.Star, starPrefab },
                { Item.StarShard, starShardPrefab },
                { Item.Heart, heartPrefab },
            };
        }

        public GameObject SpawnItem(Item item, Transform stage, Vector2 localCoordinate)
        {
            if (stage == null) return null;

            var instance = Instantiate(prefabs[item], stage);
            instance.transform.localPosition = localCoordinate;

            return instance;
        }
    }
}
