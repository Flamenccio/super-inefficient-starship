using Flamenccio.Utility;
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
        public object Value { get; set; }
        public object Value2 { get; set; }

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

    /// <summary>
    /// Manages game events: delegates that classes can subscribe to and invoke.
    /// </summary>
    public static class GameEventManager
    {
        /// <summary>
        /// Is this class instanced yet?
        /// </summary>
        public static bool Instanced { get => true; }

        /*
        these events should be common--no highly specific events unless ABSOLUTELY needed.
            "highly specific" events would be something like "player kills this specific enemy" or "player uses this specific weapon."
        these events must be interactions between game objects: player interaction is already handled by input
        */

        /// <summary>
        /// VALUE represents star shards dropped.
        /// </summary>
        public static Action<GameEventInfo> OnEnemyKill { get; set; }

        /// <summary>
        /// VALUE represents enemy tier.
        /// </summary>
        public static Action<GameEventInfo> OnEnemySpawn { get; set; }

        /// <summary>
        /// No VALUE needed.
        /// </summary>
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
        /// No VALUE needed.
        /// </summary>
        public static Action<GameEventInfo> OnItemBoxCollect { get; set; }

        /// <summary>
        /// VALUE represents the <b>next</b> level.
        /// </summary>
        public static Action<GameEventInfo> OnLevelUp { get; set; }

        /// <summary>
        /// VALUE represents the total health gained.
        /// </summary>
        public static Action<GameEventInfo> OnHealthReplenish { get; set; }

        /// <summary>
        /// VALUE represents the total points gained.
        /// </summary>
        public static Action<GameEventInfo> OnPointGain { get; set; }

        /// <summary>
        /// VALUE represents the object spawned; reference ObjectType enum.
        /// </summary>
        public static Action<GameEventInfo> OnObjectSpawn { get; set; }

        /// <summary>
        /// VALUE represents the object destroyed; reference ObjectType enum.
        /// </summary>
        public static Action<GameEventInfo> OnObjectDestroy { get; set; }

        /// <summary>
        /// VALUE represents the game object of the equipping weapon
        /// </summary>
        public static Action<GameEventInfo> EquipWeapon { get; set; }

        /// <summary>
        /// Called when control scheme is changed.
        /// Paramter is the new control scheme.
        /// </summary>
        public static Action<InputManager.ControlScheme> OnControlSchemeChange { get; set; }

        /// <summary>
        /// Create a GameEventInfo struct with the following values.
        /// </summary>
        /// <param name="value">Depends on the event being triggered.</param>
        /// <param name="triggerer">The transform that triggered the event.</param>
        /// <returns>A new GameEventInfo struct.</returns>
        public static GameEventInfo CreateGameEvent(object value, Transform triggerer)
        {
            return new()
            {
                Value = value,
                EventTriggerer = triggerer,
            };
        }

        /// <summary>
        /// Create a GameEventInfo struct with the following values.
        /// </summary>
        /// <param name="value">Depends on the event being triggered.</param>
        /// <param name="origin">The location where the event happened.</param>
        /// <returns>A new GameEventInfo struct.</returns>
        public static GameEventInfo CreateGameEvent(object value, Vector2 origin)
        {
            return new()
            {
                Value = value,
                EventOrigin = origin,
            };
        }

        /// <summary>
        /// Create a GameEventInfo struct with the following values.
        /// </summary>
        /// <param name="triggerer">The transform that triggered the event.</param>
        /// <returns>A new GameEventInfo struct.</returns>
        public static GameEventInfo CreateGameEvent(Transform triggerer)
        {
            return new()
            {
                EventTriggerer = triggerer
            };
        }

        /// <summary>
        /// Create a GameEventInfo struct with the following values.
        /// </summary>
        /// <param name="origin">The location where the event happened.</param>
        /// <returns>A new GameEventInfo struct.</returns>
        public static GameEventInfo CreateGameEvent(Vector2 origin)
        {
            return new()
            {
                EventOrigin = origin,
            };
        }

        /// <summary>
        /// Remove all subscriptions from all events.
        /// </summary>
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