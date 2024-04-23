using System;
using System.Collections.Generic;

namespace Flamenccio.Powerup
{
    public enum BuffClass
    {
        Brutality,
        Vitality,
        Agility
    };
    public class BuffBase : IPowerup
    {
        public string Name { get; protected set; }
        public string Desc { get; protected set; }
        public int Level { get; protected set; }
        public PowerupRarity Rarity { get; protected set; }
        public List<StatBuff> Buffs { get => buffs; }
        public BuffType Type { get; protected set; }
        public BuffClass Class { get; protected set; }
        protected List<StatBuff> buffs = new();

        public class StatBuff
        {
            public StatBuff(PlayerAttributes.Attribute a, Func<int, float> p)
            {
                affectedAttribute = a;
                PercentChange = p;
            }
            public PlayerAttributes.Attribute affectedAttribute;
            public Func<int, float> PercentChange { get; protected set; }
        }
        public enum BuffType
        {
            Unconditional,
            Conditional,
            Event
        };
        public virtual void Run()
        {
            // this function is meant to be run in Update()
        }
        public void LevelChange(int levels)
        {
            if (Level < -levels) return; // if applying this change makes the level negative, don't
            Level += levels;
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
    }
}
