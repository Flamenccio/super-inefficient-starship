using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flamenccio.Enemy;

namespace Flamenccio.Core
{
    /// <summary>
    /// Manages a list of enemies that are spawned during normal gameplay.
    /// </summary>
    public class EnemyList : MonoBehaviour
    {
        public class EnemyTier
        {
            public EnemyTier(int tier)
            {
                Tier = tier;
            }
            
            public int Tier { get; }
            public List<EnemyAttributes> Enemies { get => enemies; }
            private List<EnemyAttributes> enemies = new();
        }
        
        [SerializeField] private string enemiesPath;
        private const int MINIMUM_ENEMY_SPAWNING_LEVEL = 1;
        private List<EnemyTier> tieredEnemyList = new();

        private void Start()
        {
            var rawList =
                Resources.LoadAll<GameObject>(enemiesPath)
                .ToList();
            tieredEnemyList = SortEnemies(rawList);
        }

        public GameObject GetRandomEnemy(int difficulty)
        {
            // Don't spawn anything if level isn't high enough
            if (difficulty < MINIMUM_ENEMY_SPAWNING_LEVEL) return null;

            var randomTier =
                Random.Range(MINIMUM_ENEMY_SPAWNING_LEVEL,
                    difficulty);
            
            // Check if there is a EnemyTier with exact tier
            var hasExactTier = tieredEnemyList.Any(
                e => e.Tier == randomTier);
            var tier = tieredEnemyList[0];

            if (hasExactTier)
            {
                tier = tieredEnemyList.First(e => e.Tier == randomTier);
            }
            else
            {
                // Find greatest tier less than randomTier
                var lastTier = tieredEnemyList[0];

                foreach (var e in tieredEnemyList)
                {
                    if (e.Tier > randomTier)
                    {
                        tier = lastTier;
                        break;
                    }
                    
                    lastTier = e;
                }
            }
            
            // Get random enemy in tier
            var randomEnemy = Random.Range(0, tier.Enemies.Count);

            return tier.Enemies[randomEnemy].gameObject;
        }

        private List<EnemyTier> SortEnemies(List<GameObject> enemies)
        {
            var tieredList = new List<EnemyTier>();

            foreach (var e in enemies)
            {
                if (!e.TryGetComponent(out EnemyAttributes ie))
                {
                    Debug.LogWarning("Could not find type " +
                                     $"{typeof(EnemyAttributes)} in {e}. " +
                                     "Skipping.");
                    continue;
                }
                
                RegisterEnemy(ie, tieredList);
            }
            
            // Finally sort the tieredList by each element's
            // Tier property and return it
            return tieredList.OrderBy(e => e.Tier).ToList();
        }

        private void RegisterEnemy(EnemyAttributes enemy, 
            List<EnemyTier> tieredList)
        {
            // If the tier is less than 0, don't add
            if (enemy.Tier < 0) return;
            
            // Look for existing tier in tieredList
            var existingTier =
                tieredList.Exists(tier => tier.Tier == enemy.Tier);

            if (existingTier)
            {
                var tier = tieredList.First(tier => tier.Tier == enemy
                    .Tier);
                tier.Enemies.Add(enemy);
            }
            else
            {
                var tier = new EnemyTier(enemy.Tier);
                tier.Enemies.Add(enemy);
                tieredList.Add(tier);
            }
        }
    }
}