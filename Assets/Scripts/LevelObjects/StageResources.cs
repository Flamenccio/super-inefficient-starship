using Flamenccio.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Used to tell stages how to build.
    /// </summary>
    public struct StageProperties
    {
        public Directions.CardinalValues Direction;
        public List<StageVariant.LinkSet> LinkSet;
        public Sprite Sprite;
        public WallLayout WallLayout;
    }

    /// <summary>
    /// Stuff used to spawn and build a stage.
    /// </summary>
    public class StageResources : MonoBehaviour
    {
        public static StageResources Instance { get; private set; }
        [SerializeField] private GameObject stagePrefab;
        private Dictionary<string, Dictionary<Directions.CardinalValues, List<string>>> stageBlacklists = new();
        private Dictionary<string, StageProperties> stageProperties = new();

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

            LoadVariants(stageProperties);
            LoadAllBlacklists(stageBlacklists);
        }

        private void LoadVariants(Dictionary<string, StageProperties> output)
        {
            var load = Resources.LoadAll<StageVariant>("StageVariants");

            foreach (var variant in load)
            {

                // If there are no rotation variants, just add the base shape.
                if (variant.DoNotRotate)
                {
                    StageProperties properties = new()
                    {
                        Direction = Directions.CardinalValues.North,
                        Sprite = variant.Sprite,
                        LinkSet = CopyLinkSet(variant.Links),
                        WallLayout = variant.SecondaryWallLayout
                    };
                    output.Add(variant.VariantId, properties);

                    continue;
                }

                // Otherwise, add each of the rotation variants.
                // The base shape (the value in the dictionary) is added.
                // The actual rotation will be calculated when needed.
                RotateStage(variant, output);
            }
        }

        /// <summary>
        /// Takes a stage variant and provides all directional variants.
        /// </summary>
        /// <param name="variant">Variant to rotate.</param>
        /// <param name="output">Dictionary to output results to.</param>
        private void RotateStage(StageVariant variant, Dictionary<string, StageProperties> output)
        {
            if (variant.DoNotRotate) return;

            for (int i = 0; i < 4; i++)
            {
                string name = $"{variant.VariantId}_{i}";
                List<StageVariant.LinkSet> newLinks = CopyLinkSet(variant.Links);

                for (int j = 0; j < newLinks.Count; j++)
                {
                    var link = newLinks[j];
                    bool directionPolarity = DirectionPolarity(link.LinkDirection);
                    var newDirection = Directions.IntToDirection((int)link.LinkDirection + i);
                    var newSubLinkMask = link.SubLinkMask;
                    var subLinkPositions = link.SubLinkPositions;

                    if (!directionPolarity.Equals(DirectionPolarity(newDirection)))
                    {
                        newSubLinkMask = ReverseBinary(link.SubLinkMask);
                        subLinkPositions = CreateSubLinks(newSubLinkMask, link.InvertSubLinkPositions);
                    }

                    newLinks[j] = new()
                    {
                        SubLinkPositions = subLinkPositions,
                        InvertSubLinkPositions = link.InvertSubLinkPositions,
                        LinkDirection = newDirection
                    };
                }

                StageProperties properties = new()
                {
                    Direction = Directions.IntToDirection(i),
                    Sprite = variant.Sprite,
                    WallLayout = variant.SecondaryWallLayout,
                    LinkSet = newLinks
                };

                output.Add(name, properties);
            }
        }

        /// <summary>
        /// Returns a deep copy of the given list of linksets.
        /// </summary>
        /// <param name="original">Original list to copy.</param>
        private List<StageVariant.LinkSet> CopyLinkSet(List<StageVariant.LinkSet> original)
        {
            List<StageVariant.LinkSet> copy = new();

            foreach (var link in original)
            {
                StageVariant.LinkSet linkCopy = new()
                {
                    LinkDirection = link.LinkDirection,
                    SubLinkPositions = new(link.SubLinkPositions),
                    InvertSubLinkPositions = link.InvertSubLinkPositions
                };
                copy.Add(linkCopy);
            }

            return copy;
        }

        /// <summary>
        /// Generates a list of sublinks from a given mask.
        /// </summary>
        private List<int> CreateSubLinks(int mask, bool invert)
        {
            List<int> set = new();

            for (int i = 0; i < 8; i++)
            {
                int bit = (1 << i) & mask;

                if (invert)
                {
                    if (bit == 0)
                    {
                        set.Add(i);
                    }
                }
                else if (bit != 0)
                {
                    set.Add(i);
                }
            }

            return set;
        }

        private void LoadAllBlacklists(Dictionary<string, Dictionary<Directions.CardinalValues, List<string>>> blacklists)
        {
            foreach (var x in stageProperties)
            {
                string currentName = x.Key;
                blacklists.Add(currentName, LoadBlacklist(currentName));
            }
        }

        private Dictionary<Directions.CardinalValues, List<string>> LoadBlacklist(string stage)
        {
            if (!stageProperties.TryGetValue(stage, out var property)) return new();

            Dictionary<Directions.CardinalValues, List<string>> newDict = new();
            property.LinkSet
                .ForEach(x => newDict.Add(x.LinkDirection, GetBlacklistedVariants(property, x.LinkDirection)));

            return newDict;
        }

        private int ReverseBinary(int value)
        {
            value = Mathf.Abs(value);
            int reversed = 0;
            int maxPower = 7; // Works only for 8-bit integers

            //for (maxBits = 0; Mathf.Pow(2f, maxBits) < value; maxBits++) ;

            for (int n = 0; n <= maxPower; n++)
            {
                int bit = ((1 << n) & value) == 0 ? 0 : 1;
                reversed |= (bit << (maxPower - n));
            }

            return reversed;
        }

        private bool DirectionPolarity(Directions.CardinalValues direction)
        {
            return (int)direction <= 2;
        }

        /// <summary>
        /// Does the given variant have an existing link in given direction?
        /// </summary>
        private bool HasLinkInDirection(string variant, Directions.CardinalValues direction)
        {
            if (!stageProperties.TryGetValue(variant, out var stageVariant)) return false;

            var b = stageVariant.LinkSet.Exists(x => x.LinkDirection.Equals(direction));

            return b;
        }

        /// <summary>
        /// Retrives a list of stage variants IDs that can be extended in the given direction.
        /// </summary>
        private List<string> GetVariantsExtendableInDirection(Directions.CardinalValues direction)
        {
            List<string> v = new();

            stageProperties
                .Select(element => element.Key)
                .Where(variant => HasLinkInDirection(variant, direction))
                .ToList()
                .ForEach(variant => v.Add(variant));

            return v;
        }


        /// <summary>
        /// Get blacklisted stage variants of given root in given direction.
        /// </summary>
        /// <param name="properties">The root stage variant.</param>
        /// <param name="direction">Direction from root stage to find blacklisted variants.</param>
        /// <returns>List of stage variants ids.</returns>
        private List<string> GetBlacklistedVariants(StageProperties properties, Directions.CardinalValues direction)
        {
            List<string> blacklisted = new();

            if (!properties.LinkSet.Exists(v => v.LinkDirection.Equals(direction)))
            {
                return stageProperties.Keys.ToList();
            }

            var link = properties.LinkSet.Find(x => x.LinkDirection == direction);
            var mask = link.SubLinkMask;
            var oppositeDirection = direction.OppositeOf(false);
            stageProperties
                .Select(x => (Name: x.Key, Property: x.Value))
                .ToList()
                .ForEach(x =>
                {
                    if (!HasLinkInDirection(x.Name, oppositeDirection))
                    {
                        blacklisted.Add(x.Name);
                    }
                    else
                    {
                        var set = x.Property.LinkSet.Find(y => y.LinkDirection.Equals(oppositeDirection));

                        if ((set.SubLinkMask & mask) == 0)
                        {
                            blacklisted.Add(x.Name);
                        }
                    }
                });

            return blacklisted;
        }

        /// <summary>
        /// Returns a list of available stage variants that may extend from a root stage in the given direction.
        /// </summary>
        public List<string> GetAvailableVariantsInDirection(Directions.CardinalValues direction, string rootVariantId)
        {
            if (!stageProperties.TryGetValue(rootVariantId, out var property)) return new();

            return new(
                GetVariantsExtendableInDirection(direction.OppositeOf(false))
                .Except(stageBlacklists[rootVariantId][direction])
                ); // basically, find all stage variants that can extend in the opposite direction of localSpawnDirection.Direction and then remove variants blacklisted by the roots variant.
        }

        /// <summary>
        /// Creates an instance of a stage with the given variant and returns its instance..
        /// </summary>
        /// <param name="variantId">Variant of new stage.</param>
        /// <returns>Instance of new stage.</returns>
        public Stage CreateStage(string variantId)
        {
            if (!stageProperties.TryGetValue(variantId, out var _))
            {
                Debug.LogError($"Stage variant {variantId} does not exist.");
                return null;
            }

            var instance = Instantiate(stagePrefab, gameObject.transform).GetComponent<Stage>();
            instance.UpdateVariant(variantId);
            return instance;
        }

        public StageProperties GetStageProperties(string variantId)
        {
            if (!stageProperties.TryGetValue(variantId, out var x)) x = new();

            return x;
        }
    }
}