using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour, IPowerup
{
    public enum WeaponType
    {
        Main,
        Special,
        Defense,
        Support
    };

    protected WeaponType weaponType;
    protected int cost = 1;
    protected float cooldown;
    protected float cooldownTimer;
    protected float holdThreshold;
    protected PlayerAttributes playerAtt;

    public string Name { get; protected set; }
    public string Desc { get; protected set; }
    public int Level { get; protected set; }
    public PowerupRarity Rarity { get; protected set; }

    public WeaponType Type { get => weaponType; }
    public int Cost { get => cost; }
    public float Cooldown { get => cooldown; }

    private void Awake()
    {
        Level = 1;
        Startup();
    }
    protected virtual void Startup()
    {
        cooldownTimer = 0f;
        playerAtt = gameObject.GetComponent<PlayerAttributes>();
    }

    // HACK: rather than passing a new deductAmmo method every Execute() call, why not cache it when an instance comes awake?
    public virtual void Execute(float directionDeg, Vector2 origin, Func<int, bool> deductAmmo) { }
    public virtual void Run()
    {
        if (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
        }
    }
    public virtual void PlayerEffectTap(Rigidbody2D rb) { } // any effects that a weapon may do to a player on TAP
    public virtual void PlayerEffectHoldEnter(Rigidbody2D rb) { } // any effects that a weapon do to a player when entering HOLD attacks.
    public virtual void PlayerEffectHold(Rigidbody2D rb) { } // any effects that a weapon do to a player while HOLD is performed.
    public virtual void PlayerEffectHoldExit(Rigidbody2D rb) { } // any effects that a weapon do to a player when exiting HOLD attacks.
    public void LevelChange(int levels)
    {
        if (Level < -levels) return; // if applying this change makes the level negative, don't
        Level += levels;
    }
}
