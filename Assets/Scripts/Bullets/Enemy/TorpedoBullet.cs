using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Attack.Enemy
{
    public class TorpedoBullet : EnemyBulletBase
    {
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private GameObject hitbox;
        [SerializeField] private string explodeVfx;
        private GameObject player;
        private EffectManager effectManager;
        private float lifetime = 0f;
        private float trailTimer = 0f;
        private const float SEARCH_RADIUS = 6f;
        private const float TURN_SPEED = 6f / 60f;
        private const float TRAIL_FREQUENCY = 3f / 60f;

        private void Start()
        {
            effectManager = EffectManager.Instance;
        }

        protected override void Behavior()
        {
            TrackPlayer();
            SpawnTrails();
        }

        protected override void DeathTimer()
        {
            lifetime += Time.deltaTime;

            if (lifetime >= maxDistance)
            {
                Destroy(gameObject);
            }
        }

        private void TrackPlayer()
        {
            if (player == null)
            {
                // search for the player nearby
                rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                Collider2D search = Physics2D.OverlapCircle(transform.position, SEARCH_RADIUS, playerLayer);

                if (search != null) player = search.gameObject;
            }
            else
            {
                // move towards player
                AllAngle toPlayer = new()
                {
                    Vector = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y)
                };
                rb.rotation = Mathf.LerpAngle(rb.rotation, toPlayer.Degree, TURN_SPEED);
                rb.velocity = transform.right * moveSpeed;
            }
        }

        private void SpawnTrails()
        {
            trailTimer += Time.deltaTime;

            if (trailTimer >= TRAIL_FREQUENCY)
            {
                trailTimer = 0f;
                effectManager.SpawnTrail(TrailPool.Trails.EnemyMissileTrail, transform.position);
            }
        }

        protected override void Trigger(Collider2D collider)
        {
            EffectManager.Instance.SpawnEffect(explodeVfx, transform.position);
            Instantiate(hitbox, transform.position, Quaternion.identity).GetComponent<Hitbox>().EditProperties(0f, 2f, playerDamage, Hitbox.HitboxAffiliation.Enemy, KnockbackPower.High);
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Strong, transform.position);
            base.Trigger(collider);
        }
    }
}