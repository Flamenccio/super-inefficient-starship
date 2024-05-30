using System;
using System.Collections.Generic;

namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// Base class for all conditional buffs.
    /// </summary>
    public class ConditionalBuff : BuffBase
    {
        protected PlayerAttributes attributes;
        protected Action<List<PlayerAttributes.Attribute>> levelBuff;

        public ConditionalBuff()
        {
            Type = BuffType.Conditional;
        }

        protected virtual void Deactivate()
        {
            Level = 0;
        }
    }
}