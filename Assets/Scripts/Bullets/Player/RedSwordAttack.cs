using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    /// <summary>
    /// Controls a player bullet. Swings a large sword in front of player. Sticks to player, not affected by walls.
    /// </summary>
    public class RedSwordAttack : PlayerBullet
    {
        [HideInInspector] public bool Flipped { get; set; }
        [SerializeField] private CircleCollider2D hitCollider;
        [SerializeField] private SpriteRenderer spriteRen;
        private float lifeTimer = 0f;
        private const float ACTIVATION_FRAME = 2f / 24f;
        private const float LIFE_TIMER_MAX = 6f / 24f;
        private const float ACTIVATION_TIMER_MAX = 1f / 24f;
        private bool activated = false;

        protected override void Launch()
        {
            // don't launch
        }

        protected override void Behavior()
        {
            spriteRen.flipY = Flipped;
            lifeTimer += Time.deltaTime;

            if (!activated && lifeTimer >= ACTIVATION_FRAME)
            {
                activated = true;
                hitCollider.enabled = true;
            }
            else if (activated && lifeTimer >= ACTIVATION_FRAME + ACTIVATION_TIMER_MAX)
            {
                hitCollider.enabled = false;
            }

            if (lifeTimer >= LIFE_TIMER_MAX)
            {
                Destroy(gameObject);
            }
        }

        protected override void Trigger(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(TagManager.GetTag(Tag.EnemyBullet)))
            {
                EffectManager.Instance.SpawnEffect("BulletParry", transform.position);
            }
            else
            {
                EffectManager.Instance.SpawnEffect("BulletImpact", transform.position);
            }
        }
    }
}