using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpecial : WeaponBase 
{
    // base class for all special weapons
    protected override void Startup()
    {
        base.Startup();
        weaponType = WeaponType.Special;
    }
}
