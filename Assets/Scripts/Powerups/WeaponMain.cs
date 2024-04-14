using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMain : WeaponBase
{
    protected GameObject mainAttack;
    // parent class of all main weapons
    protected override void Startup()
    {
        weaponType = WeaponType.Main;
        base.Startup();
    }
    protected int GetWeaponDamage()
    {
        return mainAttack.GetComponent<BulletControl>().Damage;
    }
    protected float GetWeaponRange()
    {
        return mainAttack.GetComponent<BulletControl>().Range;
    }
    protected float GetWeaponSpeed()
    {
        return mainAttack.GetComponent<BulletControl>().Speed;
    }
    protected float GetWeaponCooldown()
    {
        return Cooldown;
    }
}
