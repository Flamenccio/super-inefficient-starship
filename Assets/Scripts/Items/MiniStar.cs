using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Item
{
    public class MiniStar : Star
    {
        private const float MAX_SPEED = 20.0f;
        private const float MIN_SPEED = 15.0f;
        private const float DECELERATION = MIN_SPEED / 120f;
        private float launchSpeed = MIN_SPEED;
        private Rigidbody2D rb;
        protected override void SpawnEffect()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            launchSpeed = Random.Range(MIN_SPEED, MAX_SPEED);
            rb.AddForce(transform.right * launchSpeed, ForceMode2D.Impulse);
        }
        protected override void CollectEffect(Transform player)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.miniStarCollect, transform.position);
        }
        protected override void ConstantEffect()
        {
            if (rb.velocity.magnitude > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x - rb.velocity.x * DECELERATION, rb.velocity.y - (rb.velocity.y * DECELERATION));
            }
        }
        protected override void TriggerEffect(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("InvisibleWall") || collider.gameObject.CompareTag("PrimaryWall"))
            {
                rb.velocity = Vector2.zero;
            }
        }
        protected override void CollisionEffect(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("InvisibleWall") || collision.gameObject.CompareTag("PrimaryWall"))
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
}
