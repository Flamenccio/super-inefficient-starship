using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new stage variant", menuName = "Stage Variant", order = 0)]
public class StageVariant : ScriptableObject
{
    public enum variants
    {
        Normal = 0,
        UpperLeftHole = 1,
        UpperRightHole = 2,
        LowerLeftHole = 4,
        LowerRightHole = 3,
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
        [SerializeField] private Directions.directions linkDirection;
        [SerializeField] private List<variants> blackListedVariants;
        public Directions.directions LinkDirection { get { return linkDirection; } }
        public List<variants> BlackListedVariants {  get { return blackListedVariants; } }
    }
    [SerializeField] private Sprite sprite;
    [SerializeField] private WallLayout secondaryWallLayout;
    [SerializeField] private List<LinkSet> linkSet = new List<LinkSet>();
    [SerializeField] private variants variant;
    public Sprite Sprite { get { return sprite; } }
    public WallLayout SecondaryWallLayout { get { return secondaryWallLayout; } }
    public List<LinkSet> Links { get { return linkSet; } }
    public variants Variant {  get { return variant; } }
}
