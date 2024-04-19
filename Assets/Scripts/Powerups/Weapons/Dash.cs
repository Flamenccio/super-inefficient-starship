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
        Desc = "[TAP]: Quickly move a set distance in direction you are moving in.";
        Name = "Dash";
        cooldown = 1.0f;
        cost = 0;
        Rarity = PowerupRarity.Common;
    }
    public override void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin)
    {
        if (cooldownTimer < cooldown) return; // don't do anything if on cooldown
        if (!PlayerMotion.Instance.RestrictMovement(DURATION)) return;
        float r = Mathf.Deg2Rad * moveAngleDeg;
        Vector2 v = new(Mathf.Cos(r), Mathf.Sin(r));
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDash, transform.position);
        PlayerMotion.Instance.Move(v, SPEED, DURATION);
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
