using UnityEngine;

namespace Flamenccio.Attack
{
    public class BounceBullet : EnemyBulletNormal
    {
        private int bounces = 0;
        [SerializeField] private int maxBounces = 10;
        [SerializeField] private LayerMask wallsLayer;

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
