using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageControl : MonoBehaviour // UNUSED
{
    /*
    // types of stage variants
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
    }

    // this particular stage's variant
    private variants stageVariant;

    // get resources from other class
    private StageSharedResources sharedResources = null;
    private Directions directionsLibrary = new Directions();

    // prefabs
    [SerializeField] private GameObject invisibleWallPrefab;

    // other components attached to this gameObject
    private PolygonCollider2D polyCollider;
    private SpriteRenderer spriteRen;
    private Mesh polymesh;
    private Bounds polybounds;
    private StageLayout layout;

    // stages that are connected to the north, east, south, or west of this stage
    [SerializeField] private StageControl[] connectedStages = new StageControl[4];
    [SerializeField] private SecondaryWall[] primaryWalls = new SecondaryWall[4];

    // this stage's ID (default value is 1)
    public int stageId = 1;

    public variants StageVariant { get => stageVariant; }

    private void Awake()
    {
        // if this stage is an additional stage, initialize it once it's instanced
        if (stageId != 0) Init();
    }
    private void Start()
    {
        // if this stage is the starting stage, initialize it a little later than Awake() is called
        if (stageId == 0)
        {
            Init();

            stageVariant = variants.Normal;
            layout = sharedResources.StageLayout(stageVariant);
            SpawnInvisibleWalls();

            polymesh = polyCollider.CreateMesh(true, true);
            polybounds = polymesh.bounds;
        }
    }
    private void Init()
    {
        sharedResources = gameObject.GetComponentInParent<StageSharedResources>();
        spriteRen = gameObject.GetComponent<SpriteRenderer>();
        polyCollider = gameObject.GetComponent<PolygonCollider2D>();
    }
    public void UpdateVariant(variants variant)
    {
        Debug.Log("NEW STAGE: " + variant);

        // update the sprite
        stageVariant = variant;
        layout = sharedResources.StageLayout(stageVariant);
        spriteRen.sprite = layout.Sprite;

        // reshape the polygon collider and update mesh and bounds
        Destroy(polyCollider);
        Destroy(polymesh);
        polyCollider = gameObject.AddComponent<PolygonCollider2D>();
        polymesh = polyCollider.CreateMesh(true, true);
        polybounds = polymesh.bounds;
        polyCollider.isTrigger = true;
    }
    public bool ExtendStage(StageControl newStage, int direction)
    {
        // force the value of direction to be between 0-3 (inclusive)
        int directionModified = direction % 4;

        if (!IsExtendable(newStage, directionsLibrary.IntToDirection(directionModified))) return false;

        // add the given stage to this stage's specified direction and return true
        connectedStages[directionModified] = newStage;
        return true;
    }
    /// <summary>
    /// Only use if either:
    /// You've already checked if a stage extension is legal before this is run OR
    /// You know what you're doing
    /// </summary>
    public void ExtendStageUnsafe(StageControl newStage, int direction)
    {
        direction = direction % 4;
        connectedStages[direction] = newStage;
    }
    public void ExtendStageUnsafe(StageControl newStage, Directions.directions direction)
    {
        ExtendStageUnsafe(newStage, directionsLibrary.DirectionToInt(direction));
    }
    public bool IsExtendable(StageControl newStage, Directions.directions direction)
    {
        // is the new stage being added to one of this stage's inaccessible directions?
        foreach (Directions.directions dir in layout.InaccessibleDirections)
        {
            if (direction == dir)
            {
                return false;
            }
        }

        // is there already a stage that's conneceted in the same direction?
        if (connectedStages[(int)direction] != null) return false;

        // is the selected stage this one?
        if (newStage.gameObject.GetInstanceID().Equals(this.gameObject.GetInstanceID())) return false;

        // is the new stage incompatible in the specific direction?
        foreach (StageLayout.variantBlackList blackListed in layout.IncompatibleVariants)
        {
            if (blackListed.Variant == newStage.stageVariant && blackListed.Direction == direction)
            {
                return false;
            }
        }

        return true;
    }
    public bool ExtendStage(StageControl stage, Directions.directions direction)
    {
        return ExtendStage(stage, (int)direction);
    }
    public bool HasSpaceAt(int direction)
    {
        return (connectedStages[direction] == null);
    }
    public bool HasSpaceAt(Directions.directions direction)
    {
        return HasSpaceAt((int)direction);
    }
    public void SpawnInvisibleWalls()
    {
        // spawn the base (primary) walls and store them into a list
        for (int i = 0; i < 4; i++)
        {
            if (primaryWalls[i] != null) continue;
            if (connectedStages[i] != null) continue;
            SecondaryWall instance = Instantiate(invisibleWallPrefab, transform).GetComponent<SecondaryWall>();
            Debug.Log(sharedResources.StageLayout(variants.Normal));
            CopyWallAttributes(instance, sharedResources.StageLayout(variants.Normal).WallLayout.Layout[i]);
            instance.UpdateAttributes();
            primaryWalls[i] = instance;
        }

        // spawn every secondary wall based on the stageVariant
        if (stageVariant > variants.Normal)
        {
            for (int i = 0; i < layout.WallLayout.Layout.Count; i++)
            {
                SecondaryWall instance = Instantiate(sharedResources.InvisibleWall, transform).GetComponent<SecondaryWall>();
                CopyWallAttributes(instance, layout.WallLayout.Layout[i]);
                instance.UpdateAttributes();
            }
        }
    }
    public float GetExtentX()
    {
        return polybounds.extents.x;
    }
    public float GetExtentY()
    {
        return polybounds.extents.y;
    }
    private void CopyWallAttributes(SecondaryWall wall, WallLayout.invisibleWallAttributes attributes)
    {
        wall.relativePosition = attributes.Position;
        wall.xSize = attributes.XSize;
        wall.ySize = attributes.YSize;
    }
    public void RemoveInvisibleWalls()
    {
        int i = 0;
        foreach(SecondaryWall wall in primaryWalls)
        {
            if (wall != null && connectedStages[i] != null)
            {
                RemoveInvisibleWall(i);
            }
            i++;
        }
    }
    public void RemoveInvisibleWall(int dir)
    {
        int dirModified = dir % 4;
        GameObject remove = primaryWalls[dirModified].gameObject;
        primaryWalls[dirModified] = null;
        Destroy(remove);
    }
    */
}
