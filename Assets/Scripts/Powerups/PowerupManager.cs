using System;
using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Powerup.Weapon;
using Flamenccio.HUD;
using Flamenccio.Powerup.Buff;

namespace Flamenccio.Powerup
{
    public interface IPowerup
    {
        string Name { get; }
        string Desc { get; }
        int Level { get; }
        PowerupRarity Rarity { get; }
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
        // TODO this seems to be doing too much at once. Maybe split the class into a WeaponManager and BuffManager classes

        [SerializeField] private GameObject defaultMainWeapon;
        [SerializeField] private GameObject defaultDefenseWeapon;
        [SerializeField] private GameObject defaultSpecialWeapon;
        [SerializeField] private GameObject defaultSubWeapon;
        [SerializeField] private CrosshairsControl crosshairsControl;
        private WeaponMain mainAttack;
        private WeaponSub subAttack;
        private WeaponSpecial specialAttack;
        private WeaponDefense defenseAttack;
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
        public bool DefenseAttackAimAssisted
        {
            get
            {
                if (defenseAttack == null) return false;
                return defenseAttack.AimAssisted;
            }
        }

        // MAIN ATTACK METHODS
        private Action<float, float, Vector2> mainAttackTap;
        private Action<float, float, Vector2> mainAttackHold;
        private Action<float, float, Vector2> mainAttackHoldEnter;
        private Action<float, float, Vector2> mainAttackHoldExit;

        // SUB ATTACK METHODS
        private Action<float, float, Vector2> subAttackTap;

        // SPECIAL ATTACK METHODS
        private Action<float, float, Vector2> specialAttackTap;

        // SUPPORT ATTACK METHODS

        // DEFENSE ATTACK METHODS
        private Action<float, float, Vector2> defenseAttackTap;

        private PlayerAttributes playerAttributes;

