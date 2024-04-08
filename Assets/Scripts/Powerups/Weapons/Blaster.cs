using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : WeaponMain
{
    protected override void Startup()
    {
        base.Startup();
        Name = "Blaster";
        Desc = "Fires a short-ranged bullet on [TAP].\nDamage: low\nRange: low\nSpeed: Very fast\nCooldown: Very short";
        Rarity = PowerupRarity.Common;
        cost = 1;
        cooldown = 8f / 60f;
        mainAttack = Resources.Load<GameObject>("Prefabs/Bullets/Player");
    }
    public override void Execute(float directionDeg, Vector2 origin, Func<int, bool> deductAmmo)
    {
        if (cooldownTimer < cooldown) return; // don't do anything if on cooldown
        if (!deductAmmo(cost)) return; // don't do anything if there is not enough ammo
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShoot, transform.position);
        Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, directionDeg));
        cooldownTimer = 0f;
    }
}
