using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    public class Stage : MonoBehaviour
    {
        public StageVariant.Variants Variant { get { return variant; } }
        public Sprite Sprite { get { return sprite; } }
        public bool InitialStage { get { return initialStage; } }
        public Vector2 Extents { get => polymesh.bounds.extents; }
        public Vector2 Center { get => polymesh.bounds.center; }
        [SerializeField] private SpriteRenderer spriteRen; // sprite renderer
        [SerializeField] private GameObject invisibleWallPrefab; // prefab for invisible walls
        [SerializeField] private GameObject stageLinkPrefab; // prefab for stage links
        [SerializeField] private bool initialStage; // is this stage the first one in the level?
        private StageVariant.Variants variant = StageVariant.Variants.Normal; // initialize the stage as a normal variant
        private Sprite sprite; // sprite representing the stage
        private Dictionary<Directions.CardinalValues, StageLink> links = new(); // a list of all stage links associated with their direction
        private PolygonCollider2D polyCollider; // collider that conforms to sprite shape
        private Mesh polymesh; // mesh for poly collider
        private void Awake()
        {
            polyCollider = gameObject.GetComponent<PolygonCollider2D>();
            polymesh = polyCollider.CreateMesh(true, true);
        }
        private void Start()
        {
            if (initialStage) UpdateVariant(StageVariant.Variants.Normal);
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
            BuildSecondaryWalls(v); // spawn secondary walls
            BuildLinks(v); // spawn and place stage links (primary walls)
        }
        private void BuildSecondaryWalls(StageVariant variant)
        {
            int i = 0;

            if (variant.SecondaryWallLayout != null)
            {
                foreach (var invisibleWallConfig in variant.SecondaryWallLayout.Layout)
                {
                    SecondaryWall instance = Instantiate(invisibleWallPrefab, transform).GetComponent<SecondaryWall>();
                    CopyWallAttributes(instance, invisibleWallConfig);
                    instance.UpdateAttributes();
                    i++;
                }
            }
        }
        private void BuildLinks(StageVariant variant)
        {
            foreach (var link in variant.Links)
            {
                StageLink instance = Instantiate(stageLinkPrefab, transform).GetComponent<StageLink>();
                instance.UpdateProperties(link.BlackListedVariants, link.LinkDirection);
                links.Add(link.LinkDirection, instance);

                switch (link.LinkDirection)
                {
                    case Directions.CardinalValues.North:
                        instance.Place(new Vector2(0, 9));
                        instance.SpawnWall(PrimaryWall.Orientation.Horizontal);
                        break;

                    case Directions.CardinalValues.East:
                        instance.Place(new Vector2(9, 0));
                        instance.SpawnWall(PrimaryWall.Orientation.Vertical);
                        break;

                    case Directions.CardinalValues.South:
                        instance.Place(new Vector2(0, -9));
                        instance.SpawnWall(PrimaryWall.Orientation.Horizontal);
                        break;

                    case Directions.CardinalValues.West:
                        instance.Place(new Vector2(-9, 0));
                        instance.SpawnWall(PrimaryWall.Orientation.Vertical);
                        break;
                }
            }
        }
        public bool LinkableInDirection(Directions.CardinalValues dir)
        {
            return links.TryGetValue(dir, out StageLink x) && x.IsVacant();
        }
        /// <summary>
        /// Link a stage to this stage disregarding incompatible variants.
        /// </summary>
        public bool LinkStageUnsafe(Directions.CardinalValues dir, Stage stage)
        {
            links.TryGetValue(dir, out StageLink l);
            if (l == null)
            {
                return false;
            }
            return l.PopulateLink(stage);
        }
        private void CopyWallAttributes(SecondaryWall wall, WallLayout.WallAttributes attributes)
        {
            // URGENT temporarily disabled
            /*
            wall.relativePosition = attributes.Position;
            wall.xSize = attributes.XSize;
            wall.ySize = attributes.YSize;
            */
        }
        public void ScanNearbyStages()
        {
            foreach (KeyValuePair<Directions.CardinalValues, StageLink> link in links)
            {
                link.Value.ScanNearbyStages();
            }
        }
    }
}
