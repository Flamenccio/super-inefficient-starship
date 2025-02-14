using UnityEngine;

namespace Flamenccio.Core.Player
{
    /// <summary>
    /// Leaves a trail behind player as they move around. Allows some enemies to track player.
    /// </summary>
    public class Footprint : MonoBehaviour
    {
        public Footprint NextFootprint
        {
            get { return nextFootprint; }
        }

        public Footprint PrevFootprint
        {
            get { return prevFootprint; }
        }

        public float Age { get => age; }
        private float age = 0f; // the age of the footprint in seconds
        private Footprint nextFootprint = null;
        private Footprint prevFootprint = null;

        public void ResetAge()
        {
            age = 0f;
        }

        public void Place(Vector2 pos)
        {
            transform.position = pos;
            ResetAge();
        }

        /// <summary>
        /// udpates this footprint's next footprint and returns the footprint that was replaced
        /// </summary>
        public Footprint SetNext(Footprint footprint)
        {
            Footprint last = nextFootprint;
            nextFootprint = footprint;
            return last;
        }

        public Footprint SetPrev(Footprint footprint)
        {
            Footprint last = prevFootprint;
            prevFootprint = footprint;
            return last;
        }
    }
}