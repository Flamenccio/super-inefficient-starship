using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemniscaticWindCycling : WeaponSpecial
{
    private const int MAX_CHARGES = 2;
    private int charges = MAX_CHARGES;
    private GameObject shockwaveEffect;
    private GameObject attack;
    protected override void Startup()
    {
        base.Startup();
        Name = "Lemniscatic Wind Cycling";
        Desc = "Rushes forward, dealing damage to any enemies in your path.";
        Level = 1;
        Rarity = PowerupRarity.Rare;
        cooldown = 3.0f;
        shockwaveEffect = Resources.Load<GameObject>("Prefabs/Effects/LemniscaticWindCyclingShockwave");
        attack = Resources.Load<GameObject>("Prefabs/Bullets/LemniscaticWindCyclingAttack");
    }
    public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
    {
        if (!PlayerMotion.Instance.RestrictMovement(0.10f)) return;

        if (charges <= 0) return;

        // spawn shockwave at player position
        Instantiate(shockwaveEffect, origin, Quaternion.Euler(0f, 0f, moveAngleDeg));
        Instantiate(attack, PlayerMotion.Instance.transform, false);

        Debug.Log(transform);
        float r = moveAngleDeg * Mathf.Deg2Rad;
        PlayerMotion.Instance.Move(new Vector2(Mathf.Cos(r), Mathf.Sin(r)), 50f, 0.10f);
        charges--;
    }
    public override void Run()
    {
        //base.Run();

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
    }
}
