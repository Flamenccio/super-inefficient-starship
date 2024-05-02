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
            StarSpawn,
            StarCollect,
            WallSpawn,
            ItemCollect,
            ItemDestroy,
            SpecialReplenish
        }

        public static EffectManager Instance { get; private set; }
        private Dictionary<Effects, GameObject> effectDictionary;

        [SerializeField] private GameObject bulletParry;
        [SerializeField] private GameObject bulletImpact;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject enemyHit;
        [SerializeField] private GameObject enemyKill;
        [SerializeField] private GameObject specialReplenish;

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
            };

        }
        public void SpawnEffect(Effects effect, Transform parent)
        {
            if (!effectDictionary.TryGetValue(effect, out GameObject obj))
            {
                Debug.LogError($"Effect {effect} does not exist!");
                return;
            }

            Instantiate(obj, parent);
        }
        public void SpawnEffect(Effects effect, Vector2 origin)
        {
            SpawnEffect(effect, origin, Quaternion.identity);
        }
        public void SpawnEffect(Effects effect, Vector2 origin, Quaternion rotation)
        {
            if (!effectDictionary.TryGetValue(effect, out GameObject obj))
            {
                Debug.LogError($"Effect {effect} does not exist!");
                return;
            }

            Instantiate(obj, origin, rotation);
        }
    }
}
