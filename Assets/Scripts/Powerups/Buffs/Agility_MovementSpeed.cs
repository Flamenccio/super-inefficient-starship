using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agility_MovementSpeed : UnconditionalBuff
{
    public Agility_MovementSpeed()
    {
        Name = "Thruster Upgrade";
        Level = 1;
        Class = BuffClass.Agility;
        Func<int, float> f1 = (int level) => { return level * 0.10f; };
        Desc = ("[LEVEL " + Level.ToString() + "]: Move " + f1(Level) + "% faster.");
        buffs.Add(new StatBuff(PlayerAttributes.Attribute.MoveSpeed, f1));
    }
}
