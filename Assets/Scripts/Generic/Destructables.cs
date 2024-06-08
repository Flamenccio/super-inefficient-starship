using UnityEngine;

namespace Flamenccio.Core
{
    /// <summary>
    /// Base class for all objects that can be destroyed.
    /// </summary>
    public class Destructables : MonoBehaviour
    {
        public int MaxHP { get => maxHP; set => maxHP = value; }
        public int CurrentHP { get => currentHP; }
        public int Loot { get => loot; set => loot = value; }
        [SerializeField] protected int maxHP = 0;
        [SerializeField] protected int loot = 0;
        protected int currentHP = 0;

        protected virtual void Start()
        {
            currentHP = maxHP;
        }
    }
}
