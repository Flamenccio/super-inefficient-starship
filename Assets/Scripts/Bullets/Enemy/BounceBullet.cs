using UnityEngine;

namespace Flamenccio.Attack.Enemy
{
    /// <summary>
    /// Controls bouncing enemy bullets. Bounces off of obstacles at most 10 times.
    /// </summary>
    public class BounceBullet : EnemyBulletBase
    {
        [SerializeField] private int maxBounces = 10;
        [SerializeField] private LayerMask wallsLayer;
        private int bounces = 0;

        protected override void Behavior()
        {
            if (bounces >= maxBounces)
            {
                Destroy(gameObject);
            }
        }

        protected override void Trigger(Collider2D collider)
        {
            RaycastHit2D cast = Physics2D.CircleCast(transform.position, 0.33f, Vector2.MoveTowards(transform.position, collider.transform.position, 0.50f), 0.50f, wallsLayer);
            rb.velocity = Vector2.Reflect(rb.velocity, cast.normal);
            bounces++;
        }

        protected override void Collide(Collision2D collision)
        {
            // do nothing
        }
    }
}
