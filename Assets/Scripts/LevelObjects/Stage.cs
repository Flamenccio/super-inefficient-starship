using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRen; // sprite renderer
    [SerializeField] private GameObject invisibleWallPrefab; // prefab for invisible walls
    [SerializeField] private GameObject stageLinkPrefab; // prefab for stage links
    [SerializeField] private bool initialStage; // is this stage the first one in the level?
    [SerializeField] private StageResources resources; // class that provides access to common stage resources

    private StageVariant.variants variant = StageVariant.variants.Normal; // initialize the stage as a normal variant
    private Sprite sprite; // sprite representing the stage
    private Dictionary<Directions.directions, StageLink> links = new Dictionary<Directions.directions, StageLink>(); // a list of all stage links associated with their direction
    private PolygonCollider2D polyCollider; // collider that conforms to sprite shape
    private Mesh polymesh; // mesh for poly collider

    public StageVariant.variants Variants { get { return variant; } }
    public Sprite Sprite { get { return sprite; } }
    public bool InitialStage { get { return initialStage; } }

    private void Start()
    {
        // normal variant by default
        if (initialStage)
        {
            //UpdateVariant(StageVariant.variants.Normal);
        }
    }
    private void Awake()
    {
        polyCollider = gameObject.GetComponent<PolygonCollider2D>();
        polymesh = polyCollider.CreateMesh(true, true);
    }
    // this function is called when another stage wants to connect to this one.
    // in order for a connection to work, the extender must request a "handshake" to the extendee.
    // the connection only happens if both extender and extendee accept the handshake. 
    /// <summary>
    /// Request an extension handshake to this stage.
    /// </summary>
    /// <param name="dir">Direction to extend in.</param>
    /// <param name="stage">Stage to extend to.</param>
    /// <returns></returns>
    public bool Handshake(Directions.directions dir, Stage stage)
    {
        if (!LinkableInDirection(dir, stage.variant))
        {
            return false; // if a stage link is not available in given direction, return false.
        }
        links.TryGetValue(dir, out StageLink s);
        if (s != null)
        {
            return s.IsVacant(); // if the stage link exists in given direction, let the stage link decide. 
        }
        else
        {
            return false; // otherwise, not linkable in direction.
        }
    }
    public bool Extend(Directions.directions dir, Stage stage)
    {
        Directions d = new Directions();
        if (!Handshake(dir, stage) || !stage.Handshake(d.OppositeOf(dir), this))// check if both this and stage's handshakes are true
        {
            return false;
        }
        // link two stages.
        LinkStageUnsafe(dir, stage);
        stage.LinkStageUnsafe(d.OppositeOf(dir), this);
        return true;
    }
    public void UpdateVariant(StageVariant.variants updatedVariant)
    {
        variant = updatedVariant;
        StageVariant v = resources.GetStageVariant(variant);
        sprite = v.Sprite;
        spriteRen.sprite = sprite;

        // update polygon mesh
        Destroy(polyCollider);
        Destroy(polymesh);
        polyCollider = gameObject.AddComponent<PolygonCollider2D>();
        polymesh = polyCollider.CreateMesh(true, true);
        polyCollider.isTrigger = true;
        CommitUpdate();
    }

    /// <summary>
    /// Commit the variant update and spawn walls and links.
    /// </summary>
    private void CommitUpdate()
    {
        StageVariant v = resources.GetStageVariant(variant); // retrieve the variant's template

        // spawn secondary walls
        int i = 0;
        if (v.SecondaryWallLayout != null)
        {
            foreach (var invisibleWallConfig in  v.SecondaryWallLayout.Layout)
            {
                SecondaryWall instance = Instantiate(invisibleWallPrefab, transform).GetComponent<SecondaryWall>();
                CopyWallAttributes(instance, invisibleWallConfig);
                instance.UpdateAttributes();
                i++;
            }
        }

        // spawn and place stage links (primary walls)
        foreach (var link in v.Links)
        {
            StageLink instance = Instantiate(stageLinkPrefab, transform).GetComponent<StageLink>();
            instance.UpdateProperties(link.BlackListedVariants, link.LinkDirection);

            links.Add(link.LinkDirection, instance);

            switch (link.LinkDirection)
            {
                case Directions.directions.North:
                    instance.Place(new Vector2(0, 9));
                    instance.SpawnWall(PrimaryWall.orientation.Horizontal);
                    break;

                case Directions.directions.East:
                    instance.Place(new Vector2(9, 0));
                    instance.SpawnWall(PrimaryWall.orientation.Vertical);
                    break;

                case Directions.directions.South:
                    instance.Place(new Vector2(0, -9));
                    instance.SpawnWall(PrimaryWall.orientation.Horizontal);
                    break;

                case Directions.directions.West:
                    instance.Place(new Vector2(-9, 0));
                    instance.SpawnWall(PrimaryWall.orientation.Vertical);
                    break;
            }
        }
    }
    public bool LinkableInDirection(Directions.directions dir)
    {
        return links.TryGetValue(dir, out StageLink x);
    }
    public bool LinkableInDirection(Directions.directions dir, StageVariant.variants variant)
    {
        StageLink s;
        links.TryGetValue(dir, out s);
        bool t = false;
        if (s != null) t = s.IsValidVariant(variant) && s.IsVacant();
        return LinkableInDirection(dir) && t;
    }
    /// <summary>
    /// Link a stage to this stage disregarding incompatible variants. 
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="stage"></param>
    /// <returns></returns>
    public bool LinkStageUnsafe(Directions.directions dir, Stage stage)
    {
        links.TryGetValue(dir, out StageLink l);
        if (l == null)
        {
            return false;
        }
        return l.PopulateLink(stage);
    }
    private void CopyWallAttributes(SecondaryWall wall, WallLayout.invisibleWallAttributes attributes)
    {
        wall.relativePosition = attributes.Position;
        wall.xSize = attributes.XSize;
        wall.ySize = attributes.YSize;
    }
    public float GetExtentX()
    {
        return polymesh.bounds.extents.x;
    }
    public float GetExtentY()
    {
        return polymesh.bounds.extents.y;
    }
    public void ScanNearbyStages()
    {
        foreach (KeyValuePair<Directions.directions, StageLink> link in links)
        {
            link.Value.ScanNearbyStages();
        }
    }
}
