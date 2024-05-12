using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Flamenccio.Core
{
    /// <summary>
    /// <para>The representation of VALUE changes upon context. Read the summary of each event for more info.</para>
    /// <para>EVENTTRIGGERER represents which game object triggered the event, or the game object of interest.</para>
    /// <para>EVENTORIGIN is used when the event triggerer will be destroyed. This is automatically calculated when EVENTTRIGGERER is assigned.</para>
    /// </summary>
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
    public static class GameEventManager
    {
        /*
        these events should be common--no highly specific events unless ABSOLUTELY needed.
            "highly specific" events would be something like "player kills this specific enemy" or "player uses this specific weapon."
        these events must be interactions between game objects: player interaction is already handled by input
        */

        public static Action<GameEventInfo> OnEnemyKill { get; set; }
        public static Action<GameEventInfo> OnEnemySpawn { get; set; }
        public static Action<GameEventInfo> OnEnemyHit { get; set; }
        /// <summary>
        /// VALUE represents damage taken.
        /// </summary>
        public static Action<GameEventInfo> OnPlayerHit { get; set; }
        /// <summary>
        /// VALUE represents points gained.
        /// </summary>
        public static Action<GameEventInfo> OnStarCollect { get; set; }
        public static Action<GameEventInfo> OnStarSpawn { get; set; }
        /// <summary>
        /// VALUE represents points gained.
        /// </summary>
        public static Action<GameEventInfo> OnMiniStarCollect { get; set; }
        /// <summary>
        /// VALUE represents health replenished.
        /// </summary>
        public static Action<GameEventInfo> OnHeartCollect { get; set; }
        /// <summary>
        /// VALUE represents the <b>next</b> level.
        /// </summary>
        public static Action<GameEventInfo> OnLevelUp { get; set; }
        public static Action<GameEventInfo> OnHealthReplenish { get; set; }
        public static Action<GameEventInfo> OnPointGain { get; set; }

        public static GameEventInfo CreateGameEvent(float value, Transform triggerer)
        {
            return new()
            {
                Value = value,
                EventTriggerer = triggerer,
            };
        }
        public static GameEventInfo CreateGameEvent(float value, Vector2 origin)
        {
            return new()
            {
                Value = value,
                EventOrigin = origin,
            };
        }
        public static GameEventInfo CreateGameEvent(Transform triggerer)
        {
            return new()
            {
                EventTriggerer = triggerer
            };
        }
        public static GameEventInfo CreateGameEvent(Vector2 origin)
        {
            return new()
            {
                EventOrigin = origin,
            };
        }
        public static void ClearAllEvents()
        {
            var fields = typeof(GameEventManager).GetProperties(BindingFlags.Static | BindingFlags.Public);

            fields
                .Where(f => f.PropertyType == typeof(Action<GameEventInfo>))
                .ToList()
                .ForEach(f => f.SetValue(null, (Action<GameEventInfo>)((_) => { })));
        }
    }
}
