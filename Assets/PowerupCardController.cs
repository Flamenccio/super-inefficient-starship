using Flamenccio.Core;
using Flamenccio.Effects;
using Flamenccio.Localization;
using Flamenccio.Powerup.Weapon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls a PowerupCard UI element
    /// </summary>
    public class PowerupCardController : MonoBehaviour
    {
        public GameObject AssociatedWeapon { get; private set; }

        [SerializeField] private TMP_Text weaponName;
        [SerializeField] private TMP_Text weaponDescription;

        /// <summary>
        /// Tries to display a weapon object. If the given game object is not a weapon, returns with an error message.
        /// </summary>
        /// <param name="weapon">Weapon object to display</param>
        public void DisplayWeapon(GameObject weapon)
        {
            if (!weapon.TryGetComponent<WeaponBase>(out var weaponBase))
            {
                Debug.LogError($"GameObject {weapon} is not a weapon object");
                return;
            }

            if (!weapon.TryGetComponent<ObjectDescription>(out var objectDescription))
            {
                Debug.LogError($"Weapon {weapon} does not implement an ObjectDescription component");
                return;
            }

            AssociatedWeapon = weapon;
            weaponName.text = objectDescription.GetObjectName().GetLocalizedString();
            weaponDescription.text = objectDescription.GetObjectDescription().GetLocalizedString();
        }

        public void Clear()
        {
            AssociatedWeapon = null;
            weaponName.text = string.Empty;
            weaponDescription.text = string.Empty;
        }

        public void OnSelected()
        {
            // equip weapon
            GameEventManager.EquipWeapon(GameEventManager.CreateGameEvent(AssociatedWeapon, PlayerMotion.Instance.PlayerPosition));
            UIEventManager.DisplayGameHUD();
        }
    }
}
