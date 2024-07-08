using System.Linq;
using UnityEngine;

namespace Flamenccio.Utility
{
    /// <summary>
    /// Custom Physics2D methods. Note: technically not extension methods.
    /// </summary>
    public static class PhysicsExtensions
    {
        /// <summary>
        /// Casts a cone in a certain direction.
        /// </summary>
        /// <param name="origin">Start point of cone.</param>
        /// <param name="maxDistance">Radius of cone.</param>
        /// <param name="directionDegrees">Direction (in degrees) of the center of the cone.</param>
        /// <param name="widthDegrees">The angle between the legs of the cone.</param>
        /// <param name="layerMask">Layer mask.</param>
        /// <returns>Array of Collider2D that falls within the cone.</returns>
        public static Collider2D[] ConeCastAll(Vector2 origin, float maxDistance, float directionDegrees, float widthDegrees, int layerMask)
        {
            widthDegrees = Mathf.Clamp(widthDegrees, 0f, 360f);

            var colliders = Physics2D.OverlapCircleAll(origin, maxDistance, layerMask)
                .Where(x =>
                {
                    AllAngle angle = new()
                    {
                        Vector = (Vector2)x.transform.position - origin,
                    };

                    return Mathf.Abs(ShortestAngle(directionDegrees, angle.Degree)) <= (widthDegrees / 2f);
                });

            return colliders.ToArray();
        }

        /// <summary>
        /// Finds the shortest path (in degrees) from angle1 to angle2.
        /// </summary>
        private static float ShortestAngle(float angle1, float angle2)
        {
            float difference = ((angle1 - angle2 + 180f) % 360f) - 180f;
            return difference < -180f ? difference + 360f : difference;
        }
    }
}
