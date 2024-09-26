using Flamenccio.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Enemy
{
    /// <summary>
    /// Base class for all enemy behavior classes
    /// </summary>
    public class EnemyBehavior : MonoBehaviour, DistanceBehaviorCull.IDistanceDisable
    {
        protected bool culled = false;

        protected void Awake()
        {
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
        public virtual void OnDeath() { }

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
