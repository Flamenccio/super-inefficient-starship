using Flamenccio.Components;
using Flamenccio.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Flamenccio.Enemy
{
    /// <summary>
    /// Base class for all enemy behavior classes
    /// </summary>
    public class EnemyBehavior : MonoBehaviour, DistanceBehaviorCull.IDistanceDisable
    {
        public UnityEvent AttackTelegraph;
        protected bool culled = false;
        protected EnemyAttributes attributes;

        // The length of the "blinking" effect before the enemy attacks
        protected const float ATTACK_TELEGRAPH_DURATION = 0.25f;

        protected void Awake()
        {
            if (!TryGetComponent<EnemyAttributes>(out attributes))
            {
                Debug.LogError("No attached EnemyAttributes");
                return;
            }

            OnSpawn();
        }

        protected void FixedUpdate()
        {
            if (culled) return;

            Behavior();
        }

        /// <summary>
        /// Called under Awake()
        /// </summary>
        public virtual void OnSpawn() { }

        /// <summary>
        /// Called whenever enemy dies
        /// </summary>
        public virtual void OnDeath()
        {
            if (!attributes.Alive) return;

            Spawner.Instance.SpawnStarShard(transform.position, attributes.Loot);
            GameEventManager.OnEnemyKill(GameEventManager.CreateGameEvent(transform.position));
            Destroy(gameObject);
        }

        public virtual void OnDamage(int damage)
        {
            // Play hit effect when hit
            GameEventManager.OnEnemyHit.Invoke(GameEventManager.CreateGameEvent(transform.position));
        }

        /// <summary>
        /// Called under FixedUpdate()
        /// </summary>
        protected virtual void Behavior() { }

        public virtual void Disable()
        {
            culled = true;
        }

        public virtual void Enable()
        {
            culled = false;
        }
    }
}
