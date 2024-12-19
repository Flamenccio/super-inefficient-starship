using Flamenccio.Core;
using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// Class that controls a "flying star" effect when a mini star is picked up. Flies away from player, then loops back towards player.
    /// </summary>
    public class StarFly : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        private float maxSpeed;
        private float speed = 0f;
        private Transform target;
        private float timer;
        private float trailTimer;
        private int loop = 0;
        private EffectManager effectManager;
        private const float TRAIL_FREQUENCY = 1f / 60f;

        private void Awake()
        {
            maxSpeed = 25;
        }

        private void Start()
        {
            effectManager = EffectManager.Instance;
        }

        public void FlyTo(Transform v)
        {
            speed = maxSpeed;
            target = v;
            rb.rotation = Random.Range(0, 360f);
        }

        private void FixedUpdate()
        {
            if (GameState.Paused) return;

            MoveToPlayer();
            SpawnTrails();
        }

        private void SpawnTrails()
        {
            effectManager.SpawnTrail(TrailPool.Trails.StarFlyTrail, transform.position);
        }

        private void MoveToPlayer()
        {
            // turn to face player and move towards them.
            timer += Time.deltaTime * 1.4f;
            float turnSpeed = timer;

            if (target != null)
            {
                float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x);
                rb.rotation = Mathf.LerpAngle(rb.rotation, targetAngle, turnSpeed);
                rb.linearVelocity = transform.right * speed;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && loop >= 1)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                loop++;
            }
        }
    }
}