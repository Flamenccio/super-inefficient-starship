using UnityEngine;
using Flamenccio.Effects.Audio;
using Flamenccio.Utility;

namespace Flamenccio.Item
{
    /// <summary>
    /// An item that drops from destroying enemies.
    /// </summary>
    public class MiniStar : Star
    {
        private const float MAX_SPEED = 20.0f;
        private const float MIN_SPEED = 15.0f;
        private const float DECELERATION = MIN_SPEED / 120f;
        private Rigidbody2D rb;

        protected override void SpawnEffect()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            float launchSpeed = Random.Range(MIN_SPEED, MAX_SPEED);
            var direction = Directions.RandomVector2();
            rb.AddForce(direction * launchSpeed, ForceMode2D.Impulse);
        }

        protected override void CollectEffect(Transform player)
        {
            AudioManager.Instance.PlayOneShot(collectSfx, transform.position);
        }

        protected override void ConstantEffect()
        {
            if (rb.velocity.magnitude > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x - (rb.velocity.x * DECELERATION), rb.velocity.y - (rb.velocity.y * DECELERATION));
            }
        }

        protected override void TriggerEffect(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(TagManager.GetTag(Tag.InvisibleWall)) 
                || collider.gameObject.CompareTag(TagManager.GetTag(Tag.PrimaryWall)))
            {
                rb.velocity = Vector2.zero;
            }
        }

        protected override void CollisionEffect(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(TagManager.GetTag(Tag.InvisibleWall))
                || collision.gameObject.CompareTag(TagManager.GetTag(Tag.PrimaryWall)))
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
}