using System;
using UnityEngine;

namespace Flamenccio.Core
{
    public struct GameEventInfo
    {
        public float Value { get; set; }
        public Transform EventTriggerer
        {
            readonly get => eventTriggerer;
            set
            {
                eventTriggerer = value;
                EventOrigin = value.position;
            }
        }
        public Vector2 EventOrigin { get; set; } // if the transform is not available (e.g. when the game object is destroyed), use this
        private Transform eventTriggerer;
    }
    public class GameEventManager : MonoBehaviour
    {
        public static Action<GameEventInfo> OnEnemyKilled { get; set; }
        public static Action<GameEventInfo> OnEnemySpawned { get; set; }
        public static Action<GameEventInfo> OnEnemyHit { get; set; }
        public static Action<GameEventInfo> OnPlayerHit { get; set; }
    }
}
