using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Objects;
using Flamenccio.Core;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Controls behavior of a stage object.
    /// </summary>
    public class Stage : MonoBehaviour
    {
        public string VariantId { get => variantId; }
        public bool InitialStage { get { return initialStage; } }
        public List<Portal> Portals { get => portals; }
        public Dictionary<Directions.CardinalValues, StageLink> StageLinks { get => links; }

        [SerializeField] private GameObject invisibleWallPrefab; // prefab for invisible walls
        [SerializeField] private GameObject stageLinkPrefab; // prefab for stage links
        [SerializeField] private bool initialStage; // is this stage the first one in the level?
        [SerializeField, Tooltip("The child transform that allows the stage to move independently from its actual position.")] private Transform shapeTransform;
        [SerializeField] private StageShape stageShape;

        private string variantId = "normal"; // initialize as normal variant.
        private Dictionary<Directions.CardinalValues, StageLink> links = new(); // a list of all stage links associated with their direction
        private List<Portal> portals = new(MAX_PORTAL_COUNT);
        private const int MAX_PORTAL_COUNT = 2;

        private void Awake()
        {
            stageShape = shapeTransform.gameObject.GetComponent<StageShape>();
        }

        private void Start()
        {
            if (initialStage) UpdateVariant("normal");
        }

        /// <summary>
        /// Change this stage's variant to the one given.
        /// </summary>
        public void UpdateVariant(string updatedVariantId)
        {
            variantId = updatedVariantId;
            var v = StageResources.Instance.GetStageProperties(variantId);
            stageShape.StageSprite = v.Sprite;
            stageShape.FaceDirection(v.Direction);

            if (v.LinkSet.Count == 0) // TODO Make a better "null" check.
            {
                Debug.LogError($"Failed to update stage: {variantId} not valid stage variant.");
                return;
            }

            BuildBoundaries(v);
            BuildLinks(v);
        }

        private void BuildBoundaries(StageProperties properties)
        {
            if (properties.WallLayout == null) return;

            foreach (var wallConfig in properties.WallLayout.Layout)
            {
                var instance = Instantiate(invisibleWallPrefab, stageShape.transform).GetComponent<SecondaryWall>();
                instance.BuildWall(wallConfig);
            }
        }

        private void BuildLinks(StageProperties variant)
        {
            foreach (var link in variant.LinkSet)
            {
                StageLink instance = Instantiate(stageLinkPrefab, transform).GetComponent<StageLink>();
                instance.UpdateProperties(link.SubLinkMask, link.LinkDirection);
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
            float xBounds = stageShape.Extents.x;
            float yBounds = stageShape.Extents.y - 0.5f;
            Vector2 raycastOrigin = new(
                transform.position.x + stageShape.Center.x - xBounds - 1,
                transform.position.y  + stageShape.Center.y + Random.Range(-yBounds, yBounds));
            bool turn = false; // false = looking for raycastTestLayer; true = looking for inviswall layer
            List<float> collisions = new(); // x coordinates--y is kept constant
            LayerMask raycastEnterLayer = LayerManager.GetLayerMask(Layer.RaycastTest);
            LayerMask raycastExitLayers = LayerManager.GetLayerMask(new List<Layer>{ Layer.Stage, Layer.InvisibleWall });
            stageShape.gameObject.layer = LayerManager.GetLayer(Layer.RaycastTest); // temporarily change stage layer
            RaycastHit2D ray;

            do
            {
                ray = Physics2D.Raycast(raycastOrigin, Vector2.right, 2 * stageShape.Extents.x, turn ? raycastExitLayers : raycastEnterLayer);

                if (ray.collider == null) break;

                raycastOrigin.x = ray.point.x + 0.05f;
                collisions.Add(ray.point.x);

                turn = !turn;
            } while (ray.collider != null);

            stageShape.gameObject.layer = LayerManager.GetLayer(Layer.Stage); // return stage layer
            int pairs = Mathf.FloorToInt(collisions.Count / 2f);
            int pair = Random.Range(0, pairs);

            if (pairs < 1)
            {
                Debug.LogWarning($"{variantId}: Failed to find a global point.");
                return new(0, 0);
            }

            return new(Random.Range(collisions[2 * pair], collisions[(2 * pair) + 1]), raycastOrigin.y);
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
            var extents = stageShape.Extents;
            return new(Random.Range(-extents.x, extents.x), Random.Range(-extents.y, extents.y));
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