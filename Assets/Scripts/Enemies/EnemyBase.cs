using System.Collections;
using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Core;
using Flamenccio.Effects;
using Flamenccio.Utility;
using Flamenccio.Components; // ALL THE EFFECTS!

namespace Enemy
{
    public interface IEnemy
    {
        int Tier { get; }
    }

    /// <summary>
    /// Base class for all enemies.
    /// </summary>
    public class EnemyBase : Destructables, DistanceBehaviorCull.IDistanceDisable
    {
        [SerializeField] protected int tier;
        [SerializeField] protected float moveSpeed = 0f;
        [SerializeField] protected SpriteRenderer spriteRen;
        [SerializeField] protected Animator animator;
        protected LayerMask playerLayer;
        protected Transform player;
        protected GameState gameState;
        protected Sprite enemySprite;
        protected Rigidbody2D rb;
        protected bool telegraphed = false; // so we only play the telegraph animation once
        protected bool alive = true;
        protected const float FLASH_DURATION = 2f / 60f;
        protected const float ATTACK_TELEGRAPH_DURATION = 12f / 60f;
        protected bool active = true;

        protected override void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            player = PlayerMotion.Instance.transform;
            playerLayer = LayerManager.GetLayerMask(Layer.Player);
        }

        protected void Awake()
        {
            currentHP = maxHP;
            OnSpawn();
        }

        public virtual void Disable()
        {
            active = false;
        }

        public virtual void Enable()
        {
            active = true;
        }

        /// <summary>
        /// Called under Awake().
        /// </summary>
        protected virtual void OnSpawn() { }

        /// <summary>
        /// The actions that the enemy will perform. This happens under FixedUpdate() and only when the enemy is active.
        /// </summary>
        protected virtual void Behavior() { }

        /// <summary>
        /// The animations that the enemy will perform. This happens under FixedUpdate() and only when the enemy is active.
        /// </summary>
        protected virtual void Animation() { }

        /// <summary>
        /// The behavior that happens when the enemy enters a trigger.
        /// </summary>
        protected virtual void Trigger(Collider2D col) { }

        protected void FixedUpdate()
        {
            if (!active) return;

            Behavior();
            Animation();
        }

        protected void HealthCheck()
        {
            // if the enemy's health reaches 0,
            if (currentHP <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            if (!alive) return; // ensures this function is only called once per enemy lifetime

            alive = false;
            Spawner.Instance.SpawnStarShard(transform.position, loot);
            GameEventManager.OnEnemyKill(GameEventManager.CreateGameEvent(transform.position));
            Destroy(gameObject); // destroy self
        }

        protected void Hurt(int damage)
        {
            currentHP -= damage;
            if (currentHP > 0)
            {
                DamageFlash();
                GameEventManager.OnEnemyHit(GameEventManager.CreateGameEvent(transform));
            }
            HealthCheck();
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(TagManager.GetTag(Tag.PlayerBullet))
                || collision.gameObject.CompareTag(TagManager.GetTag(Tag.NeutralBullet)))
            {
                Hurt(collision.gameObject.GetComponent<BulletControl>().PlayerDamage);
            }

            Trigger(collision);
        }

        protected void DamageFlash()
        {
            StartCoroutine(DamageFlashAnimation());
        }

        protected IEnumerator DamageFlashAnimation()
        {
            for (int i = 0; i < 3; i++)
            {
                spriteRen.color = Color.black;
                yield return new WaitForSeconds(FLASH_DURATION);
                spriteRen.color = Color.white;
                yield return new WaitForSeconds(FLASH_DURATION);
            }
        }

        protected IEnumerator AttackTelegraph()
        {
            animator.SetBool("attack", true);
            yield return new WaitForSeconds(ATTACK_TELEGRAPH_DURATION);
            animator.SetBool("attack", false);
        }
    }
}