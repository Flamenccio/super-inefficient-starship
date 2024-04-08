using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageSharedResources : MonoBehaviour
{
    // a class that contains all resources that stages will have access too

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    private List<WallLayout> invisibleWallLayouts = new List<WallLayout>();
    private List<StageLayout> stageLayouts = new List<StageLayout>();
    private List<StageVariant> variants = new List<StageVariant>();
    [SerializeField] private GameObject invisibleWall;
    [SerializeField] private LayerMask invisibleWallLayer;
    Directions directions = new Directions();

    public List<Sprite> Sprites { get => sprites; }
    public List<WallLayout> InvisibleWallLayouts { get => invisibleWallLayouts; }
    public List<StageVariant> Variants { get => variants; }
    public GameObject InvisibleWall { get => invisibleWall; }
    public Directions Directions { get => directions;}
    public LayerMask InvisibleWallLayer { get => invisibleWallLayer; }
    private void Awake()
    {
        StageLayout[] temp = Resources.LoadAll<StageLayout>("Stage Layouts");
        foreach (StageLayout SL in temp)
        {
            stageLayouts.Add(SL);
            //Debug.Log(SL);
        }

        WallLayout[] temp2 = Resources.LoadAll<WallLayout>("Wall Layouts");
        foreach (WallLayout IWL in temp2)
        {
            invisibleWallLayouts.Add(IWL);
        }

        StageVariant[] temp3 = Resources.LoadAll<StageVariant>("Stage Variants");
        foreach (StageVariant SV in temp3)
        {
            variants.Add(SV);
        }
    }
    public StageLayout StageLayout(StageControl.variants n)
    {
        foreach (StageLayout SL in stageLayouts)
        {
            if (SL.Variant.Equals(n))
            {
                return SL;
            }
        }
        return null;
    }
    public StageVariant StageVariant(StageVariant.variants v)
    {
        foreach (StageVariant SV in variants)
        {
            if (SV.Variant.Equals(v))
            {
                return SV;
            }
        }
        return null;

    }
}
