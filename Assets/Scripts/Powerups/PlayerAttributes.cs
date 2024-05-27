using Flamenccio.Powerup.Buff;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Powerup
{
    /// <summary>
    /// Manages all of the players stats (HP, move speed, etc.)
    /// </summary>
    public class PlayerAttributes : MonoBehaviour
    {
        public int Ammo { get; private set; }
        public int MaxAmmo { get => (int)attributeValues[Attribute.MaxAmmo]; private set => attributeValues[Attribute.MaxAmmo] = value; }
        public int HP { get; private set; }
        public float MoveSpeed { get => (float)attributeValues[Attribute.MoveSpeed]; private set => attributeValues[Attribute.MoveSpeed] = value; }
        public int MaxHP { get => (int)attributeValues[Attribute.MaxHP]; private set => attributeValues[Attribute.MaxHP] = value; }
        public int KillPoints { get; private set; }
        public float KillPointBonus { get => (float)attributeValues[Attribute.KillPointBonus]; private set => attributeValues[Attribute.KillPointBonus] = value; }
        public float WeaponRange { get; }
        public int MainWeaponDamage { get; }
        public float MainWeaponDamageBonus { get => (float)attributeValues[Attribute.MainDamageBonus]; private set => attributeValues[Attribute.MainDamageBonus] = value; }
        public int SpecialCharges { get; private set; }
        public int MaxSpecialCharges { get; private set; }
        public float SpecialChargeTimer { get; private set; }
        public float SpecialChargeCooldown { get; private set; }

        public float SpecialChargeProgress
        {
            get
            {
                if (SpecialChargeCooldown <= 0f)
                {
                    return 0f;
                }
                return SpecialChargeTimer / SpecialChargeCooldown;
            }
        }

        public enum Attribute
        {
            MaxAmmo,
            MoveSpeed,
            MaxHP,
            KillPointBonus,
            MainDamageBonus,
        };

        public enum AmmoUsage
        {
            MainTap,
            MainHold,
            MainHoldExit,
            MainHoldEnter,
            SubTap
        };

        private float[] attributeBonuses; // array of total percent increase of each attribute
        private uint alteredAttributes; // a bitmask representing what attributes have been temporarily altered
        private int baseAmmo = 10;
        private int baseMaxAmmo = 50;
        private int baseMaxHP = 3;
        private float baseKillPointBonus = 1.25f;
        private float baseMoveSpeed = 4.8f;
        private float baseMainWeaponDamageBonus = 0f;
        private Dictionary<Attribute, double> attributeValues = new(); // current attribute values used for calculations and stuff
        private Dictionary<Attribute, double> baseAttributeValues = new();
        private Dictionary<AmmoUsage, AmmoCostModifier> ammoCostModifiers = new();
        private Dictionary<AmmoUsage, AmmoCostModifier> localAmmoCostModifiers = new();

        private void Start()
        {
        }

        private void Awake()
        {
            attributeBonuses = new float[Enum.GetNames(typeof(Attribute)).Length];

            // set default values
            MaxHP = baseMaxHP;
            HP = MaxHP;
            MoveSpeed = baseMoveSpeed;
            KillPointBonus = baseKillPointBonus;
            Ammo = baseAmmo;
            SpecialCharges = 0;
            MaxSpecialCharges = 0;
            SpecialChargeTimer = 0f;
            SpecialChargeCooldown = 0f;

            // populate base dictionary
            baseAttributeValues.Add(Attribute.MaxAmmo, baseMaxAmmo);
            baseAttributeValues.Add(Attribute.MaxHP, baseMaxHP);
            baseAttributeValues.Add(Attribute.MoveSpeed, baseMoveSpeed);
            baseAttributeValues.Add(Attribute.KillPointBonus, baseKillPointBonus);
            baseAttributeValues.Add(Attribute.MainDamageBonus, baseMainWeaponDamageBonus);

            for (int i = 0; i < Enum.GetValues(typeof(AmmoUsage)).Length; i++)
            {
                ammoCostModifiers.Add((AmmoUsage)i, new());
            }

            foreach (Attribute a in baseAttributeValues.Keys)
            {
                attributeValues[a] = baseAttributeValues[a];
            }
        }

        private void Update()
        {
            // automatically replenish charges
            if (SpecialCharges < MaxSpecialCharges)
            {
                SpecialChargeTimer += Time.deltaTime;
            }
            else
            {
                SpecialChargeTimer = 0f;
            }

            if (SpecialChargeTimer >= SpecialChargeCooldown)
            {
                SpecialChargeTimer = 0f;
                ReplenishCharge(1);
            }
        }

        /// <summary>
        /// Use ammo.
        /// </summary>
        /// <returns><b>True</b> if there is enough ammo, <b>false</b> if there is not enough ammo.</returns>
        public bool UseAmmo(int ammo)
        {
            if (Ammo < ammo)
            {
                return false;
            }

            Ammo -= ammo; // use ammo
            return true;
        }

        /// <summary>
        /// Use ammo, but also apply cost scaling depending on what's using it.
        /// </summary>
        /// <param name="usage">Weapon that's using the ammo.</param>
        /// <returns><b>True</b> if there is enough ammo, <b>false</b> if there is not enough ammo.</returns>
        public bool UseAmmo(int ammo, AmmoUsage usage)
        {
            int final;

            if (!localAmmoCostModifiers.TryGetValue(usage, out AmmoCostModifier modifier))
            {
                ammoCostModifiers.TryGetValue(usage, out modifier);
            }

            final = modifier.GetFinalCost(ammo);

            if (final > Ammo) return false;

            Ammo -= final;

            return true;
        }

        /// <summary>
        /// Add some ammo. Caps at maximum ammo.
        /// </summary>
        /// <param name="ammo">How much ammo to grant.</param>
        public void AddAmmo(int ammo)
        {
            Ammo += ammo;

            if (Ammo > MaxAmmo) Ammo = MaxAmmo;
        }

        /// <summary>
        /// Adds kill points.
        /// </summary>
        /// <param name="killPoints">How much kill points to grant.</param>
        public void AddKillPoints(int killPoints)
        {
            KillPoints += killPoints;
        }

        /// <summary>
        /// Clears all kill points.
        /// </summary>
        /// <returns>The number of total kill points cleared.</returns>
        public int UseKillPoints()
        {
            int x = KillPoints;
            KillPoints = 0;
            return x;
        }

        /// <summary>
        /// Changes life by given amount. Cannot go above maximum or below 0.
        /// </summary>
        /// <param name="life">How much life to give or take.</param>
        /// <returns>True if successful, false if HP is capped.</returns>
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

            if (a == Attribute.MaxHP) attributeValues[a] = y + bonus; // max hp is a simple integer value; calculate differently
            else attributeValues[a] = (1 + bonus) * y; // calculate final attribute value and apply it
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

            attributeValues[a] *= percent; // change value
            alteredAttributes |= (uint)1 << i; // add attribute to bit mask
        }

        /// <summary>
        /// Restore an attribute's original value if it was temporarily changed.
        /// </summary>
        /// <param name="a">Attribute to restore.</param>
        public void RestoreAttributeChange(Attribute a)
        {
            BuffManager p = gameObject.GetComponent<BuffManager>(); // cheat a little
            RestoreAttributeChange(a, p.Buffs);
        }

        /// <summary>
        /// Restore an attribute's original value if it was temporarily changed.
        /// </summary>
        /// <param name="a">Attribute to restore.</param>
        /// <param name="b">A buff list to recompile stat bonuses from.</param>
        public void RestoreAttributeChange(Attribute a, List<BuffBase> b)
        {
            int i = (int)a;

            if ((alteredAttributes & (1 << i)) == 0) return; // if this value has not been changed, don't do anything.

            alteredAttributes &= ~((uint)1 << i); // clear bit corresponding to affected attribute
            RecompileBonus(a, b);
        }

        /// <summary>
        /// Use some special charges. Given value should be positive.
        /// </summary>
        /// <param name="amount">Amount to use.</param>
        /// <returns>True if successful, false if there are not enough special charges.</returns>
        public bool UseCharge(int amount)
        {
            amount = Mathf.Abs(amount);

            if (SpecialCharges < amount) return false;

            SpecialCharges -= amount;
            return true;
        }

        /// <summary>
        /// Replenish some charges. Given value should be positive. Caps at maximum charges.
        /// </summary>
        /// <param name="amount">Amount to replenish.</param>
        /// <returns>True if successful, false if special charge cap is hit.</returns>
        public bool ReplenishCharge(int amount)
        {
            amount = Mathf.Abs(amount);

            if (SpecialCharges + amount > MaxSpecialCharges) return false;

            SpecialCharges += amount;
            return true;
        }

        /// <summary>
        /// Initialize special charges.
        /// </summary>
        /// <param name="max">Maximum amount of special charges.</param>
        /// <param name="cooldown">The time it should take for a special charge to repelnish naturally.</param>
        /// <returns>Always returns true currently</returns>
        public bool SetCharges(int max, float cooldown)
        {
            // TODO make false condition.
            MaxSpecialCharges = max;
            SpecialCharges = MaxSpecialCharges;
            SpecialChargeCooldown = cooldown;

            return true;
        }

        /// <summary>
        /// Changes maximum amount of special charges.
        /// </summary>
        /// <param name="amount">Amount to change by.</param>
        public void AddCharges(int amount)
        {
            // TODO make this safer by checking MaxSpecialCharges.

            //MaxSpecialCharges += amount;
        }

        /// <summary>
        /// Removes all special charges.
        /// </summary>
        public void RemoveCharges()
        {
            SetCharges(0, 0);
        }

        /// <summary>
        /// Get the ammo cost modifier of some attack.
        /// </summary>
        /// <param name="attack">Attack to get cost modifier from.</param>
        /// <returns>The AmmoCostModifier class of the attack.</returns>
        public AmmoCostModifier GetAmmoCostModifier(AmmoUsage attack)
        {
            // TODO do we really need this?
            return ammoCostModifiers[attack];
        }

        /// <summary>
        /// Creates a new local ammo modifier.
        /// </summary>
        /// <param name="attack">The attack the modifier will affect.</param>
        /// <param name="forceAdd">Should we replace a local modifier of the same attack, if it exists?</param>
        /// <returns><b>null</b> when unsuccessful; the new modifier when successful.</returns>
        public AmmoCostModifier AddLocalAmmoCostModifier(AmmoUsage attack, bool forceAdd)
        {
            if (localAmmoCostModifiers.ContainsKey(attack))
            {
                if (!forceAdd) return null;

                localAmmoCostModifiers.Remove(attack);
            }

            AmmoCostModifier newModifier = new();
            localAmmoCostModifiers.Add(attack, newModifier);

            return newModifier;
        }

        /// <summary>
        /// Attempts to remove an AmmoCostModifier from the list of AmmoCostModifiers.
        /// </summary>
        /// <param name="modifier">Modifier to remove.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveLocalAmmoCostModifier(AmmoCostModifier modifier)
        {
            if (localAmmoCostModifiers.ContainsValue(modifier))
            {
                var x = localAmmoCostModifiers
                    .First(x => x.Value == modifier)
                    .Key;

                localAmmoCostModifiers.Remove(x);

                return true;
            }

            return false;
        }
    }
}