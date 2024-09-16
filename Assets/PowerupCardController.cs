using Flamenccio.Core;
using Flamenccio.Effects;
using Flamenccio.Localization;
using Flamenccio.Powerup.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
        [SerializeField] private TMP_Text weaponType;

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

            // TODO temporary solution
            if (weaponBase is WeaponDefense)
            {
                weaponType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Misc", "misc.weapons.type.defense");
            }
            else if (weaponBase is WeaponMain)
            {
                weaponType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Misc", "misc.weapons.type.main");
            }
            else if (weaponBase is WeaponSub)
            {
                weaponType.text= LocalizationSettings.StringDatabase.GetLocalizedString("Misc", "misc.weapons.type.sub");
            }
            else if (weaponBase is WeaponSpecial)
            {
                weaponType.text= LocalizationSettings.StringDatabase.GetLocalizedString("Misc", "misc.weapons.type.special");
            }
            else
            {
                Debug.LogError("Given weapon is not a weapon");
            }
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
