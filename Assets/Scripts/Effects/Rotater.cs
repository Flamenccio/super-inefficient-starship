using UnityEngine;
using Flamenccio.Effects;

namespace Flamenccio.Utility.SpriteEffects
{
    public class Rotater : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;
        [SerializeField] private RotateDirection direction = RotateDirection.Clockwise;
        public enum RotateDirection
        {
            Clockwise = -1,
            ClounterClockwise = 1
        }
        private void FixedUpdate()
        {
            if (Vector2.Distance(transform.position, PlayerMotion.Instance.PlayerPosition) <= 20f) transform.Rotate(new Vector3(0f, 0f, speed * (int)direction));
        }
    }
}
