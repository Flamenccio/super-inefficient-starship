using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : WeaponSub
{
    private const float DURATION = 5f / 60f;
    private const float SPEED = 50f;
    private GameObject afterImage;
    protected override void Startup()
    {
        base.Startup();
        afterImage = Resources.Load<GameObject>("Prefabs/Effects/Afterimage");
        Desc = "Move quickly in any direction.";
        Name = "Dash";
        cooldown = 1.0f;
        cost = 0;
        Rarity = PowerupRarity.Common;
    }
    public override void Execute(float directionDeg, Vector2 origin, Func<int, bool> deductAmmo)
    {
        Debug.Log("A" + cooldownTimer);
        if (cooldownTimer < cooldown) return; // don't do anything if on cooldown
        Debug.Log("B");
        Directions d = new();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDash, transform.position);
        PlayerMotion.Instance.RestrictMovement(DURATION);
        PlayerMotion.Instance.Move(d.DegreeToVector2(directionDeg), SPEED, DURATION);
        cooldownTimer = 0f;
    }
    public override void Run()
    {
        base.Run();
        if (cooldownTimer <= DURATION)
        {
            Instantiate(afterImage, transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
        }
    }
}
