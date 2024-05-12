using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    public class StageResources : MonoBehaviour
    {
        public static StageResources Instance { get; private set; }
        public List<StageVariant.Variants> AllVariants { get; private set; }
        public List<StageVariant> StageVariants { get => stageVariants; }
        private List<StageVariant> stageVariants = new();
        private void Start()
        {
        }
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
        public bool HasLinkInDirection(StageVariant.Variants variant, Directions.CardinalValues dir)
        {
            foreach (StageVariant.LinkSet ls in stageVariants[(int)variant].Links)
            {
                if (ls.LinkDirection == dir)
                {
                    return true;
                }
            }
            return false;
        }
        public List<StageVariant.Variants> GetVariantsExtendableInDirection(Directions.CardinalValues dir)
        {
            List<StageVariant.Variants> v = new();
            StageVariants
                .Where(variant => HasLinkInDirection(variant.Variant, dir))
                .ToList()
                .ForEach(variant => v.Add(variant.Variant));

            return v;
        }
    }
}
