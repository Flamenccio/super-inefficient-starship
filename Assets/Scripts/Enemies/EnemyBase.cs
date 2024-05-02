using System.Collections;
using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Core;
using Flamenccio.Effects; // ALL THE EFFECTS!
using Flamenccio.Effects.Visual;
using Flamenccio.Effects.Audio;

namespace Enemy
{
    public interface IEnemy
    {
        int Tier { get; }
    }
    public class EnemyBase : Destructables
    {
        [SerializeField] protected int tier;
        [SerializeField] protected float moveSpeed = 0f;
        [SerializeField] protected LayerMask playerLayer;
        [SerializeField] protected SpriteRenderer spriteRen;
        [SerializeField] protected Animator animator;
        [SerializeField] protected GameObject miniStarPrefab;
        [SerializeField] protected float activeRange; // the player must be within range for this enemy to activate
        protected Transform player;
        protected GameState gameState;
        protected Sprite enemySprite;
        protected Rigidbody2D rb;
        protected float slowUpdateTimer = 0f;
        protected bool active = true; // is this enemy currently active?
        protected bool telegraphed = false; // so we only play the telegraph animation once
        protected const float FLASH_DURATION = 2f / 60f;
        protected const float ATTACK_TELEGRAPH_DURATION = 12f / 60f;
        protected const float SLOW_UPDATE_FREQUENCY = 0.25f;

        protected override void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            player = PlayerMotion.Instance.transform;
        }
        protected void Awake()
        {
            currentHP = maxHP;
            OnSpawn();
        }
        protected virtual void OnSpawn()
        {

        }
        /// <summary>
        /// The actions that the enemy will perform. This happens under Update()
        /// </summary>
        protected virtual void Behavior()
        {
            // stuff to do
        }
        protected virtual void Animation()
        {

        }
        protected virtual void Trigger(Collider2D col)
        {
            // additional checks for OnTriggerEnter
        }
        protected void FixedUpdate()
        {
            if (active)
            {
                Behavior();
                Animation();
            }

            HealthCheck();

            if (slowUpdateTimer >= SLOW_UPDATE_FREQUENCY)
            {
                slowUpdateTimer = 0f;
                SlowUpdate();
            }
            else
            {
                slowUpdateTimer += Time.fixedDeltaTime;
            }
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
            for (int i = 0; i < loot; i++)
            {
                float randomAngle = UnityEngine.Random.Range(0f, 359f);
                Instantiate(miniStarPrefab, transform.position, Quaternion.Euler(0f, 0f, randomAngle));
            }

            EffectManager.Instance.SpawnEffect(EffectManager.Effects.EnemyKill, transform.position);
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Normal, transform.position); // do a little shake
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.enemyKill, transform.position);
            Destroy(gameObject); // destroy self
        }
        protected void Hurt(int damage)
        {
            currentHP -= damage;
            if (currentHP > 0)
            {
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.enemyHurt, transform.position);
                DamageFlash();
                CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, transform.position);
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.EnemyHit, transform.position);
            }
        }
        /// <summary>
        /// Called every 0.25 seconds.
        /// </summary>
        protected virtual void SlowUpdate()
        {
            active = Vector2.Distance(transform.position, player.position) <= activeRange;
        }
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("PBullet") || collision.gameObject.CompareTag("NBullet"))
            {
                Hurt(collision.gameObject.GetComponent<BulletControl>().Damage);
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
