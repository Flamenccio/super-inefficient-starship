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
        [Tooltip("What should be notified when this Enemy dies?")] public UnityEvent Death;
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

        public void Damage(int damage)
        {
            CurrentHP = Mathf.Clamp(CurrentHP - damage, -1, MaxHP);

            if (CurrentHP <= 0)
            {
                Alive = false;
                Death?.Invoke();
            }
        }
    }
}