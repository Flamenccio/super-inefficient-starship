using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRen; // sprite renderer
    [SerializeField] private GameObject invisibleWallPrefab; // prefab for invisible walls
    [SerializeField] private GameObject stageLinkPrefab; // prefab for stage links
    [SerializeField] private bool initialStage; // is this stage the first one in the level?

    private StageVariant.Variants variant = StageVariant.Variants.Normal; // initialize the stage as a normal variant
    private Sprite sprite; // sprite representing the stage
    private Dictionary<Directions.directions, StageLink> links = new(); // a list of all stage links associated with their direction
    private PolygonCollider2D polyCollider; // collider that conforms to sprite shape
    private Mesh polymesh; // mesh for poly collider
    public StageVariant.Variants Variant { get { return variant; } }
    public Sprite Sprite { get { return sprite; } }
    public bool InitialStage { get { return initialStage; } }
    public Vector2 Extents { get => polymesh.bounds.extents; }
    public Vector2 Center { get => polymesh.bounds.center; }
    private void Awake()
    {
        if (initialStage) UpdateVariant(StageVariant.Variants.Normal);

        polyCollider = gameObject.GetComponent<PolygonCollider2D>();
        polymesh = polyCollider.CreateMesh(true, true);
    }
    /// <summary>
    /// Request an extension handshake to this stage.
    /// </summary>
    /// <param name="dir">Direction to extend in.</param>
    /// <param name="stage">Stage to extend to.</param>
    /// <returns>True if successful, false if unsuccessful</returns>
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
    public void UpdateVariant(StageVariant.Variants updatedVariant)
    {
        variant = updatedVariant;
        StageVariant v = StageResources.Instance.GetStageVariant(variant);
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
        StageVariant v = StageResources.Instance.GetStageVariant(variant); // retrieve the variant's template

        // spawn secondary walls
        int i = 0;
        if (v.SecondaryWallLayout != null)
        {
            foreach (var invisibleWallConfig in v.SecondaryWallLayout.Layout)
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
        return links.TryGetValue(dir, out StageLink x) && x.IsVacant();
    }
    public bool LinkableInDirection(Directions.directions dir, StageVariant.Variants variant)
    {
        links.TryGetValue(dir, out StageLink s);
        bool t = false;
        if (s != null) t = s.IsValidVariant(variant);
        return LinkableInDirection(dir) && t;
    }
    /// <summary>
    /// Link a stage to this stage disregarding incompatible variants. 
    /// </summary>
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
    public void ScanNearbyStages()
    {
        foreach (KeyValuePair<Directions.directions, StageLink> link in links)
        {
            link.Value.ScanNearbyStages();
        }
    }
}
