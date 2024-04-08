using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public enum PowerUpType
{
    Buff,
    Augment,
    Addon
} 
public interface PowerupInterface
{
    int MaxUpgrade { get; }
    /// <summary>
    /// The amount <c>r</c> in which <c>r</c>/<c>t</c> is the probability of pulling this powerup from the loot table and <c>t</c> is the total number of items in the loot table.
    /// </summary>
    int Rarity { get; }
    PowerUpType PowerupType { get; }
    string Title { get; }
}
[CreateAssetMenu(fileName = "New Buff", menuName = "Buff Template", order = 2)]
public class BuffTemplate : ScriptableObject, PowerupInterface 
{
    [Serializable]
    public struct BuffEffects
    {
        /// <summary>
        /// What stat to affect.
        /// </summary>
        /// <summary>
        /// How much to change the specified stat in percentage starting at tier 1.
        /// </summary>
        [SerializeField] private List<float> enhanceTiers;
        [SerializeField] private List<BuffConditions> conditions;
        public List<float> EnhanceTiers { get => enhanceTiers; }
        public List<BuffConditions> Conditions { get => conditions; }
        public bool Active()
        {
            if (conditions.Count == 0) return true;
            foreach (BuffConditions con in conditions)
            {
                if (!con.ConditionMet()) return false;
            }
            return true;
        }
    }
    public enum PrerequisitesType
    {
        HpPercentage,
        SpPercentage,
        NearbyWalls,
        NearbyEnemies,
        TimeRemaining,
        GoalDistance,
        AmmoCount,
        TotalMods,
        HpAmount,
    }
    public enum PrerequisitesCondition
    {
        LessThan,
        LessThanOrEqual,
        MoreThan,
        MoreThanOrEqual,
        Equal,
    }
    [Serializable]
    public struct BuffConditions
    {
        [SerializeField] private PrerequisitesType prereqType;
        [SerializeField] private PrerequisitesCondition prereqCondition;
        private float prereqProgress;
        [SerializeField] private float prereqValue;
        public float PrereqProgress
        {
            get => prereqProgress;
            set
            {
                prereqProgress = value;
                if (prereqProgress < 0) prereqProgress = 0;
            }
        }
        public PrerequisitesType PrereqType { get => prereqType; }
        public PrerequisitesCondition PrereqCondition { get => prereqCondition; }
        public bool ConditionMet()
        {
            switch (prereqCondition)
            {
                case PrerequisitesCondition.LessThan:
                    return (prereqProgress < prereqValue);
                    
                case PrerequisitesCondition.LessThanOrEqual:
                    return (prereqProgress <= prereqValue);

                case PrerequisitesCondition.MoreThan:
                    return (prereqProgress > prereqValue);

                case PrerequisitesCondition.MoreThanOrEqual:
                    return (prereqProgress >= prereqValue);

                case PrerequisitesCondition.Equal:
                    return (prereqProgress <= prereqValue);

                default:
                    return false;
            }
        }
    }
    [SerializeField] private int upgradeLevel;
    [SerializeField] private int maxUpgrade;
    [SerializeField] private int rarity;
    [SerializeField] private string title;
    [SerializeField] private List<BuffEffects> effects = new List<BuffEffects>();
    
    public List<BuffEffects> Effects { get => effects; }
    public int MaxUpgrade { get => maxUpgrade; }
    public int UpgradeLevel { get => upgradeLevel; }
    public int Rarity { get => rarity; }
    public PowerUpType PowerupType { get => PowerUpType.Buff; }
    public string Title { get => title; }

    public void ChangeUpgradeLevel(int change)
    {
        upgradeLevel += change;
        if (upgradeLevel > maxUpgrade) upgradeLevel = maxUpgrade;
        if (upgradeLevel < 0 ) upgradeLevel = 0;
    }

}
