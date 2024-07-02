using System.Linq;
using UnityEngine;

namespace Flamenccio.Utility
{
    public static class PhysicsExtensions
    {
        public static Collider2D[] ConeCastAll(Vector2 origin, float maxDistance, float directionDegrees, float widthDegrees, int layerMask)
        {
            directionDegrees %= 360f;
            widthDegrees = Mathf.Clamp(widthDegrees, 0f, 360f);

            var colliders = Physics2D.OverlapCircleAll(origin, maxDistance, layerMask)
                .Where(x =>
                {
                    AllAngle angle = new()
                    {
                        Vector = (Vector2)x.transform.position - origin,
                    };

                    /*
                    if (angle.Degree > directionDegrees + 180f)
                    {
                        angle.Degree -= 360f;
                    }
                    */

                    return ((directionDegrees - angle.Degree) % 360f) <= (widthDegrees / 2f);
                });

            return colliders.ToArray();
        }
    }
}
