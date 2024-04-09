using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public enum Attribute
    {
        MaxAmmo,
        MoveSpeed,
        MaxHP,
        KillPointBonus,
        MainDamageBonus,
    };
    private float[] attributeBonuses; // array of total percent increase of each attribute
    private uint alteredAttributes; // a bitmask representing what attributes have been temporarily altered

    private int baseAmmo = 10;
    private int baseMaxAmmo = 50;
    private int baseMaxHP = 3;
    private int baseKillPoints = 0;
    private float baseKillPointBonus = 1.25f;
    private float baseMoveSpeed = 4.8f;
    private float baseMainWeaponDamageBonus = 0f;
    
    private Dictionary<Attribute, double> attributeValues = new(); // current attribute values used for calculations and stuff
    private Dictionary<Attribute, double> baseAttributeValues = new();

    public int Ammo { get; private set; }
    public int MaxAmmo { get => (int)attributeValues[Attribute.MaxAmmo]; private set => attributeValues[Attribute.MaxAmmo] = value; }
    public int HP { get; private set; }
    public float MoveSpeed { get => (float)attributeValues[Attribute.MoveSpeed]; private set => attributeValues[Attribute.MoveSpeed] = value; }
    public int MaxHP { get => (int)attributeValues[Attribute.MaxHP]; private set => attributeValues[Attribute.MaxHP] = value; }
    public int KillPoints { get; private set; }
    public float KillPointBonus {  get => (float)attributeValues[Attribute.MaxHP]; private set => attributeValues[Attribute.KillPointBonus] = value; }
    public float WeaponRange { get; private set; }
    public int MainWeaponDamage { get; private set; }
    public float MainWeaponDamageBonus {  get => (float)attributeValues[Attribute.MainDamageBonus]; private set => attributeValues[Attribute.MainDamageBonus] = value; }
    private void Awake()
    {
        attributeBonuses = new float[Enum.GetNames(typeof(Attribute)).Length];

        MaxHP = baseMaxHP; // default values
        HP = MaxHP;
        MoveSpeed = baseMoveSpeed;
        KillPointBonus = baseKillPointBonus;
        Ammo = baseAmmo;

        // populate bonus dictionary
        /*
        attributeValues.Add(Attribute.MaxAmmo, baseMaxAmmo);
        attributeValues.Add(Attribute.MoveSpeed, baseMoveSpeed);
        attributeValues.Add(Attribute.MaxHP, baseMaxHP);
        attributeValues.Add(Attribute.KillPointBonus, baseKillPointBonus);
        attributeValues.Add(Attribute.MainDamageBonus, baseMainWeaponDamageBonus);
        */

        // populate base dictionary
        baseAttributeValues.Add(Attribute.MaxAmmo, baseMaxAmmo);
        baseAttributeValues.Add(Attribute.MaxHP, baseMaxHP);
        baseAttributeValues.Add(Attribute.MoveSpeed, baseMoveSpeed);
        baseAttributeValues.Add(Attribute.KillPointBonus, baseKillPointBonus);
        baseAttributeValues.Add(Attribute.MainDamageBonus, baseMainWeaponDamageBonus);

        foreach (Attribute a in baseAttributeValues.Keys)
        {
            attributeValues[a] = baseAttributeValues[a];
        }
    }
    /*
    private void Start()
    {

    }
    
    private void Update()
    {
    }
    */
    public bool UseAmmo(int ammo)
    {
        if (Ammo - ammo < 0)
        {
            return false;
        }
        Ammo -= ammo; // use ammo
        return true;
    }
    public void AddAmmo(int ammo)
    {
        if (Ammo + ammo > MaxAmmo)
        {
            return;
        }
        Ammo += ammo;
    }
    public void AddKillPoints(int killPoints)
    {
        KillPoints += killPoints;
    }
    public int UseKillPoints()
    {
        int x = KillPoints;
        KillPoints = 0;
        return x;
    }
    public bool ChangeLife(int life)
    {
        if (HP + life > MaxHP) return false;
        HP = Mathf.Clamp(HP + life, 0, MaxHP);

        return true;
    }
    /// <summary>
    /// Takes some attribute <b>a</b> and a list of buffs <b>b</b>. From the list, recalculates attribute bonus. Also applies the bonus to the attribute.
    /// </summary>
    public void RecompileBonus(Attribute a, List<BuffBase> b)
    {
        if ((alteredAttributes | ((uint)1 << (int)a)) == alteredAttributes) RestoreAttributeChange(a, b); // if this value is already changed, restore it.
        attributeBonuses[(int)a] = 0f;
        foreach (BuffBase bb in b)
        {
            attributeBonuses[(int)a] = bb.GetPercentChangeOf(a);
        }
        ApplyBonus(a);
    }
    /// <summary>
    /// Changes the actual attribute value based on current attribute bonuses.
    /// </summary>
    public void ApplyBonus(Attribute a)
    {
        float bonus = attributeBonuses[(int)a]; // get the percent change for specific attribute
        baseAttributeValues.TryGetValue(a, out double y); // now get the base value for specific attribute
        attributeValues[a] = (1 + bonus) * y; // calculate final attribute value and apply it
        Debug.Log(attributeValues[a]);
        Debug.Log(MoveSpeed);
    }
    /// <summary>
    /// Temporarily change an attribute's value by some percent.<para>The final value is used in calculations.</para>
    /// </summary>
    /// <param name="a">Attribute to affect.</param>
    /// <param name="percent">How much to affect it.</param>
    public void TemporaryAttributeChange(Attribute a, float percent)
    {
        int i = (int)a;
        if ((alteredAttributes | ((uint)1 << i)) == alteredAttributes) return; // if this value is already changed, don't do anything.
        attributeValues[a] = attributeValues[a] * percent; // change value

        alteredAttributes |= (uint)1 << i; // add attribute to bit mask
    }
    /// <summary>
    /// Restore an attribute's original value if it was temporarily changed.
    /// </summary>
    /// <param name="a"></param>
    public void RestoreAttributeChange(Attribute a)
    {
        PowerupManager p = gameObject.GetComponent<PowerupManager>(); // cheat a little
        RestoreAttributeChange(a, p.Buffs);
    }
    public void RestoreAttributeChange(Attribute a, List<BuffBase> b)
    {
        int i = (int)a;
        if ((alteredAttributes & (1 << i)) == 0) return; // if this value has not been changed, don't do anything.

        alteredAttributes &= ~((uint)1 << i); // clear bit corresponding to affected attribute
        RecompileBonus(a, b);
    }
}
