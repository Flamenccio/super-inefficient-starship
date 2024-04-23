using System;
using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Powerup.Weapon;
using Flamenccio.HUD;

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
        [SerializeField] private GameObject defaultMainWeapon;
        [SerializeField] private GameObject defaultSubWeapon;
        [SerializeField] private GameObject defaultSpecialWeapon;
        private WeaponMain mainAttack;
        private WeaponSub subAttack;
        private WeaponSpecial specialAttack;
        private Action powerupUpdate;
        private List<GameObject> supportWeapons = new(Enum.GetNames(typeof(WeaponBase.WeaponType)).Length);
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

        private PlayerAttributes playerAttributes;

        private void Awake()
        {
        }
        private void Start()
        {
            playerAttributes = gameObject.GetComponent<PlayerAttributes>();
            if (!AddWeapon(defaultMainWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultSubWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultSpecialWeapon)) Debug.LogError("Conflicting weapon type!");
        }
        private bool AddMain(GameObject main) // replaces main weapon with given one. Returns previous main weapon.
        {
            if (mainAttack != null)
            {
                powerupUpdate -= mainAttack.Run;
                Destroy(mainAttack.gameObject);
            }

            if (!main.TryGetComponent<WeaponMain>(out mainAttack)) return false;

            CrosshairsControl crosshairs = GetComponentInChildren<CrosshairsControl>(); // HACK this is temporary; put this somewhere else!

            
            mainAttack = main.GetComponent<WeaponMain>();
            powerupUpdate += mainAttack.Run; // update delegates and stuff
            MainAttackTap = mainAttack.Tap;
            MainAttackHold = mainAttack.Hold;
            MainAttackHoldEnter = mainAttack.HoldEnter;
            MainAttackHoldExit = mainAttack.HoldExit;
            
            if (crosshairs == null)
            {
                Debug.LogError("Crosshairs not found!");
            }
            else
            {
                crosshairs.UpdateWeaponRange(mainAttack.GetWeaponRange());
            }

            return true;
        }
        private bool AddSub(GameObject sub) // same thing as above
        {
            if (subAttack != null)
            {
                powerupUpdate -= subAttack.Run;
                Destroy(subAttack.gameObject);
            }

            if (!sub.TryGetComponent<WeaponSub>(out subAttack)) return false;

            subAttack = sub.GetComponent<WeaponSub>();
            powerupUpdate += subAttack.Run;
            SubAttackTap = subAttack.Tap;

            return true;
        }
        private bool AddSpecial(GameObject special)
        {
            if (specialAttack != null)
            {
                powerupUpdate -= specialAttack.Run;
                Destroy(specialAttack);
            }

            if (!special.TryGetComponent(out specialAttack)) return false;

            specialAttack = special.GetComponent<WeaponSpecial>();
            powerupUpdate += specialAttack.Run;
            SpecialAttackTap = specialAttack.Tap;

            return true;
        }
        public bool AddWeapon(GameObject weaponObjectPrefab)
        {
            if (weaponObjectPrefab == null) return false;

            if (!weaponObjectPrefab.TryGetComponent(out WeaponBase weaponBase))
            {
                Debug.Log("asdfasd");
                return false;
            }

            GameObject weaponObjectInstance = Instantiate(weaponObjectPrefab, transform, false);

            if (weaponBase is WeaponMain)
            {
                return AddMain(weaponObjectInstance);
            }
            else if (weaponBase is WeaponSub)
            {
                return AddSub(weaponObjectInstance);
            }
            else if (weaponBase is WeaponSpecial)
            {
                return AddSpecial(weaponObjectInstance);
            }

            return false;
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
