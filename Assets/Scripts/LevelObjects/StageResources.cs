using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Stuff used to spawn and build a stage.
    /// </summary>
    public class StageResources : MonoBehaviour
    {
        public static StageResources Instance { get; private set; }
        public List<StageVariant.Variants> AllVariants { get; private set; }
        public List<StageVariant> StageVariants { get => stageVariants; }
        private List<StageVariant> stageVariants = new();
        [SerializeField] private GameObject stagePrefab;

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

            AllVariants = new(System.Enum.GetValues(typeof(StageVariant.Variants)).Cast<StageVariant.Variants>());
            stageVariants.AddRange(Resources.LoadAll<StageVariant>("StageVariants")); // load all stagevariants here
            stageVariants.Sort((a, b) => (int)a.Variant < (int)b.Variant ? -1 : 1); // sort variants by variant
        }

        /// <summary>
        /// Returns a StageVariant matching the variant given.
        /// </summary>
        public StageVariant GetStageVariant(StageVariant.Variants variant)
        {
            foreach (StageVariant sv in stageVariants)
            {
                if (sv.Variant == variant) return sv;
            }
            return null;
        }

        /// <summary>
        /// Does the given variant have an existing link in given direction?
        /// </summary>
        public bool HasLinkInDirection(StageVariant.Variants variant, Directions.CardinalValues direction)
        {
            foreach (StageVariant.LinkSet ls in stageVariants[(int)variant].Links)
            {
                if (ls.LinkDirection == direction)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrives a list of stage variants that can be extended in the given direction.
        /// </summary>
        public List<StageVariant.Variants> GetVariantsExtendableInDirection(Directions.CardinalValues direction)
        {
            List<StageVariant.Variants> v = new();
            StageVariants
                .Where(variant => HasLinkInDirection(variant.Variant, direction))
                .ToList()
                .ForEach(variant => v.Add(variant.Variant));

            return v;
        }

        /// <summary>
        /// Returns a list of available stage variants that may extend from a root stage in the given direction.
        /// </summary>
        public List<StageVariant.Variants> GetAvailableVariantsInDirection(Directions.CardinalValues direction, StageVariant.Variants rootVariant)
        {
            List<StageVariant.Variants> blacklisted = new(GetStageVariant(rootVariant).Links.First(v => v.LinkDirection == direction).BlackListedVariants); // copy blacklisted variants of stage link in chosen direction

            return new(GetVariantsExtendableInDirection(Directions.OppositeOf(direction)).Except(blacklisted)); // basically, find all stage variants that can extend in the opposite direction of localSpawnDirection.Direction and then remove variants blacklisted by the roots variant.
        }

        /// <summary>
        /// Creates an instance of a stage with the given variant and returns its instance..
        /// </summary>
        /// <param name="variant">Variant of new stage.</param>
        /// <returns>Instance of new stage.</returns>
        public Stage CreateStage(StageVariant.Variants variant)
        {
            var instance = Instantiate(stagePrefab, gameObject.transform).GetComponent<Stage>();
            instance.UpdateVariant(variant);
            return instance;
        }
    }
}