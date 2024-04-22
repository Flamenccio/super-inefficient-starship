using System;
using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Powerup.Weapon;

namespace Flamenccio.Powerup
{
    public interface IPowerup
    {
        string Name { get; }
        string Desc { get; }
        int Level { get; }
        PowerupRarity Rarity { get; }
        void LevelChange(int l);
        void Run();
    }
    public enum PowerupRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    };

    public class PowerupManager : MonoBehaviour
    {
        private WeaponMain mainAttack;
        private WeaponSub subAttack;
        private WeaponSpecial specialAttack;
        private Action powerupUpdate;
        private List<BuffBase> buffs = new();
        public List<BuffBase> Buffs { get => buffs; }
        public bool MainAttackAimAssisted
        {
            get
            {
                if (mainAttack == null) return false;
                return mainAttack.AimAssisted;
            }
        }
        public bool SubAttackAimAssisted
        {
            get
            {
                if (subAttack == null) return false;
                return subAttack.AimAssisted;
            }
        }
        public bool SpecialAttackAimAssisted
        {
            get
            {
                if (specialAttack == null) return false;
                return specialAttack.AimAssisted;
            }
        }

        // MAIN ATTACK METHODS
        public Action<float, float, Vector2> MainAttackTap { get; private set; }
        public Action<float, float, Vector2> MainAttackHold { get; private set; }
        public Action<float, float, Vector2> MainAttackHoldEnter { get; private set; }
        public Action<float, float, Vector2> MainAttackHoldExit { get; private set; }

        // SUB ATTACK METHODS
        public Action<float, float, Vector2> SubAttackTap { get; private set; }

        // SPECIAL ATTACK METHODS
        public Action<float, float, Vector2> SpecialAttackTap { get; private set; }

        // SUPPORT ATTACK METHODS


        [SerializeField][Tooltip("Path to default weapon.")] private UnityEditor.MonoScript defaultMain;
        [SerializeField][Tooltip("Path to default sub weapon.")] private UnityEditor.MonoScript defaultSub;
        [SerializeField] private UnityEditor.MonoScript debugSpecial; // gives special at start
        private PlayerAttributes playerAttributes;

        private void Awake()
        {
            AddWeapon(defaultSub);
            AddWeapon(defaultMain);

            if (debugSpecial != null)
            {
                AddWeapon(debugSpecial);
            }
        }
        private void Start()
        {
            playerAttributes = gameObject.GetComponent<PlayerAttributes>();
        }
        private WeaponMain AddMain(WeaponMain main) // replaces main weapon with given one. Returns previous main weapon.
        {
            WeaponMain temp = mainAttack;

            if (mainAttack != null)
            {
                powerupUpdate -= mainAttack.Run;
                Destroy(mainAttack); // replace scripts
            }

            mainAttack = main;
            powerupUpdate += mainAttack.Run; // update delegates and stuff
            MainAttackTap = mainAttack.Tap;
            MainAttackHold = mainAttack.Hold;
            MainAttackHoldEnter = mainAttack.HoldEnter;
            MainAttackHoldExit = mainAttack.HoldExit;
            return temp;
        }
        private WeaponSub AddSub(WeaponSub sub) // same thing as above
        {
            WeaponSub temp = subAttack;

            if (subAttack != null)
            {
                powerupUpdate -= subAttack.Run;
                Destroy(subAttack);
            }

            subAttack = sub;
            powerupUpdate += subAttack.Run;
            SubAttackTap = subAttack.Tap;
            return temp;
        }
        private WeaponSpecial AddSpecial(WeaponSpecial special)
        {
            WeaponSpecial temp = specialAttack;

            if (specialAttack != null)
            {
                powerupUpdate -= specialAttack.Run;
                Destroy(specialAttack);
            }

            specialAttack = special;
            powerupUpdate += specialAttack.Run;
            SpecialAttackTap = specialAttack.Tap;
            return temp;
        }
        public void AddWeapon(UnityEditor.MonoScript script)
        {
            if (script == null) return;

            Type weaponType = script.GetClass();
            Type weaponClass = weaponType.BaseType;
            object t = gameObject.AddComponent(weaponType).GetComponent(weaponType);

            if (weaponClass == typeof(WeaponSpecial))
            {
                AddSpecial(t as WeaponSpecial);
                return;
            }
            if (weaponClass == typeof(WeaponMain))
            {
                AddMain(t as WeaponMain);
                return;
            }
            if (weaponClass == typeof(WeaponSub))
            {
                AddSub(t as WeaponSub);
                return;
            }
        }
        private void Update()
        {
            powerupUpdate?.Invoke();
        }
        public void AddBuff(BuffBase b)
        {
            int x = FindBuff(b);
            if (x < 0)
            {
                buffs.Add(b); // if there is no existing duplciate, add it
            }
            else
            {
                buffs[x].LevelChange(1); // if there is an existing duplicate level up
            }
            foreach (PlayerAttributes.Attribute a in b.GetAffectedAttributes())
            {
                playerAttributes.RecompileBonus(a, buffs);
            }
        }
        public bool RemoveBuff(BuffBase b)
        {
            int x = FindBuff(b);
            if (x < 0)
            {
                return false;
            }
            buffs.RemoveAt(x);
            buffs.Sort((BuffBase a, BuffBase b) => a.Level < b.Level ? -1 : 1); // basically resort the list based on level: higher level buffs will be placed at the top.
            foreach (PlayerAttributes.Attribute a in b.GetAffectedAttributes())
            {
                playerAttributes.RecompileBonus(a, buffs);
            }
            return true;
        }
        /// <summary>
        /// Finds a buff from <b>buffs</b> and returns its index in the list. -1 if it doesn't exist.
        /// </summary>
        private int FindBuff(BuffBase b)
        {
            int i = 0;
            foreach (BuffBase bb in buffs)
            {
                if (bb.Name.Equals(b.Name))
                {
                    return i;
                }
                i++;
            }
            return -1; // if there is no buff that exists
        }
    }
}
