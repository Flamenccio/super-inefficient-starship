using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Stage Layout", menuName = "Stage Layout", order = 1)]
public class StageLayout : ScriptableObject // UNUSED
{
    /*
    [System.Serializable]
    public struct variantBlackList
    {
        [SerializeField] private StageControl.variants blackListVariant;
        [SerializeField] private Directions.directions direction;
        public StageControl.variants Variant { get => blackListVariant; }
        public Directions.directions Direction { get => direction; }
    }
    // this will be used ON TOP of the base layout
    [Tooltip("This layout will be used ON TOP of the base layout.")]
    [SerializeField] private WallLayout wallLayout;
    [Tooltip("What the stage will look like.")]
    [SerializeField] private Sprite sprite;
    [Tooltip("The list of variants that cannot connect to this wall in the specified direction.")]
    [SerializeField] private List<variantBlackList> incomptibleVariants = new List<variantBlackList>();
    [Tooltip("The directions that are inaccessible to all stage variants.")]
    [SerializeField] private List<Directions.directions> inaccessibleDirections = new List<Directions.directions>();
    [Tooltip("The stage layout that this layout represents.")]
    [SerializeField] private StageControl.variants variant = StageControl.variants.Normal;

    public WallLayout WallLayout { get => wallLayout; }
    public Sprite Sprite { get => sprite; }
    public List<variantBlackList> IncompatibleVariants { get => incomptibleVariants; }
    public List<Directions.directions> InaccessibleDirections { get => inaccessibleDirections; }
    public StageControl.variants Variant { get => variant; }
    public variantBlackList IncompatibleVariant(int n)
    {
        return incomptibleVariants[n];
    }
    */
}
