using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    [CreateAssetMenu(fileName = "new stage variant", menuName = "Stage Variant", order = 0)]
    public class StageVariant : ScriptableObject
    {
        public Sprite Sprite { get { return sprite; } }
        public WallLayout SecondaryWallLayout { get { return secondaryWallLayout; } }
        public List<LinkSet> Links { get { return linkSet; } }
        public Variants Variant { get { return variant; } }
        public enum Variants
        {
            Normal = 0,
            UpperLeftHole = 1,
            UpperRightHole = 2,
            LowerRightHole = 3,
            LowerLeftHole = 4,
            CenterHole = 5,
            Cross = 6,
            ChokepointNorth = 7,
            ChokepointSouth = 8,
            ChokepointEast = 9,
            ChokepointWest = 10,
        }
        [System.Serializable]
        public struct LinkSet
        {
            [SerializeField] private Directions.CardinalValues linkDirection;
            [SerializeField] private List<Variants> blackListedVariants;
            public Directions.CardinalValues LinkDirection { get { return linkDirection; } }
            public List<Variants> BlackListedVariants { get { return blackListedVariants; } }
        }
        [SerializeField] private Sprite sprite;
        [SerializeField] private WallLayout secondaryWallLayout;
        [SerializeField] private List<LinkSet> linkSet = new();
        [SerializeField] private Variants variant;
    }
}
