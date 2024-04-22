using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.LevelObject.Stages
{
    [CreateAssetMenu(fileName = "Empty Layout", menuName = "Invisible Wall Layout", order = 0)]
    public class WallLayout : ScriptableObject
    {
        [System.Serializable]
        public struct invisibleWallAttributes
        {
            [Tooltip("Horizontal size of the wall.")]
            [SerializeField] private float xSize;
            [Tooltip("Vertical size of the wall.")]
            [SerializeField] private float ySize;
            [Tooltip("The relative position of the invisible wall from an origin point.")]
            [SerializeField] private Vector2 position;
            [Tooltip("Is the wall affected by new spawned rooms?")]
            [SerializeField] private bool permanence;

            public float XSize { get => xSize; }
            public float YSize { get => ySize; }
            public Vector2 Position { get => position; }
            public bool Permanence { get => permanence; }
        }
        [Tooltip("The stage variant that this layout is associated with.")]
        [SerializeField] private StageVariant.Variants associatedVariant;
        [SerializeField] private List<invisibleWallAttributes> layout = new List<invisibleWallAttributes>();
        public List<invisibleWallAttributes> Layout { get => layout; }
        public StageVariant.Variants AssociatedVariant { get => associatedVariant; }
    }
}
