using Flamenccio.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    public class EffectManager : MonoBehaviour
    {
        public enum Effects
        {
            BulletParry,
            BulletImpact,
            Explosion,
            EnemyHit,
            EnemyKill,
            SpecialReplenish,
            PlayerHit,
        }

        public static EffectManager Instance { get; private set; }
        private Dictionary<Effects, GameObject> effectDictionary;

        [SerializeField] private GameObject bulletParry;
        [SerializeField] private GameObject bulletImpact;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject enemyHit;
        [SerializeField] private GameObject enemyKill;
        [SerializeField] private GameObject specialReplenish;
        [SerializeField] private GameObject playerHit;
        [SerializeField] private TrailPool trailPool;

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

            // TODO make better way to do this
            effectDictionary = new()
            {
                { Effects.BulletParry, bulletParry },
                { Effects.BulletImpact, bulletImpact },
                { Effects.Explosion, explosion },
                { Effects.EnemyHit, enemyHit },
                { Effects.EnemyKill, enemyKill },
                { Effects.SpecialReplenish, specialReplenish },
                { Effects.PlayerHit, playerHit },
            };
        }

        private void Start()
        {
            // subscribe to events
            GameEventManager.OnEnemyKill += (v) => SpawnEffect(Effects.EnemyKill, v.EventOrigin);
            GameEventManager.OnEnemyHit += (v) => SpawnEffect(Effects.EnemyHit, v.EventOrigin);
            GameEventManager.OnPlayerHit += (v) => SpawnEffect(Effects.PlayerHit, v.EventTriggerer);
        }

        /// <summary>
        /// Spawn an effect and make it a child of some transform.
        /// </summary>
        public void SpawnEffect(Effects effect, Transform parent)
        {
            if (!effectDictionary.TryGetValue(effect, out GameObject obj))
            {
                Debug.LogError($"Effect {effect} does not exist!");
                return;
            }

            Instantiate(obj, parent);
        }

        /// <summary>
        /// Spawn an effect and place it somewhere in the game world.
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="origin"></param>
        public void SpawnEffect(Effects effect, Vector2 origin)
        {
            SpawnEffect(effect, origin, Quaternion.identity);
        }

        /// <summary>
        /// Spawn an effect and place it somewhere in the game world with a quaternion rotation
        /// </summary>
        public void SpawnEffect(Effects effect, Vector2 origin, Quaternion rotation)
        {
            if (!effectDictionary.TryGetValue(effect, out GameObject obj))
            {
                Debug.LogError($"Effect {effect} does not exist!");
                return;
            }

            Instantiate(obj, origin, rotation);
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
    }
}