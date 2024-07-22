using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{

    /// <summary>
    /// A "blueprint" that stores information for spawning a stage.
    /// </summary>
    [CreateAssetMenu(fileName = "new stage variant", menuName = "Stage Variant", order = 0)]
    public class StageVariant : ScriptableObject
    {
        public Sprite Sprite { get => sprite; }
        public WallLayout SecondaryWallLayout { get => secondaryWallLayout; }
        public List<LinkSet> Links { get => linkSet; }
        public string VariantId => variantId;
        public bool DoNotRotate => doNotRotate;

        [System.Serializable]
        public struct LinkSet
        {
            [SerializeField, Tooltip("Range 0-7")] public List<int> SubLinkPositions;
             public bool InvertSubLinkPositions { get => invertSubLink; set => invertSubLink = value; }

            public Directions.CardinalValues LinkDirection
            {
                get { return linkDirection; }
                set { linkDirection = value; }
            }
            public int SubLinkMask { get => GetSubLinkMask(SubLinkPositions, InvertSubLinkPositions); }
            [SerializeField, Tooltip("With this toggled, assume that this link has all sublinks except the ones listed.")] private bool invertSubLink;
            [SerializeField] private Directions.CardinalValues linkDirection;
        }

        [SerializeField] private Sprite sprite;
        [SerializeField] private WallLayout secondaryWallLayout;
        [SerializeField] private List<LinkSet> linkSet = new();
        [SerializeField] private string variantId;
        [SerializeField, Tooltip("Should this stage variant be rotated to make 4 rotated variants?")] private bool doNotRotate = false;

        private static int GetSubLinkMask(List<int> sublinks, bool isInverted)
        {
            int mask = isInverted ? 255 : 0;

            for (int i = 0; i < MAX_SUBLINKS && i < sublinks.Count; i++)
            {
                if (sublinks[i] < 0 || sublinks[i] > MAX_SUBLINKS - 1) continue;

                int currentBit = 1 << sublinks[i];

                if (isInverted)
                {
                    currentBit = ~currentBit;
                    mask &= currentBit;
                }
                else
                {
                    mask |= currentBit;
                }
            }

            return mask;
        }

        private const int MAX_SUBLINKS = 8;
    }
}