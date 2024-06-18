using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Item
{
    // base class for all in-game appearances of items.
    public class Item : MonoBehaviour
    {
        [SerializeField] protected string collectEffect = "i_generic_collect"; // set a default collect vfx
        [SerializeField] protected string spawnEffect;
        protected readonly string PLAYER_TAG = TagManager.GetTag(Tag.Player);

        /// <summary>
        /// Any additional effects that the item will do upon spawning.
        /// </summary>
        protected virtual void SpawnEffect() { }

        /// <summary>
        /// Any additional effects that the item will do upon collection (entering trigger collision with player).
        /// </summary>
        protected virtual void CollectEffect(Transform player) { }

        /// <summary>
        /// Any additional effects that will happen constantly (on FixedUpdate).
        /// </summary>
        protected virtual void ConstantEffect() { }

        /// <summary>
        /// Any additional effects that the item will do upon entering a trigger collision with any gameObject.
        /// </summary>
        protected virtual void TriggerEffect(Collider2D collider) { }

        protected virtual void CollisionEffect(Collision2D collision) { }

        protected void Awake()
        {
            if (spawnEffect != null)
            {
                EffectManager.Instance.SpawnEffect(spawnEffect, transform);
            }
            SpawnEffect();
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            TriggerEffect(collision);
            if (collision.CompareTag(PLAYER_TAG))
            {
                CollectEffect(collision.transform);
                EffectManager.Instance.SpawnEffect(collectEffect, transform.position);
                Destroy(gameObject);
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEffect(collision);
        }

        protected void FixedUpdate()
        {
            ConstantEffect();
        }
    }
}