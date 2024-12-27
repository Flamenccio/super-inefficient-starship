using Flamenccio.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Enemy
{
    public class EnemyAttributes : EntityAttributes
    {
        public int Tier { get => tier; }
        public int Loot { get => loot; }
        [SerializeField] protected int tier;
        [SerializeField] protected int loot;
    }
}
