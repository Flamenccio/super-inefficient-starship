using Flamenccio.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Enemy
{
    public class EnemyAttributes : EntityAttributes
    {
        public int Tier { get => tier; }
        public List<GameObject> Attacks = new();
        [SerializeField] protected int tier;
    }
}
