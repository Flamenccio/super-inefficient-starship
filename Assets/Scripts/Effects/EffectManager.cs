using Flamenccio.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Flamenccio.Effects.Visual
{
    public class EffectManager : MonoBehaviour
    {
        [Serializable]
        public struct EffectObject
        {
            [field: SerializeField, Tooltip("Name of the effect. Must be in snake_case, have no spaces.\nThe general format of the name should look something like: \n<category>_<trigger>_<more info>\nExample: p_weapon_main_blaster_tap")] public string Name { get; set; }
            [field: SerializeField] public GameObject Effect { get; set; }
        }

        public struct EffectID
        {
            public EffectCategory Category { get; set; }
            public int Index { get; set; }
        }

        public enum EffectCategory
        {
            None,
            Player,
            Item,
            Enemy,
            Object,
            Misc
        }

        /*
         * TODO
         * Divide effects list into separate lists based on source. i.e:
         *      Player
         *      Item
         *      Enemy
         *      Neutral
         *      Object
         *
         * Then figure out a way to sort the effects into their respective lists, returning an error message if one fails.
         * If the sort fails, keep going through the rest of the sort. Clear the effects list once done.
         */

        public static EffectManager Instance { get; private set; }

        [SerializeField] private GameObject collectedStarShard;
        [SerializeField] private TrailPool trailPool;
        [SerializeField] private List<EffectObject> effects = new();
        private List<EffectObject> playerEffects = new();
        private List<EffectObject> enemyEffects = new();
        private List<EffectObject> itemEffects = new();
        private List<EffectObject> objectEffects = new();
        private List<EffectObject> miscEffects = new();

        private Dictionary<EffectCategory, List<EffectObject>> effectListMap;
        private Dictionary<string, EffectCategory> effectCategoryMap;

        private const string CATEGORY_PLAYER = "p";
        private const string CATEGORY_ENEMY = "e";
        private const string CATEGORY_ITEM = "i";
        private const string CATEGORY_OBJECT = "o";
        private const string CATEGORY_MISC = "m";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            effectListMap = new()
            {
                { EffectCategory.Player, playerEffects },
                { EffectCategory.Enemy, enemyEffects },
                { EffectCategory.Item, itemEffects },
                { EffectCategory.Object, objectEffects },
                { EffectCategory.Misc, miscEffects },
                { EffectCategory.None, new() }
            };
            effectCategoryMap = new()
            {
                { CATEGORY_PLAYER, EffectCategory.Player },
                { CATEGORY_ENEMY, EffectCategory.Enemy },
                { CATEGORY_ITEM, EffectCategory.Item },
                { CATEGORY_OBJECT, EffectCategory.Object },
                { CATEGORY_MISC, EffectCategory.Misc },
            };
            SortEffects();
        }

        private void Start()
        {
            // subscribe to events
            GameEventManager.OnEnemyKill += (v) => SpawnEffect("e_kill", v.EventOrigin);
            GameEventManager.OnEnemyHit += (v) => SpawnEffect("e_hit", v.EventOrigin);
            GameEventManager.OnPlayerHit += (v) => SpawnEffect("p_hit", v.EventTriggerer);
        }

        private void SortEffects()
        {
            foreach (var effect in effects)
            {
                var subList = GetEffectList(effect.Name);
                subList.Add(effect);
            }

            effectListMap[EffectCategory.None].Clear();
        }

        private List<EffectObject> GetEffectList(string effectName)
        {
            return effectListMap[FindMatchingCategory(effectName)];
        }

        /// <summary>
        /// Tries to find a matching category from a string.
        /// </summary>
        /// <param name="effectName">The entire name of the effect.</param>
        /// <returns>The effect's category.</returns>
        private EffectCategory FindMatchingCategory(string effectName)
        {
            string category = SplitEffectName(effectName)[0];

            if (category.Length != 1) return EffectCategory.None; // the category part of the name must be a single character

            return effectCategoryMap.ContainsKey(category) ? effectCategoryMap[category] : EffectCategory.None;
        }

        /// <summary>
        /// Splits the effect name with capital letters as the separator.
        /// </summary>
        /// <param name="effectName">The name of the effect.</param>
        /// <returns>An array of strings of the separated words.</returns>
        private string[] SplitEffectName(string effectName)
        {
            return effectName.Split('_');
        }

        /// <summary>
        /// Spawn an effect and place it somehwere in the game world. The effect will have a rotation of 0 degrees.
        /// </summary>
        /// <param name="effectName">The name of the effect.</param>
        /// <param name="origin">Where to place the effect.</param>
        public void SpawnEffect(string effectName, Vector2 origin)
        {
            Stopwatch sw = new();
            sw.Start();

            EffectID id = GetEffectId(effectName);
            SpawnEffect(id, origin);

            sw.Stop();
            Debug.Log($"Elapsed: {sw.Elapsed}");
        }

        /// <summary>
        /// Spawn an effect under a parent GameObject.
        /// </summary>
        /// <param name="effectName">Name of effect.</param>
        /// <param name="parent">Parent GameObject Transform.</param>
        public void SpawnEffect(string effectName, Transform parent)
        {
            EffectID id = GetEffectId(effectName);
            SpawnEffect(id, parent);
        }

        /// <summary>
        /// Spawn an effect and place it somewhere in the game world with a rotation.
        /// </summary>
        /// <param name="effectName">The name of the effect.</param>
        /// <param name="origin">Where to place the effect.</param>
        /// <param name="rotation">Rotation of the effect.</param>
        public void SpawnEffect(string effectName, Vector2 origin, Quaternion rotation)
        {
            EffectID id = GetEffectId(effectName);
            SpawnEffect(id, origin, rotation);
        }

        public void SpawnEffect(EffectID effectID, Vector2 origin)
        {
            if (effectID.Index < 0 || effectID.Category == EffectCategory.None) return;

            var list = effectListMap[effectID.Category];

            if (effectID.Index > list.Count) return;

            Instantiate(list[effectID.Index].Effect, origin, Quaternion.identity);
        }

        public void SpawnEffect(EffectID effectID, Vector2 origin, Quaternion rotation)
        {
            if (effectID.Index < 0 || effectID.Category == EffectCategory.None) return;

            var list = effectListMap[effectID.Category];

            if (effectID.Index > list.Count) return;

            Instantiate(list[effectID.Index].Effect, origin, rotation);
        }

        public void SpawnEffect(EffectID effectID, Transform parent)
        {
            if (effectID.Index < 0 || effectID.Category == EffectCategory.None) return;

            var list = effectListMap[effectID.Category];

            if (effectID.Index > list.Count) return;

            Instantiate(list[effectID.Index].Effect, parent);
        }

        /// <summary>
        /// Get the ID of an effect.
        /// </summary>
        /// <param name="effectName">Name of effect.</param>
        /// <returns>The ID of the effect; -1 if no such effect exists.</returns>
        public EffectID GetEffectId(string effectName)
        {
            var category = FindMatchingCategory(effectName);

            if (category == EffectCategory.None) return GetNullId();

            var list = effectListMap[category];
            int index = 0;

            if (!list.Select(x => x.Name).Contains(effectName))
            {
                return GetNullId();
            }
            else
            {
                foreach (var item in list)
                {
                    if (item.Name.Equals(effectName)) return new()
                    {
                        Category = category,
                        Index = index
                    };

                    index++;
                }

                return GetNullId();
            }
        }

        private EffectID GetNullId()
        {
            return new()
            {
                Category = EffectCategory.None,
                Index = -1
            };
        }

        /// <summary>
        /// Spawn a trail and place it somewhere in the game world.
        /// </summary>
        /// <returns>The trail spawned.</returns>
        public Trail SpawnTrail(TrailPool.Trails trail, Vector2 origin)
        {
            Trail t = trailPool.TrailsPool[trail].Get();
            t.transform.position = origin;
            return t;
        }

        public void SpawnCollectedStarShard(Vector2 origin, Transform target)
        {
            Instantiate(collectedStarShard, origin, Quaternion.identity).GetComponent<StarFly>().FlyTo(target);
        }
    }
}