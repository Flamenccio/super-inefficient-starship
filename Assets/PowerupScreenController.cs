using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls the display of the powerup menu.
    /// </summary>
    public class PowerupScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject weaponCardPrefab; // prefab of a weapon card UI element
        [SerializeField] private GameObject buffCardPrefab; // prefab of a buff card UI element
        private GameObject[] weaponCards = new GameObject[3];
        private GameObject[] buffCards = new GameObject[5];

        private void Start()
        {
            // Instantiate new weapon cards to each position in weaponCards
        }

        public void HideAllScreens()
        {
            // Hide all selection screens.
        }

        public void DisplayWeaponSelection()
        {
            // Hide other screens.
            // Show the weapon selection screen.
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
            // Check value of position
            // Check if weapon is not a weapon
            // Check for an ObjectDescription class on weapon GameObject
            // Grab name and description from ObjectDescription
            // Display strings on card
        }

        /// <summary>
        /// Clear all weapon cards.
        /// </summary>
        public void ClearAllWeaponCards()
        {
            // Remove all displayed strings on all weapon cards.
        }
    }
}
