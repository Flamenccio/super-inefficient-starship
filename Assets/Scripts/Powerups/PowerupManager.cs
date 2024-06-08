using System;
using UnityEngine;
using Flamenccio.Powerup.Weapon;
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

    /// <summary>
    /// Interface to both WeaponManager and BuffManager.
    /// </summary>
    public class PowerupManager : MonoBehaviour
    {
        private WeaponManager weaponManager;
        private BuffManager buffManager;
        public bool MainAttackAimAssisted { get => weaponManager.MainAttackAimAssisted; }
        public bool SubAttackAimAssisted { get => weaponManager.SubAttackAimAssisted; }
        public bool SpecialAttackAimAssisted { get => weaponManager.SpecialAttackAimAssisted; }
        public bool DefenseAttackAimAssisted { get => weaponManager.DefenseAttackAimAssisted; }

        private void Awake()
        {
            weaponManager = GetComponent<WeaponManager>();
            buffManager = GetComponent<BuffManager>();
        }

        /// <summary>
        /// Attempts to add a weapon from the given game object.
        /// </summary>
        /// <param name="weaponObjectPrefab">The weapon to grant.</param>
        /// <returns><b>True</b> if successful, <b>false</b> if unsuccessful.</returns>
        public bool AddWeapon(GameObject weaponObjectPrefab)
        {
            return weaponManager.AddWeapon(weaponObjectPrefab);
        }

        /// <summary>
        /// Attempts to add a buff of the given type. If the player already has the given buff, it upgrades the buff instead.
        /// </summary>
        /// <param name="buffType">The buff to grant.</param>
        public void AddBuff(Type buffType)
        {
            buffManager.AddBuff(buffType);
        }

        /// <summary>
        /// Attemps to remove a buff of the given type.
        /// </summary>
        /// <param name="buffType">The buff to remove.</param>
        /// <returns><b>True</b> if successful, <b>falase</b> if unsuccessful.</returns>
        public bool RemoveBuff(Type buffType)
        {
            return buffManager.RemoveBuff(buffType);
        }

        #region public weapon attack methods

        public void MainAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.MainAttackTap(aimAngle, moveAngle, origin);
        }

        public void MainAttackHold(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.MainAttackHold(aimAngle, moveAngle, origin);
        }

        public void MainAttackHoldEnter(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.MainAttackHoldEnter(aimAngle, moveAngle, origin);
        }

        public void MainAttackHoldExit(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.MainAttackHoldExit(aimAngle, moveAngle, origin);
        }

        public void SubAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.SubAttackTap(aimAngle, moveAngle, origin);
        }

        public void SpecialAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.SpecialAttackTap(aimAngle, moveAngle, origin);
        }

        public void DefenseAttackTap(float aimAngle, float moveAngle, Vector2 origin)
        {
            weaponManager.DefenseAttackTap(aimAngle, moveAngle, origin);
        }

        #endregion public weapon attack methods
    }
}