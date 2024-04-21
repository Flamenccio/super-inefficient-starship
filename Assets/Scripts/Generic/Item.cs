using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Item
{
    // base class for all in-game appearances of items.
    public class Item : MonoBehaviour
    {
        [Tooltip("The effect that plays when the item is collected. Found in the particles folder.")]
        [SerializeField] protected GameObject collectEffect;
        [SerializeField] protected GameObject spawnEffect;

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
                Instantiate(spawnEffect, transform); // summon spawn effect, if it exists.
            }
            SpawnEffect();
        }
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            TriggerEffect(collision);
            if (collision.CompareTag("Player"))
            {
                CollectEffect(collision.transform);
                if (collectEffect != null)
                {
                    Instantiate(collectEffect, transform.position, Quaternion.identity);
                }
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
