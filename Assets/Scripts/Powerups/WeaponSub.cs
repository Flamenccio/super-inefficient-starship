using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSub : WeaponBase // parent class for all sub weapons
{
    protected override void Startup()
    {
        base.Startup();
        weaponType = WeaponType.Sub;
    }
}
