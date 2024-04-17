using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour, IPowerup
{
    public enum WeaponType
    {
        Main,
        Sub,
        Special,
        Support
    };

    protected WeaponType weaponType;
    protected int cost = 1;
    protected float cooldown;
    protected float cooldownTimer;
    protected float holdThreshold;
    protected PlayerAttributes playerAtt;
    public bool AimAssisted { get; protected set; } // whether the weapon should aim towards the aim assist target on TAP

    public string Name { get; protected set; }
    public string Desc { get; protected set; }
    public int Level { get; protected set; }
    public PowerupRarity Rarity { get; protected set; }
    public WeaponType Type { get => weaponType; }
    public int Cost { get => cost; }
    public float Cooldown { get => cooldown; }
    protected Func<int, bool> consumeAmmo;

    private void Awake()
    {
        Level = 1;
        AimAssisted = false;
        Startup();
    }
    protected virtual void Start()
    {
        
    }
    protected virtual void Startup()
    {
        cooldownTimer = 0f;
        if (!gameObject.TryGetComponent<PlayerAttributes>(out playerAtt))
        {
            Debug.LogError("ERROR: could not find a PlayerAttributes class in player.");
            throw new NullReferenceException("ERROR: could not find a PlayerAttributes class in player.");
        }
        consumeAmmo = playerAtt.UseAmmo;
    }
    public virtual void Run()
    {
        if (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
        }
    }
    /// <summary>
    /// What weapon does when player taps button.
    /// </summary>
    /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
    /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
    public virtual void Tap(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
    /// <summary>
    /// The player effect that is called <b>once</b> when player enters hold attack.
    /// </summary>
    /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
    /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
    public virtual void HoldEnter(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
    /// <summary>
    /// The player effect that is run continuously while player is holding attack button.
    /// </summary>
    /// <param name="aimAngleDeg">Where player is <b>aiming</b> in degrees (right stick on gamepads).</param>
    /// <param name="moveAngleDeg">Where player is <b>moving</b> in degrees (left stick on gamepads).</param>
    public virtual void Hold(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
    /// <summary>
    /// The player effect that is called <b>once</b> when player exits hold attack.
    /// </summary>
    public virtual void HoldExit(float aimAngleDeg, float moveAngleDeg, Vector2 origin) { }
    public void LevelChange(int levels)
    {
        if (Level < -levels) return; // if applying this change makes the level negative, don't
        Level += levels;
    }
}
