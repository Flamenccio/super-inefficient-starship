using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;

namespace Flamenccio.LevelObject.Stages
{
    public class StageLink : MonoBehaviour
    {
        public Stage ParentStage { get { return parentStage; } }
        [SerializeField] private List<StageVariant.Variants> blacklistedVariants = new();
        [SerializeField] private LayerMask STAGE_LAYER;
        [SerializeField] private LayerMask STAGE_LINK_LAYER;
        private const int STAGE_LENGTH = 16; // length (and width) of a stage module. 
        private Vector2 OVERLAP_BOX_SIZE = new Vector2(STAGE_LENGTH / 2, STAGE_LENGTH / 2); // the size of the overlap box used to scan for nearby stages.
        private const float NEAR_SEARCH_RADIUS = 2.0f; // radius of circle used to search for nearby links.
        private Stage linkedStage = null;
        private PrimaryWall primaryWall = null;
        private Directions.CardinalValues placement = Directions.CardinalValues.North;
        private Stage parentStage = null;

        private void Awake()
        {
            parentStage = gameObject.GetComponentInParent<Stage>();
            primaryWall = gameObject.GetComponent<PrimaryWall>();
        }
        public bool IsValidVariant(StageVariant.Variants variant)
        {
            foreach (StageVariant.Variants v in blacklistedVariants)
            {
                if (variant == v) return false;
            }
            return true;
        }
        public bool PopulateLink(Stage stage)
        {
            if (stage == null)
            {
                return false;
            }
            if (linkedStage != null) // do not link if this link is already populated
            {
                return false;
            }
            if (!IsValidVariant(stage.Variant))
            {
                return false;
            }
            linkedStage = stage;
            primaryWall.DestroyWall();
            return true;
        }
        // bypasses the variant check, does not connect if given stage is parent or if already populated 
        private void ForcePopulateLink(Stage stage)
        {
            if (linkedStage != null) // if link is already populate it, do not overwrite!
            {
                return;
            }
            if (stage.GetInstanceID() == ParentStage.gameObject.GetInstanceID()) // do not link to parent stage
            {
                return;
            }
            if (stage == null) // if given stage is null, do not link
            {
                return;
            }

            linkedStage = stage;
            primaryWall.DestroyWall();
        }
        public bool IsVacant()
        {
            return linkedStage == null;
        }
        // just a wrapper
        public void SpawnWall(PrimaryWall.Orientation orient)
        {
            primaryWall.SpawnWall(orient);
        }
        public void UpdateProperties(List<StageVariant.Variants> blacklist, Directions.CardinalValues dir)
        {
            // we can't update the properties if the link is already in use
            if (linkedStage != null) return;

            blacklistedVariants = blacklist;
            placement = dir;
        }
        public void Place(Vector2 position)
        {
            transform.localPosition = position;
        }
        /// <summary>
        /// Scan for nearby stage links.
        /// Forces a connection even if two stages form a broken path.
        /// (It's assumed that the stage for this link is already connected to a compatible stage variant.)
        /// </summary>
        public void ScanNearbyStages()
        {
            if (linkedStage != null) return; // if this link is already populated, no need to scan for more stages.

            // First pass: look for close stage links. If there are any, connect to them.
            Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)gameObject.transform.position, NEAR_SEARCH_RADIUS, STAGE_LINK_LAYER);
            foreach (Collider2D col in colliders) // TODO simplify loop
            {
                if (col.gameObject.GetInstanceID() == gameObject.GetInstanceID()) // check if col is this game object
                {
                    continue;
                }
                if (!col.gameObject.TryGetComponent<StageLink>(out var other)) // check if this game object is a stage link
                {
                    continue;
                }
                if (other.ParentStage.gameObject.GetInstanceID() == parentStage.gameObject.GetInstanceID()) // check if parent of this stage link is the same as this one
                {
                    continue;
                }
                // otherwise, it should be ok
                ForcePopulateLink(other.ParentStage); // populate this link with the other stage 
                other.ForcePopulateLink(ParentStage); // populate the other link with this stage
                return;
            }

            // Second pass: look for close stages. If there are any, connect both stages.
            colliders = Physics2D.OverlapBoxAll((Directions.Instance.DirectionsToVector2(placement) * (STAGE_LENGTH / 2)) + (Vector2)gameObject.transform.position, OVERLAP_BOX_SIZE, 0f, STAGE_LAYER);
            foreach (Collider2D col in colliders) // TODO simplify loop
            {
                if (col.gameObject.GetInstanceID() != parentStage.gameObject.GetInstanceID()) // if the collider is this link's associated stage, move on
                {
                    Stage other = col.gameObject.GetComponent<Stage>();
                    ForcePopulateLink(other);
                    other.LinkStageUnsafe(Directions.Instance.OppositeOf(placement), ParentStage);
                    return;
                }
            }
        }
    }
}
