using UnityEngine;
using Flamenccio.HUD;
using System.Collections.Generic;
using System;
using Flamenccio.Core;

namespace Flamenccio.Powerup.Weapon
{
    /// <summary>
    /// Manages the players current weapons.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private GameObject defaultMainWeapon;
        [SerializeField] private GameObject defaultDefenseWeapon;
        [SerializeField] private GameObject defaultSpecialWeapon;
        [SerializeField] private GameObject defaultSubWeapon;
        [SerializeField] private CrosshairsControl crosshairsControl;

        private WeaponMain mainAttack;
        private WeaponSub subAttack;
        private WeaponSpecial specialAttack;
        private WeaponDefense defenseAttack;
        private Action weaponUpdate;
        private List<GameObject> supportWeapons = new(Enum.GetNames(typeof(WeaponBase.WeaponType)).Length);

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

        private void Start()
        {
            if (!AddWeapon(defaultMainWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultDefenseWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultSpecialWeapon)) Debug.LogError("Conflicting weapon type!");
            if (!AddWeapon(defaultSubWeapon)) Debug.LogError("Conflicting weapon type!");

            GameEventManager.EquipWeapon += (x) => AddWeapon(x.Value as GameObject);
        }

        private void Update()
        {
            weaponUpdate?.Invoke();
        }

        #region attack delegates

        // MAIN ATTACK METHODS
        private Action<float, float, Vector2> mainAttackTap;
        private Action<float, float, Vector2> mainAttackHold;
        private Action<float, float, Vector2> mainAttackHoldEnter;
        private Action<float, float, Vector2> mainAttackHoldExit;

        // SUB ATTACK METHODS
        private Action<float, float, Vector2> subAttackTap;
        private Action<float, float, Vector2> subAttackHold;
        private Action<float, float, Vector2> subAttackHoldEnter;
        private Action<float, float, Vector2> subAttackHoldExit;

        // SPECIAL ATTACK METHODS
        private Action<float, float, Vector2> specialAttackTap;

        // SUPPORT ATTACK METHODS

        // DEFENSE ATTACK METHODS
        private Action<float, float, Vector2> defenseAttackTap;

        #endregion attack delegates

        #region weapon adding

        private bool AddMain(GameObject main) // replaces main weapon with given one. Returns previous main weapon.
        {
            if (mainAttack != null)
            {
                weaponUpdate -= mainAttack.Run;
                Destroy(mainAttack.gameObject);
            }

            if (!main.TryGetComponent<WeaponMain>(out mainAttack)) return false;

            weaponUpdate += mainAttack.Run; // update delegates and stuff
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
                weaponUpdate -= subAttack.Run;
                Destroy(subAttack.gameObject);
            }

            if (!sub.TryGetComponent<WeaponSub>(out subAttack)) return false;

            weaponUpdate += subAttack.Run;
            subAttackTap = subAttack.Tap;
            subAttackHold = subAttack.Hold;
            subAttackHoldEnter = subAttack.HoldEnter;
            subAttackHoldExit = subAttack.HoldExit;

            return true;
        }

        private bool AddSpecial(GameObject special)
        {
            if (specialAttack != null)
            {
                weaponUpdate -= specialAttack.Run;
                Destroy(specialAttack);
            }

            if (!special.TryGetComponent(out specialAttack)) return false;

            weaponUpdate += specialAttack.Run;
            specialAttackTap = specialAttack.Tap;

            return true;
        }

        private bool AddDefense(GameObject defense)
        {
            if (defenseAttack != null)
            {
                weaponUpdate -= defenseAttack.Run;
                Destroy(defenseAttack);
            }

            if (!defense.TryGetComponent(out defenseAttack)) return false;

            weaponUpdate += defenseAttack.Run;
            defenseAttackTap = defenseAttack.Tap;

            return true;
        }

        /// <summary>
        /// Tries to add a weapon to the player with the weapon's game object.
        /// </summary>
        /// <param name="weaponObjectPrefab">The game object of the weapon.</param>
        /// <returns>True if successful, false otherwise.</returns>
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

        #endregion weapon adding

        #region public weapon attacks

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

        public void SubAttackHold(float aimAngle, float moveAngle, Vector2 origin)
        {
            subAttackHold?.Invoke(aimAngle, moveAngle, origin);
        }

        public void SubAttackHoldEnter(float aimAngle, float moveAngle, Vector2 origin)
        {
            subAttackHoldEnter?.Invoke(aimAngle, moveAngle, origin);
        }

        public void SubAttackHoldExit(float aimAngle, float moveAngle, Vector2 origin)
        {
            subAttackHoldExit?.Invoke(aimAngle, moveAngle, origin);
        }

        public void SpecialAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            specialAttackTap?.Invoke(aimAngle, moveAngle, origin);
        }

        public void DefenseAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            defenseAttackTap?.Invoke(aimAngle, moveAngle, origin);
        }

        #endregion public weapon attacks
    }
}