using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;
using System.Linq;
using System.Text;

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

        [System.Serializable]
        public struct LinkSet
        {
            [SerializeField] private Directions.CardinalValues linkDirection;
            [SerializeField, Tooltip("Range 0-7")] private List<int> subLinkPositions;
            [SerializeField, Tooltip("With this toggled, assume that this link has all sublinks except the ones listed.")] private bool invertSubLinkPositions;
            public Directions.CardinalValues LinkDirection { get => linkDirection; }
            public List<int> SubLinkPositions { get => subLinkPositions; }
            public int SubLinkMask { get => GetSubLinkMask(SubLinkPositions, invertSubLinkPositions); }
        }

        public static int GetSubLinkMask(List<int> sublinks, bool isInverted)
        {
            // FIXME broken
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

            Debug.Log($"Mask: {ConvertBinary(mask)}");
            return mask;
        }

        private static string ConvertBinary(int number)
        {
            StringBuilder sb = new("", 12);

            while (number > 0)
            {
                int r = number % 2;
                number = Mathf.FloorToInt(number / 2f);
                sb.Append(r);
            }

            return sb.ToString();
        }

        [SerializeField] private Sprite sprite;
        [SerializeField] private WallLayout secondaryWallLayout;
        [SerializeField] private List<LinkSet> linkSet = new();
        [SerializeField] private string variantId;
        private const int MAX_SUBLINKS = 8;
    }
}