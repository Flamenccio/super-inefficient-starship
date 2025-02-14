using Flamenccio.Attack;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Flamenccio.Components
{
    /// <summary>
    /// Base class for all attribute classes
    /// </summary>
    public class EntityAttributes : MonoBehaviour
    {
        [Tooltip("Invokes upon dying.")] public UnityEvent Death;

        [Tooltip("Invokes upon taking damage (but NOT dying). Passes in damage taken.")]
        public UnityEvent<int> OnDamage;
        public int CurrentHP { get; private set; }
        public int MaxHP { get => maxHP; }
        public bool Alive { get; private set; }
        [SerializeField] protected int maxHP;

        protected void Awake()
        {
            if (maxHP <= 0)
            {
                Debug.LogWarning($"maxHP is {maxHP}, less than or equal to zero");
            }

            CurrentHP = maxHP;
            Alive = true;
        }

        /// <summary>
        /// Calculates damage from attack
        /// </summary>
        /// <param name="attack">Incoming attack</param>
        public void Damage(Collider2D attack)
        {
            if (!attack.TryGetComponent<BulletControl>(out var bulletControl))
            {
                Debug.Log("no bullet control");
                return;
            }

            Damage(bulletControl.PlayerDamage);
        }

        public void Damage(int damage)
        {
            CurrentHP = Mathf.Clamp(CurrentHP - damage, -1, MaxHP);

            if (CurrentHP <= 0)
            {
                Death?.Invoke();
                Alive = false;
                return;
            }
            
            OnDamage?.Invoke(damage);
        }
    }
}