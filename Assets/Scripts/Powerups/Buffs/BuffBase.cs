using System;
using System.Collections.Generic;

namespace Flamenccio.Powerup.Buff
{
    public enum BuffClass // TODO soon to be deleted
    {
        Brutality,
        Vitality,
        Agility
    };

    /// <summary>
    /// Base class for all buffs.
    /// </summary>
    public class BuffBase : IPowerup
    {
        public string Name { get; protected set; }

        public int Level
        {
            get => level;
            set
            {
                if (value < 0) return;

                int copy = level;
                level = value;
                OnLevelChange(value, copy);
            }
        }

        public PowerupRarity Rarity { get; protected set; }
        public List<StatBuff> Buffs { get => buffs; }
        public BuffType Type { get; protected set; }
        public BuffClass Class { get; protected set; }
        protected List<StatBuff> buffs = new();
        private int level;

        public class StatBuff
        {
            public StatBuff(PlayerAttributes.Attribute a, Func<int, float> p)
            {
                affectedAttribute = a;
                PercentChange = p;
            }

            public PlayerAttributes.Attribute affectedAttribute;

            /// <summary>
            /// Tells how much to change a stat given the buff's level (the int parameter).
            /// </summary>
            public Func<int, float> PercentChange { get; protected set; }
        }

        public enum BuffType
        {
            Unconditional,
            Conditional,
            Event
        };

        /// <summary>
        /// Called in Update().
        /// </summary>
        public virtual void Run()
        {
            // this function is meant to be run in Update()
        }

        public virtual float GetPercentChangeOf(PlayerAttributes.Attribute a)
        {
            float total = 0f;
            foreach (StatBuff buff in Buffs)
            {
                if (buff.affectedAttribute == a)
                {
                    total += buff.PercentChange(Level);
                }
            }
            return total;
        }

        public List<PlayerAttributes.Attribute> GetAffectedAttributes()
        {
            List<PlayerAttributes.Attribute> temp = new();
            foreach (StatBuff buff in Buffs)
            {
                temp.Add(buff.affectedAttribute);
            }
            return temp;
        }

        /// <summary>
        /// This method is called <b>before</b> this buff's level changes.
        /// </summary>
        protected virtual void OnLevelChange(int newLevel, int oldLevel)
        {
        }

        public virtual void LevelUp()
        {
            Level++;
        }

        /// <summary>
        /// This method is called when this buff is removed from the buff list.
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        protected virtual void OnCreate()
        {
        }
    }
}