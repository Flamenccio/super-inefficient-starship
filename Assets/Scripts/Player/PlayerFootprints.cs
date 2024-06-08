using UnityEngine;

namespace Flamenccio.Core.Player
{
    /// <summary>
    /// Manages the set of player Footprints.
    /// </summary>
    public class PlayerFootprints : MonoBehaviour
    {
        [SerializeField] private GameObject footprintPrefab;
        private Footprint currentFootprint = null;
        private Footprint previousFootprint = null;
        private const int FOOTPRINT_AMOUNT = 10;
        private const float STEP_DISTANCE = 2.0f; // the distance needed to travel to leave one footprint

        private void Start()
        {
            InitializeFootprints();
        }

        private void Update()
        {
            if (Vector2.Distance(transform.position, previousFootprint.transform.position) >= STEP_DISTANCE)
            {
                Step();
            }
        }

        /// <summary>
        /// Spawns footprints and links them.
        /// </summary>
        private void InitializeFootprints()
        {
            for (int i = 0; i < FOOTPRINT_AMOUNT; i++)
            {
                currentFootprint = Instantiate(footprintPrefab, transform.position, Quaternion.identity).GetComponent<Footprint>();
                currentFootprint.SetPrev(previousFootprint);

                if (previousFootprint != null) previousFootprint.SetNext(currentFootprint);

                previousFootprint = currentFootprint;
            }

            currentFootprint = BottomFootprint();
        }

        /// <summary>
        /// Places the oldest footprint on the player's position and makes it the newest footprint.
        /// </summary>
        private void Step()
        {
            Footprint bottomFootprint = currentFootprint.NextFootprint;

            currentFootprint.NextFootprint.SetPrev(null);
            currentFootprint.SetNext(null);

            previousFootprint.SetNext(currentFootprint);
            currentFootprint.SetPrev(previousFootprint);

            currentFootprint.Place(transform.position);

            previousFootprint = currentFootprint;
            currentFootprint = bottomFootprint;
        }

        /// <summary>
        /// returns the footprint whose previous footprint is null
        /// </summary>
        private Footprint BottomFootprint()
        {
            Footprint fp = previousFootprint;

            while (fp.PrevFootprint != null)
            {
                fp = fp.PrevFootprint;
            }
            return fp;
        }
    }
}