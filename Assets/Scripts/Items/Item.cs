using Flamenccio.Effects.Visual;
using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Item
{
    // base class for all in-game appearances of items.
    public class Item : MonoBehaviour
    {
        [SerializeField, Tooltip("Must be all lowercase, no spaces.")] protected string itemName;
        [SerializeField] protected string collectVfx = "i_generic_collect"; // set a default collect vfx
        [SerializeField] protected string spawnVfx;
        [SerializeField] protected string collectSfx;
        protected readonly string PLAYER_TAG = TagManager.GetTag(Tag.Player);
        public string ItemName { get => itemName; }

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
            if (spawnVfx != null)
            {
                EffectManager.Instance.SpawnEffect(spawnVfx, transform);
            }
            SpawnEffect();
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            TriggerEffect(collision);
            if (collision.CompareTag(PLAYER_TAG))
            {
                CollectEffect(collision.transform);
                EffectManager.Instance.SpawnEffect(collectVfx, transform.position);
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