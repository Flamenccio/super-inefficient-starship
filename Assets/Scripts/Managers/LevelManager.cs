using System.Collections.Generic;
using UnityEngine;
using Flamenccio.LevelObject.Stages;
using Flamenccio.Utility;
using System.Linq;

namespace Flamenccio.Core
{
    public class LevelManager : MonoBehaviour
    {
        public List<Stage> Stages { get; } = new();
        private const float STAGE_LENGTH = 8.0f;

        public void Awake()
        {
            if (Stages.Count > 1) return;

            Stages.Add(FindObjectOfType<Stage>());
        }

        /// <summary>
        /// Places a new stage in the direction of a root stage. <b>Does not check if the stages are incompatible.</b>
        /// <para>The root stage must already exist. The new stage must not already have been added.</para>
        /// </summary>
        /// <param name="newStage">New stage to add.</param>
        /// <param name="rootStage">Old stage to extend from.</param>
        /// <param name="direction">Direction to extend from old stage.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public bool AddStage(Stage newStage, Stage rootStage, Directions.CardinalValues direction)
        {
            bool rootStageExists = Stages.Exists(x => x.gameObject.GetInstanceID() == rootStage.gameObject.GetInstanceID()); // The rootStage must already exist in the Stages list.
            bool newStageDoesNotExist = !Stages.Exists(y => y.gameObject.GetInstanceID() == newStage.gameObject.GetInstanceID()); // The newStage must NOT already exist in the Stages list.
            bool ready = rootStageExists && newStageDoesNotExist;

            if (!ready) return false; // do not extend if the given root stage is not in the Stages list.

            rootStage.ScanNearbyStages();

            if (!rootStage.LinkableInDirection(direction)) return false; // do not extend if the root stage is already linked in direction.

            newStage.transform.position = (Vector2)rootStage.transform.position + (2 * STAGE_LENGTH * Directions.DirectionsToVector2(direction));
            rootStage.LinkStageUnsafe(direction, newStage);
            newStage.LinkStageUnsafe(Directions.OppositeOf(direction), rootStage);
            newStage.ScanNearbyStages();
            Stages.Add(newStage);

            return true;
        }

        /// <summary>
        /// Returns a random stage from the Stages list.
        /// </summary>
        public Stage GetRandomStage()
        {
            return Stages[Random.Range(0, Stages.Count)];
        }

        /// <summary>
        /// Returns a random stage from the Stages list except for a list of given stages.
        /// </summary>
        public Stage GetRandomStageExcept(List<Stage> exceptions)
        {
            List<Stage> filteredList = new(Stages.Except(exceptions));
            return filteredList[Random.Range(0, filteredList.Count - 1)];
        }

        /// <summary>
        /// Returns a random stage from the Stages list except for a given stages.
        /// </summary>
        public Stage GetRandomStageExcept(Stage exception)
        {
            return GetRandomStageExcept(new List<Stage> { exception });
        }
    }
}
