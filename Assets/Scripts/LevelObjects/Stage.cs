using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Controls behavior of a stage object.
    /// </summary>
    public class Stage : MonoBehaviour
    {
        public StageVariant.Variants Variant { get { return variant; } }
        public Sprite Sprite { get { return sprite; } }
        public bool InitialStage { get { return initialStage; } }
        public Vector2 Extents { get => polymesh.bounds.extents; }
        public Vector2 Center { get => polymesh.bounds.center; }
        public List<Portal> Portals { get => portals; }

        [SerializeField] private SpriteRenderer spriteRen; // sprite renderer
        [SerializeField] private GameObject invisibleWallPrefab; // prefab for invisible walls
        [SerializeField] private GameObject stageLinkPrefab; // prefab for stage links
        [SerializeField] private bool initialStage; // is this stage the first one in the level?

        private StageVariant.Variants variant = StageVariant.Variants.Normal; // initialize the stage as a normal variant
        private Sprite sprite; // sprite representing the stage
        private Dictionary<Directions.CardinalValues, StageLink> links = new(); // a list of all stage links associated with their direction
        private PolygonCollider2D polyCollider; // collider that conforms to sprite shape
        private Mesh polymesh; // mesh for poly collider
        private List<Portal> portals = new(MAX_PORTAL_COUNT);
        private const int MAX_PORTAL_COUNT = 2;

        private void Awake()
        {
            polyCollider = gameObject.GetComponent<PolygonCollider2D>();
            polymesh = polyCollider.CreateMesh(true, true);
        }

        private void Start()
        {
            if (initialStage) UpdateVariant(StageVariant.Variants.Normal);
        }

        /// <summary>
        /// Change this stage's variant to the one given.
        /// </summary>
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
            if (variant.SecondaryWallLayout == null) return;

            foreach (var invisibleWallConfig in variant.SecondaryWallLayout.Layout)
            {
                SecondaryWall instance = Instantiate(invisibleWallPrefab, transform).GetComponent<SecondaryWall>();
                instance.BuildWall(invisibleWallConfig);
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

        public void ScanNearbyStages()
        {
            foreach (KeyValuePair<Directions.CardinalValues, StageLink> link in links)
            {
                link.Value.ScanNearbyStages();
            }
        }

        /// <summary>
        /// Adds a portal to this Stage. Each stage can have a maximum of 2 portals.
        /// </summary>
        /// <param name="newPortal">The portal to add.</param>
        /// <returns>True if successful, false if unsuccessful.</returns>
        public bool AddPortal(Portal newPortal)
        {
            if (newPortal == null) return false;

            if (portals.Count >= MAX_PORTAL_COUNT) return false;

            portals.Add(newPortal);
            return true;
        }

        public bool CanAddPortals()
        {
            return portals.Count < MAX_PORTAL_COUNT;
        }

        /// <summary>
        /// Returns a random global coordinate that lies within the stage.
        /// </summary>
        public Vector2 GetGlobalPointInStage()
        {
            float xBounds = Extents.x;
            float yBounds = Extents.y - 0.5f;
            Vector2 raycastOrigin = new(
                gameObject.transform.position.x - xBounds - 1,
                Random.Range(-yBounds, yBounds) + transform.position.y + Center.y);
            bool turn = false; // false = looking for raycastTestLayer; true = looking for inviswall layer
            List<float> collisions = new(); // x coordinates--y is kept constant
            LayerMask raycastEnterLayer = LayerManager.GetLayerMask(Layer.RaycastTest);
            LayerMask raycastExitLayers = LayerManager.GetLayerMask(new List<Layer>{ Layer.Stage, Layer.InvisibleWall });
            gameObject.layer = LayerManager.GetLayer(Layer.RaycastTest); // temporarily change stage layer
            RaycastHit2D ray;

            do
            {
                ray = Physics2D.Raycast(raycastOrigin, Vector2.right, 16, turn ? raycastExitLayers : raycastEnterLayer);

                if (ray.collider == null) break;

                raycastOrigin.x = ray.point.x + 0.05f;
                collisions.Add(ray.point.x);
                turn = !turn;
            } while (ray.collider != null);

            gameObject.layer = LayerManager.GetLayer(Layer.Stage); // return stage layer
            int pairs = Mathf.FloorToInt(collisions.Count / 2f);
            int pair = Random.Range(0, pairs);

            return new(Random.Range(collisions[2 * pair], collisions[(2 * pair) + 1]), raycastOrigin.y); // FIXME causes problems
        }

        /// <summary>
        /// Returns a random local coordinate within the stage.
        /// </summary>
        public Vector2 GetLocalPointInStage()
        {
            return GetGlobalPointInStage() - (Vector2)transform.position;
        }

        /// <summary>
        /// Returns a random local coordinate within the stage extents <b>(not guaranteed to be within stage)</b>.
        /// </summary>
        public Vector2 GetLocalPointInExtents()
        {
            return new(Random.Range(-Extents.x, Extents.x), Random.Range(-Extents.y, Extents.y));
        }

        /// <summary>
        /// Returns a random global coordinate within the stage extents <b>(not guaranteed to be within stage)</b>.
        /// </summary>
        public Vector2 GetGlobalPointInExents()
        {
            return GetLocalPointInExtents() + (Vector2)transform.position;
        }

        /// <summary>
        /// Determines whether a point is within the stage.
        /// </summary>
        /// <param name="point">Point to determine.</param>
        /// <param name="isGlobal">Is this point a global coordinate? (true = global, false = local)</param>
        /// <returns>True if the point is within stage, false otherwise.</returns>
        public bool PointIsInStage(Vector2 point, bool isGlobal)
        {
            if (!isGlobal) point += (Vector2)transform.position;

            Collider2D cast = Physics2D.OverlapPoint(point, LayerManager.GetLayerMask(Layer.Stage));

            return cast != null;
        }
    }
}