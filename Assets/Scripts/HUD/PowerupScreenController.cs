using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls the display of the powerup menu.
    /// </summary>
    public class PowerupScreenController : MonoBehaviour
    {
        [SerializeField] private RectTransform powerupCardContainer;
        [SerializeField] private Image background;
        [SerializeField] private GameObject weaponCardPrefab; // prefab of a weapon card UI element
        [SerializeField] private GameObject buffCardPrefab; // prefab of a buff card UI element
        private PowerupCardController[] weaponCards = new PowerupCardController[WEAPON_CARD_AMOUNT];
        private GameObject[] buffCards = new GameObject[5];
        private const int WEAPON_CARD_AMOUNT = 3;
        private float CARD_HEIGHT_OFFSET = 0; // TODO make sure to update this if the screen size changes

        private void Start()
        {
            UIEventManager.DisplayWeapons += (x) =>
            {
                if (x.Value is not List<GameObject>)
                {
                    Debug.LogWarning("Given list is not a list of GameObjects");
                }

                DisplayWeaponCards(x.Value as List<GameObject>);
            };

            InstantiateCards();

            // subscribe to events
            UIEventManager.DisplayGameHUD += () => HideAllScreens();
        }

        private void InstantiateCards()
        {
            CARD_HEIGHT_OFFSET = Camera.main.scaledPixelHeight * 1f / 3f;
            // Instantiate new weapon cards to each position in weaponCards
            for (int i = 0; i < WEAPON_CARD_AMOUNT; i++)
            {
                Vector2 position = new(
                    powerupCardContainer.position.x,
                    powerupCardContainer.position.y + ((1 - i) * CARD_HEIGHT_OFFSET)
                    );
                weaponCards[i] = Instantiate(weaponCardPrefab, position, Quaternion.identity, powerupCardContainer).GetComponent<PowerupCardController>();
            }

            HideWeaponSelection();
        }

        public void HideAllScreens()
        {
            // Hide all selection screens.
            ClearAllWeaponCards(true);
        }

        private void HideWeaponSelection()
        {
            background.enabled = false;
            foreach (var item in weaponCards)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void DisplayWeaponSelection()
        {
            background.enabled = true;
            // Hide other screens.
            // Show the weapon selection screen.
            foreach (var item in weaponCards)
            {
                item.gameObject.SetActive(true);
            }
        }

        public void DisplayBuffSelection()
        {
            // Hide other screens.
            // Show the buff selection screen.
        }

        /// <summary>
        /// Add a weapon to display as a card. Replaces an existing card if the position is already taken.
        /// </summary>
        /// <param name="position">Position of card (range 0-2).</param>
        /// <param name="weapon">GameObject of weapon to display.</param>
        public void DisplayWeaponCard(int position, GameObject weapon)
        {
            if (position < 0 || position > weaponCards.Length) return;

            weaponCards[position].DisplayWeapon(weapon);
        }

        /// <summary>
        /// Display up to 3 weapons. Automatically enables weapon card displays.
        /// </summary>
        /// <param name="weapons">List of weapons to show</param>
        public void DisplayWeaponCards(List<GameObject> weapons)
        {
            DisplayWeaponSelection();

            if (weapons.Count > weaponCards.Length) Debug.LogWarning($"Given weapons exceeds maximum weapon cards. Will only show first {weaponCards.Length} weapons.");

            for (int i = 0; i < weaponCards.Length; i++)
            {
                weaponCards[i].DisplayWeapon(weapons[i]);
            }
        }

        /// <summary>
        /// Clear all weapon cards.
        /// </summary>
        /// <param name="hide">Should the cards be hidden too?</param>
        public void ClearAllWeaponCards(bool hide)
        {
            DisplayWeaponSelection(); // weapon cards must be active to work
            
            foreach (var card in weaponCards)
            {
                card.Clear();
            }

            if (hide) HideWeaponSelection();
        }
    }
}
