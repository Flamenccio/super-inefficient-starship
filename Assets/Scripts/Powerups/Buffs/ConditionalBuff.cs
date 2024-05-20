using System;
using System.Collections.Generic;

namespace Flamenccio.Powerup.Buff
{
    public class ConditionalBuff : BuffBase
    {
        protected PlayerAttributes attributes;
        protected Action<List<PlayerAttributes.Attribute>> levelBuff;

        public ConditionalBuff()
        {
            Type = BuffType.Conditional;
            //Level = 0;
        }
        protected virtual void Deactivate()
        {
            Level = 0;
        }
    }
}
