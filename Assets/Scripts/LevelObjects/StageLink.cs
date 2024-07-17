using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Utility;
using System.Linq;

namespace Flamenccio.LevelObject.Stages
{
    /// <summary>
    /// Controls a gameobject that allows stages to connect to each other.
    /// </summary>
    public class StageLink : MonoBehaviour
    {
        public Stage ParentStage { get { return parentStage; } }

        [SerializeField] private LayerMask STAGE_LAYER;
        [SerializeField] private LayerMask STAGE_LINK_LAYER;

        private Vector2 OVERLAP_BOX_SIZE = new(STAGE_LENGTH / 2, STAGE_LENGTH / 2); // the size of the overlap box used to scan for nearby stages.
        private Stage linkedStage = null;
        private PrimaryWall primaryWall = null;
        private Directions.CardinalValues placement = Directions.CardinalValues.North;
        private Stage parentStage = null;
        private int subLinkMask = 0;

        private const int STAGE_LENGTH = 16; // length (and width) of a stage module.
        private const float NEAR_SEARCH_RADIUS = 2.0f; // radius of circle used to search for nearby links.

        private void Awake()
        {
            parentStage = gameObject.GetComponentInParent<Stage>();
            primaryWall = gameObject.GetComponent<PrimaryWall>();
        }

        /// <summary>
        /// Is the given stage able to connect to this link?
        /// </summary>
        /// <param name="newStage">New stage to connect to.</param>
        /// <returns>True if stage is compatible, false otherwise.</returns>
        public bool IsCompatibleStage(Stage newStage)
        {
            var oppositeDirection = Directions.OppositeOf(placement);

            if (!newStage.StageLinks.TryGetValue(oppositeDirection, out var link)) return false;

            if ((subLinkMask & link.subLinkMask) == 0) return false;

            return true;
        }

        /// <summary>
        /// Link this stage link to the given stage.
        /// <para>Fails when: given stage is null, this link is already occupied, or the given stage is an blacklisted variant.</para>
        /// </summary>
        /// <returns>True if successful, false if unsuccessful.</returns>
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

            if (!IsCompatibleStage(stage))
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
            if (stage == null) // if given stage is null, do not link
            {
                return;
            }
            if (stage.GetInstanceID() == ParentStage.gameObject.GetInstanceID()) // do not link to parent stage
            {
                return;
            }

            linkedStage = stage;
            primaryWall.DestroyWall();
        }

        /// <summary>
        /// Is this link currently unoccupied?
        /// </summary>
        public bool IsVacant()
        {
            return linkedStage == null;
        }

        /// <summary>
        /// Build a primary wall with the given wall orientation.
        /// </summary>
        public void SpawnWall(PrimaryWall.Orientation orient)
        {
            primaryWall.SpawnWall(orient);
        }

        /// <summary>
        /// Update this link's properties.
        /// </summary>
        /// <param name="subLinkMask">This link's sub link mask.</param>
        /// <param name="direction">Direction relative to stage to spawn in.</param>
        public void UpdateProperties(int subLinkMask, Directions.CardinalValues direction)
        {
            // we can't update the properties if the link is already in use
            if (linkedStage != null) return;

            this.subLinkMask = subLinkMask;
            placement = direction;
        }

        /// <summary>
        /// Place this link somewhere.
        /// </summary>
        public void Place(Vector2 position)
        {
            transform.localPosition = position;
        }

        /// <summary>
        /// Scan for nearby stage links.
        /// <para>Forces a connection even if two stages form a broken path.</para>
        /// <para>(It's assumed that the stage for this link is already connected to a compatible stage variant.)</para>
        /// </summary>
        public void ScanNearbyStages()
        {
            if (linkedStage != null) return; // if this link is already populated, no need to scan for more stages.

            // First pass: look for close stage links. If there are any, connect to them.
            Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)gameObject.transform.position, NEAR_SEARCH_RADIUS, STAGE_LINK_LAYER);

            colliders
                .Select(col => col.gameObject.TryGetComponent<StageLink>(out var other) ? (Collider2D: col, Other: other) : (null, null))
                .Where(pair => pair.Collider2D != null && pair.Other != null &&
                (pair.Collider2D.gameObject.GetInstanceID() != pair.Other.gameObject.GetInstanceID()) &&
                (gameObject.GetInstanceID() != pair.Collider2D.gameObject.GetInstanceID()))
                .ToList()
                .ForEach(i =>
                {
                    ForcePopulateLink(i.Other.ParentStage);
                    i.Other.ForcePopulateLink(ParentStage);
                });

            // Second pass: look for close stages. If there are any, connect both stages.
            colliders = Physics2D.OverlapBoxAll((Directions.DirectionsToVector2(placement) * (STAGE_LENGTH / 2)) + (Vector2)gameObject.transform.position, OVERLAP_BOX_SIZE, 0f, STAGE_LAYER);

            colliders
                .Select(x => x.GetComponentInParent<Stage>())
                .Where(x => x.gameObject.GetInstanceID() != ParentStage.gameObject.GetInstanceID())
                .ToList()
                .ForEach(i =>
                {
                    ForcePopulateLink(i);
                    i.LinkStageUnsafe(Directions.OppositeOf(placement), ParentStage);
                });
        }
    }
}