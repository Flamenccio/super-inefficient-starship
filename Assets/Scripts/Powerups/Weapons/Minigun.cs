using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : WeaponMain
{
    private float frequency = 4f / 60f;
    private float freqTimer = 0f;
    private const int MAX_ROUNDS = 3; // rounds per ammo
    private int rounds = 0;
    private float[] sprayPattern = { 5.0f, -8.3f, 4.6f, -6.2f, 7.4f, 0f }; // slight inacurracies in degrees
    protected override void Startup()
    {
        base.Startup();
        Name = "Minigun";
        Desc = "Continuously fires a stream of bullets on [HOLD]. Highly inaccurate.\nDamage: low\nRange: below average\nSpeed: fast\nCooldown: very short.";
        Rarity = PowerupRarity.Common;
        cooldown = 0.5f;
        cost = 1;
        mainAttack = Resources.Load<GameObject>("Prefabs/Bullets/PlayerMini");
    }
    public override void HoldExecute(float directionDeg, Vector2 origin, Func<int, bool> deductAmmo, float elapsedTime)
    {
        if (freqTimer <= 0f)
        {
            if (rounds <= 0)
            {
                if (!deductAmmo(cost)) return;
                rounds = MAX_ROUNDS;
            }
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShootMini, transform.position);
            Instantiate(mainAttack, origin, Quaternion.Euler(0f, 0f, directionDeg + sprayPattern[rounds]));
            freqTimer = frequency;
            rounds--;
        }
        else freqTimer -= elapsedTime;
    }
    public override void PlayerEffectHoldEnter(Rigidbody2D rb)
    {
        //playerAtt.TemporaryAttributeChange(PlayerAttributes.Attribute.MoveSpeed, 0.33f);
        PlayerMotion.Instance.Move(transform.right, 100f, 0.1f);
    }
    public override void PlayerEffectHoldExit(Rigidbody2D rb)
    {
        playerAtt.RestoreAttributeChange(PlayerAttributes.Attribute.MoveSpeed);
    }
}
