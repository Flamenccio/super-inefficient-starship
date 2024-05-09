using System;
using UnityEngine;

namespace Flamenccio.Core
{
    public class GameEventManager : MonoBehaviour
    {
        public static Action<Vector2> OnEnemyKilled { get; set; }
        public static Action<Transform> OnEnemySpawned { get; set; }
        public static Action<Transform> OnEnemyHit { get; set; }
        public static Action<Transform> OnPlayerHit { get; set; }
    }
}
