using Flamenccio.Powerup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.DataHandling
{
    public class WeaponLootTable : MonoBehaviour
    {
        [SerializeField] private string weaponObjectPath;
        private const float MIN_LUCK = 0f;
        private const float MAX_LUCK = 100f;
        private List<GameObject> weaponTable = new();

        private void Start()
        {
            LoadWeapons();
        }

        /// <summary>
        /// Load all weapon objects from Resources
        /// </summary>
        private void LoadWeapons()
        {
            weaponTable = Resources.LoadAll<GameObject>(weaponObjectPath).ToList();
        }

        /// <summary>
        /// Returns a random weapon
        /// </summary>
        /// <param name="luck">Affects the chances of a higher-rarity weapon appearing</param>
        public GameObject GetRandomWeapon(float luck)
        {
            // TODO all weapons have an equal chance for now, fix this later
            luck = Mathf.Clamp(luck, MIN_LUCK, MAX_LUCK);
            int pick = UnityEngine.Random.Range(0, weaponTable.Count);

            return weaponTable[pick];
        }

        /// <summary>
        /// Returns a random weapon
        /// </summary>
        /// <param name="luck">Affects the chances of a higher-rarity weapon appearing</param>
        /// <param name="exceptions">Removes excepted weapons from being chosen</param>
        public GameObject GetRandomWeapon(float luck, List<GameObject> exceptions)
        {
            // TODO all weapons have an equal chance for now, fix this later
            luck = Mathf.Clamp(luck, MIN_LUCK, MAX_LUCK);
            var modifiedTable = weaponTable.Except(exceptions).ToList();
            int pick = UnityEngine.Random.Range(0, modifiedTable.Count);

            return modifiedTable[pick];
        }

        /// <summary>
        /// Gets the percent chance of a rarity appearing
        /// </summary>
        /// <param name="rarity">Rarity type</param>
        /// <param name="luck">Increases the chances of a higher rarity weapon appearing</param>
        private float GetRarityChance(PowerupRarity rarity, float luck)
        {
            return rarity switch
            {
                PowerupRarity.Common => GetCommonChance(luck),
                PowerupRarity.Rare => GetRareChance(luck),
                PowerupRarity.Legendary => GetLegendChance(luck),
                PowerupRarity.Relic => GetRelicChance(luck),
                _ => 0f,// placeholder
            };
        }

        private PowerupRarity GetRandomRarity(float luck)
        {
            float roll = UnityEngine.Random.Range(0f, 100f);
            var chances = GetAllRarityChances(luck);
            var indexes = GetRarityIndexes(chances, 0f, 100f);
            int rarities = Enum.GetNames(typeof(PowerupRarity)).Length;

            for (int i = 0; i < rarities; i++)
            {
                if (roll < indexes[i])
                {
                    return (PowerupRarity)(i - 1);
                }
            }

            return (PowerupRarity)rarities;
        }

        /// <summary>
        /// Calculates all rarity chances and places it an ordered array
        /// </summary>
        /// <param name="luck">Increases the chances of a higher rarity weapon</param>
        /// <returns>
        ///     An float array where each element corresponds to chance of getting a weapon rarity.
        ///     <para>
        ///         Pass the PowerupRarity as the index to find its chance.
        ///     </para>
        /// </returns>
        private float[] GetAllRarityChances(float luck)
        {
            int rarities = Enum.GetNames(typeof(PowerupRarity)).Length;
            var result = new float[rarities];

            for (int i = 0; i < rarities; i++)
            {
                result[i] = GetRarityChance((PowerupRarity)i, luck);
            }

            return result;
        }

        private float[] GetRarityIndexes(float[] chances, float minRoll, float maxRoll)
        {
            if (minRoll >= maxRoll)
            {
                Debug.LogError("minRoll cannot be larger than or equal to maxRoll!");
                return null;
            }

            int rarities = Enum.GetNames(typeof(PowerupRarity)).Length;
            float[] indexes = new float[rarities];
            float range = Mathf.Abs(maxRoll - minRoll);
            float currentIndex = minRoll;

            for (int i = 0; i < rarities; i++)
            {
                indexes[i] = currentIndex;
                float rarityRange = range * chances[i];
                currentIndex += rarityRange;
            }

            return indexes;
        }

        private float GetCommonChance(float luck)
        {
            return 1f - (GetRareChance(luck) + GetLegendChance(luck) + GetRelicChance(luck));
        }
        
        private float GetRareChance(float luck)
        {
            return Mathf.Log(luck + 10f) / 18f;
        }

        private float GetLegendChance(float luck)
        {
            return Mathf.Log(luck + 7f) / 26f;
        }

        private float GetRelicChance(float luck)
        {
            return Mathf.Log(luck + 4f) / 34f;
        }
    }
}