        private void Start()
        {
            playerAttributes = gameObject.GetComponent<PlayerAttributes>();
            if (!AddWeapon(defaultMainWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultDefenseWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultSpecialWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultSubWeapon)) Debug.LogError("Conflicting weapon type!");
        }
        private bool AddMain(GameObject main) // replaces main weapon with given one. Returns previous main weapon.
        {
            if (mainAttack != null)
            {
                powerupUpdate -= mainAttack.Run;
                Destroy(mainAttack.gameObject);
            }

            if (!main.TryGetComponent<WeaponMain>(out mainAttack)) return false;

            powerupUpdate += mainAttack.Run; // update delegates and stuff
            mainAttackTap = mainAttack.Tap;
            mainAttackHold = mainAttack.Hold;
            mainAttackHoldEnter = mainAttack.HoldEnter;
            mainAttackHoldExit = mainAttack.HoldExit;
            
            if (crosshairsControl == null)
            {
                Debug.LogError("Crosshairs not found!");
            }
            else
            {
                crosshairsControl.UpdateWeaponRange(mainAttack.GetWeaponRange());
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

            powerupUpdate += subAttack.Run;
            subAttackTap = subAttack.Tap;

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

            powerupUpdate += specialAttack.Run;
            specialAttackTap = specialAttack.Tap;

            return true;
        }
        private bool AddDefense(GameObject defense)
        {
            if (defenseAttack != null)
            {
                powerupUpdate -= defenseAttack.Run;
                Destroy(defenseAttack);
            }

            if (!defense.TryGetComponent(out defenseAttack)) return false;

            powerupUpdate += defenseAttack.Run;
            defenseAttackTap = defenseAttack.Tap;

            return true;
        }

        public bool AddWeapon(GameObject weaponObjectPrefab)
        {
            if (weaponObjectPrefab == null) return false;

            if (!weaponObjectPrefab.TryGetComponent(out WeaponBase weaponBase))
            {
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
            else if (weaponBase is WeaponDefense)
            {
                return AddDefense(weaponObjectInstance);
            }

            return false;
        }
        private void Update()
        {
            powerupUpdate?.Invoke();
        }
        public void AddBuff(Type buffType)
        {
            if (!buffType.IsSubclassOf(typeof(BuffBase))) return;

            BuffBase buffInstance;

            if (buffType.IsSubclassOf(typeof(ConditionalBuff)))
            {
                Action<List<PlayerAttributes.Attribute>> x = LevelBuff;
                buffInstance = Activator.CreateInstance(buffType, new object[] { playerAttributes, x }) as BuffBase;
            }
            else
            {
                buffInstance = Activator.CreateInstance<BuffBase>();
            }

            AddBuff(buffInstance);
        }
        public void AddBuff(BuffBase b)
        {
            int x = FindBuff(b);

            if (x < 0)
            {
                buffs.Add(b); // if there is no existing duplciate, add it

                if (b is ConditionalBuff)
                {
                    powerupUpdate += (b as ConditionalBuff).Run;
                }
            }
            else
            {
                buffs[x].LevelUp(); // if there is an existing duplicate level up
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

            if (b is ConditionalBuff)
            {
                powerupUpdate -= (b as ConditionalBuff).Run;
            }

            buffs[x].OnDestroy();
            buffs.RemoveAt(x);
            buffs.Sort((BuffBase a, BuffBase b) => a.Level < b.Level ? -1 : 1); // basically re-sort the list based on level: higher level buffs will be placed at the top.

            foreach (PlayerAttributes.Attribute a in b.GetAffectedAttributes())
            {
                playerAttributes.RecompileBonus(a, buffs);
            }

            return true;
        }
        public bool RemoveBuff(Type buffType)
        {
            if (!buffType.IsSubclassOf(typeof(BuffBase))) return false;

            int x = FindBuff(buffType);

            if (x < 0) return false;

            BuffBase remove = buffs[x];

            if (remove is ConditionalBuff)
            {
                powerupUpdate -= (remove as ConditionalBuff).Run;
            }

            remove.OnDestroy();
            buffs.RemoveAt(x);
            buffs.Sort((BuffBase a, BuffBase b) => a.Level < b.Level ? -1 : 1); // basically re-sort the list based on level: higher level buffs will be placed at the top.

            foreach (PlayerAttributes.Attribute a in remove.GetAffectedAttributes())
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
            return FindBuff(b.Name);
        }
        private int FindBuff(string buffName)
        {
            int i = 0;
            foreach (BuffBase bb in buffs)
            {
                if (bb.Name.Equals(buffName))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        private int FindBuff(Type buffType)
        {
            int i = 0;
            foreach (BuffBase bb in buffs)
            {
                if (bb.GetType() == buffType)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        public void MainAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            mainAttackTap?.Invoke(aimAngle, moveAngle, origin);
        }
        public void MainAttackHold(float aimAngle, float moveAngle, Vector2 origin)
        {
            mainAttackHold?.Invoke(aimAngle, moveAngle, origin);
        }
        public void MainAttackHoldEnter(float aimAngle, float moveAngle, Vector2 origin)
        {
            mainAttackHoldEnter?.Invoke(aimAngle, moveAngle, origin);
        }
        public void MainAttackHoldExit(float aimAngle, float moveAngle, Vector2 origin)
        {
            mainAttackHoldExit?.Invoke(aimAngle, moveAngle, origin);
        }
        public void SubAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            subAttackTap?.Invoke(aimAngle, moveAngle, origin);
        }
        public void SpecialAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            specialAttackTap?.Invoke(aimAngle, moveAngle, origin);
        }
        public void DefenseAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            defenseAttackTap?.Invoke(aimAngle, moveAngle, origin);
        }
        private void LevelBuff(List<PlayerAttributes.Attribute> affectedAttributes)
        {
            foreach (var attribute in affectedAttributes)
            {
                playerAttributes.RecompileBonus(attribute, Buffs);
            }
        }
    }
}