using Flamenccio.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Flamenccio.Effects.Visual
{
    public class EffectManager : MonoBehaviour
    {
        [Serializable]
        public struct EffectObject
        {
            [field: SerializeField, Tooltip("Name of the effect. Must be in PascalCase, have no spaces.")] public string Name { get; set; }
            [field: SerializeField] public GameObject Effect { get; set; }
        }

        public static EffectManager Instance { get; private set; }

        [SerializeField] private GameObject collectedStarShard;
        [SerializeField] private TrailPool trailPool;
        [SerializeField] private List<EffectObject> effects = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            // subscribe to events
            GameEventManager.OnEnemyKill += (v) => SpawnEffect("EnemyKill", v.EventOrigin);
            GameEventManager.OnEnemyHit += (v) => SpawnEffect("EnemyHit", v.EventOrigin);
            GameEventManager.OnPlayerHit += (v) => SpawnEffect("PlayerHit", v.EventTriggerer);
        }

        /// <summary>
        /// Spawn an effect and place it somehwere in the game world. The effect will have a rotation of 0 degrees.
        /// </summary>
        /// <param name="effectName">The name of the effect.</param>
        /// <param name="origin">Where to place the effect.</param>
        public void SpawnEffect(string effectName, Vector2 origin)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var effect = effects.Find(x => x.Name.Equals(effectName)).Effect;

            if (effect == null)
            {
                Debug.LogWarning($"Could not find an effect with name {effectName}.");
                return;
            }

            Instantiate(effect, origin, Quaternion.identity);

            sw.Stop();
            Debug.Log($"Elapsed time: {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Spawn an effect and place it somewhere in the game world with a rotation.
        /// </summary>
        /// <param name="effectName">The name of the effect.</param>
        /// <param name="origin">Where to place the effect.</param>
        /// <param name="rotation">Rotation of the effect.</param>
        public void SpawnEffect(string effectName, Vector2 origin, Quaternion rotation)
        {
            var effect = effects.Find(x => x.Name.Equals(effectName)).Effect;

            if (effect == null)
            {
                Debug.LogWarning($"Could not find an effect with name {effectName}.");
                return;
            }

            Instantiate(effect, origin, rotation);
        }

        /// <summary>
        /// Spawn an effect and place it somewhere in the game world. The effect will have a rotation of 0 degrees.
        /// <para>Could be faster than passing in a string name.</para>
        /// </summary>
        /// <param name="effectId">The position of the effect to spawn in the effects list.</param>
        /// <param name="origin">Where to place the effect.</param>
        public void SpawnEffect(int effectId, Vector2 origin)
        {
            if (effectId > effects.Count) return;

            Instantiate(effects[effectId].Effect, origin, Quaternion.identity);
        }

        /// <summary>
        /// Spawn an effect and place it somewhere in the game world with a given rotation.
        /// <para>Could be faster than passing in a string name.</para>
        /// </summary>
        /// <param name="effectId">The position of the effect to spawn in the effects list.</param>
        /// <param name="origin">Where to place the effect.</param>
        /// <param name="rotation">Rotation of the effect.</param>
        public void SpawnEffect(int effectId, Vector2 origin, Quaternion rotation)
        {
            if (effectId > effects.Count)
            {
                Debug.LogWarning($"Could not find an effect with ID {effectId}");
                return;
            }

            Instantiate(effects[effectId].Effect, origin, rotation);
        }

        /// <summary>
        /// Spawn an effect under a parent GameObject.
        /// </summary>
        /// <param name="effectName">Name of effect.</param>
        /// <param name="parent">Parent GameObject Transform.</param>
        public void SpawnEffect(string effectName, Transform parent)
        {
            var effect = effects.Find(x => x.Name.Equals(effectName)).Effect;

            if (effect == null)
            {
                Debug.LogWarning($"Could not find an effect with name {effectName}.");
                return;
            }

            Instantiate(effect, parent);
        }

        /// <summary>
        /// Spawn an effect under a parent GameObject.
        /// </summary>
        /// <param name="effectId">Position of effect in effect list.</param>
        /// <param name="parent">Parent GameObject Transform.</param>
        public void SpawnEffect(int effectId, Transform parent)
        {
            if (effectId > effects.Count)
            {
                Debug.LogWarning($"Could not find an effect with ID {effectId}.");
                return;
            }

            var effect = effects[effectId].Effect;
            Instantiate(effect, parent);
        }

        /// <summary>
        /// Spawn a trail and place it somewhere in the game world.
        /// </summary>
        /// <returns>The trail spawned.</returns>
        public Trail SpawnTrail(TrailPool.Trails trail, Vector2 origin)
        {
            Trail t = trailPool.TrailsPool[trail].Get();
            t.transform.position = origin;
            return t;
        }

        public void SpawnCollectedStarShard(Vector2 origin, Transform target)
        {
            Instantiate(collectedStarShard, origin, Quaternion.identity).GetComponent<StarFly>().FlyTo(target);
        }
    }
}