using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemniscaticWindCycling : WeaponSpecial
{
    private const int MAX_CHARGES = 2;
    private int charges = MAX_CHARGES;
    private GameObject shockwaveEffect;
    private GameObject attack;
    private LemniscaticWindCyclingBullet attackInstance;
    private bool rechargeUsed = false;
    private const float DURATION = 0.10f;
    private const float SPEED = 50f;
    protected override void Startup()
    {
        base.Startup();
        Name = "Lemniscatic Wind Cycling";
        Desc = "Rushes forward, dealing damage to any enemies in your path. If at least 3 enemies are struck in one dash, grants another charge.";
        Level = 1;
        Rarity = PowerupRarity.Rare;
        cooldown = 3.0f;
        shockwaveEffect = Resources.Load<GameObject>("Prefabs/Effects/LemniscaticWindCyclingShockwave");
        attack = Resources.Load<GameObject>("Prefabs/Bullets/LemniscaticWindCyclingAttack");
    }
    public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
    {
        PlayerMotion pm = PlayerMotion.Instance;

        if (pm.AimRestricted || pm.MovementRestricted) return;

        if (charges <= 0) return;

        pm.RestrictAim(DURATION);
        pm.Blink(DURATION);
        rechargeUsed = false;
        Instantiate(shockwaveEffect, origin, Quaternion.Euler(0f, 0f, aimAngleDeg));
        attackInstance = Instantiate(attack, PlayerMotion.Instance.transform, false).GetComponent<LemniscaticWindCyclingBullet>();
        CameraEffects.instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, pm.transform.position);
        float r = aimAngleDeg * Mathf.Deg2Rad;
        pm.Move(new Vector2(Mathf.Cos(r), Mathf.Sin(r)), SPEED, DURATION);
        charges--;
    }
    public override void Run()
    {
        if (charges < MAX_CHARGES)
        {
            if (cooldownTimer < cooldown)
            {
                cooldownTimer += Time.deltaTime;
            }

            if (cooldownTimer >= cooldown)
            {
                cooldownTimer = 0f;
                charges++;
            }
        }
        if (!rechargeUsed && attackInstance != null && attackInstance.EnemiesHit >= 3)
        {
            rechargeUsed = true;
            charges++;
        }
    }
}
