using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    public class StageResources : MonoBehaviour
    {
        //[SerializeField] private Stage mainScript; // the Stage script attached to this gameobject
        private static List<StageVariant> stageVariants = new List<StageVariant>();
        public static StageResources Instance { get; private set; }
        public List<StageVariant.Variants> AllVariants { get; private set; }
        public List<StageVariant> StageVariants { get => stageVariants; }
        private void Start()
        {
        }
        private void Awake()
        {
            AllVariants = new(System.Enum.GetValues(typeof(StageVariant.Variants)).Cast<StageVariant.Variants>());

            if (Instance != null) Destroy(Instance);

            Instance = this;

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
        public bool HasLinkInDirection(StageVariant.Variants variant, Directions.directions dir)
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
        public List<StageVariant.Variants> GetVariantsExtendableInDirection(Directions.directions dir)
        {
            List<StageVariant.Variants> v = new();
            foreach (StageVariant variant in stageVariants)
            {
                if (HasLinkInDirection(variant.Variant, dir)) v.Add(variant.Variant);
            }
            return v;
        }
    }
}
