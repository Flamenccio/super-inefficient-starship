using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Attack.Enemy
{
    public class TorpedoBullet : EnemyBulletNormal
    {
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private GameObject afterimage;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject hitbox;
        private GameObject player;
        private float lifetime = 0f;
        private float afterimageTimer = 0f;
        private const float SEARCH_RADIUS = 6f;
        private const float TURN_SPEED = 6f / 60f;
        private const float AFTERIMAGE_FREQUENCY = 3f / 60f;
        protected override void Behavior()
        {
            if (player == null)
            {
                rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                Collider2D search = Physics2D.OverlapCircle(transform.position, SEARCH_RADIUS, playerLayer);
                if (search != null) player = search.gameObject;
            }
            if (player != null)
            {
                AllAngle toPlayer = new()
                {
                    Vector = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y)
                };
                rb.rotation = Mathf.LerpAngle(rb.rotation, toPlayer.Degree, TURN_SPEED);
                rb.velocity = transform.right * moveSpeed;
            }

            afterimageTimer += Time.deltaTime;

            if (afterimageTimer >= AFTERIMAGE_FREQUENCY)
            {
                afterimageTimer = 0f;
                Instantiate(afterimage, transform.position, Quaternion.identity);
            }
        }
        protected override void DeathTimer()
        {
            lifetime += Time.deltaTime;
            if (lifetime >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
        protected override void Trigger(Collider2D collider)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>().EditProperties(0f, 2f, damage, Hitbox.AttackType.Enemy, KnockbackMultipiers.High);
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Strong, transform.position);
            base.Trigger(collider);
        }
    }
}
