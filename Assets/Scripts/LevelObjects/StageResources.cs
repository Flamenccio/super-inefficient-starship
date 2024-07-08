using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flamenccio.Utility;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Stuff used to spawn and build a stage.
    /// </summary>
    public class StageResources : MonoBehaviour
    {
        public static StageResources Instance { get; private set; }
        public Dictionary<string, StageVariant> AllStageVariants { get => allStageVariants; }
        [SerializeField] private GameObject stagePrefab;
        private Dictionary<string, StageVariant> allStageVariants = new();
        private Dictionary<string, Dictionary<Directions.CardinalValues, List<string>>> stageBlacklists = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }

            LoadVariants(allStageVariants);
            LoadAllBlacklists(stageBlacklists);
        }

        private void LoadVariants(Dictionary<string, StageVariant> dictionary)
        {
            var load = Resources.LoadAll<StageVariant>("StageVariants");

            foreach (var x in load)
            {
                dictionary.Add(x.VariantId, x);
            }
        }

        private void LoadAllBlacklists(Dictionary<string, Dictionary<Directions.CardinalValues, List<string>>> blacklists)
        {
            Stopwatch sw = new();
            sw.Start();

            foreach (var x in AllStageVariants)
            {
                string currentName = x.Value.VariantId;
                blacklists.Add(currentName, LoadBlacklist(currentName));
            }

            sw.Stop();
            Debug.Log($"TIME: {sw.Elapsed.TotalMilliseconds}ms");
        }

        private  Dictionary<Directions.CardinalValues, List<string>> LoadBlacklist(string stage)
        {
            if (!AllStageVariants.TryGetValue(stage, out var variant)) return new();

            Dictionary<Directions.CardinalValues, List<string>> newDict = new();

            variant.Links
                .ForEach(x => newDict.Add(x.LinkDirection, GetBlacklistedVariants(variant, x.LinkDirection)));

            return newDict;
        }

        public StageVariant GetStageVariant(string variant)
        {
            return AllStageVariants.Values.ToList().Find(x => x.VariantId.Equals(variant));
        }

        /// <summary>
        /// Does the given variant have an existing link in given direction?
        /// </summary>
        public bool HasLinkInDirection(string variant, Directions.CardinalValues direction)
        {
            if (!AllStageVariants.TryGetValue(variant, out var stageVariant)) return false;

            return stageVariant.Links
                .Exists(x => x.LinkDirection.Equals(direction));
        }

        /// <summary>
        /// Retrives a list of stage variants IDs that can be extended in the given direction.
        /// </summary>
        public List<string> GetVariantsExtendableInDirection(Directions.CardinalValues direction)
        {
            List<string> v = new();
            AllStageVariants.Values
                .Where(variant => HasLinkInDirection(variant.VariantId, direction))
                .ToList()
                .ForEach(variant => v.Add(variant.VariantId));

            return v;
        }

        /// <summary>
        /// Returns a list of available stage variants that may extend from a root stage in the given direction.
        /// </summary>
        public List<string> GetAvailableVariantsInDirection(Directions.CardinalValues direction, string rootVariantId)
        {
            if (!AllStageVariants.TryGetValue(rootVariantId, out var variant)) return new();

            return new(
                GetVariantsExtendableInDirection(Directions.OppositeOf(direction))
                .Except(stageBlacklists[variant.VariantId][direction])
                ); // basically, find all stage variants that can extend in the opposite direction of localSpawnDirection.Direction and then remove variants blacklisted by the roots variant.
        }

        public List<string> GetBlacklistedVariants(StageVariant stageVariant, Directions.CardinalValues direction)
        {
            List<string> blacklisted = new();

            if (!stageVariant.Links.Exists(v => v.LinkDirection == direction)) return AllStageVariants.Keys.ToList();

            var link = stageVariant.Links.Find(x => x.LinkDirection == direction);
            var mask = link.SubLinkMask;
            var oppositeDirection = Directions.OppositeOf(direction);
            AllStageVariants.Values
                .ToList()
                .ForEach(x =>
                {
                    if (!HasLinkInDirection(x.VariantId, oppositeDirection))
                    {
                        blacklisted.Add(x.VariantId); // if a variant can't connect, blacklist it
                    }
                    else
                    {
                        var l = x.Links.Find(x => x.LinkDirection == oppositeDirection);

                        if ((l.SubLinkMask & mask) == 0)
                        {
                            blacklisted.Add(x.VariantId);
                        }
                    }
                });

            return blacklisted;
        }

        /// <summary>
        /// Creates an instance of a stage with the given variant and returns its instance..
        /// </summary>
        /// <param name="variantId">Variant of new stage.</param>
        /// <returns>Instance of new stage.</returns>
        public Stage CreateStage(string variantId)
        {
            if (!AllStageVariants.TryGetValue(variantId, out var _))
            {
                Debug.LogError($"Stage variant {variantId} does not exist.");
                return null;
            }

            var instance = Instantiate(stagePrefab, gameObject.transform).GetComponent<Stage>();
            instance.UpdateVariant(variantId);
            return instance;
        }
    }
}