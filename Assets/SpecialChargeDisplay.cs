using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flamenccio.HUD
{
    public class SpecialChargeDisplay : MonoBehaviour
    {
        [SerializeField] private Image specialChargePrefab;
        [SerializeField] private Transform specialChargeContainer;
        private List<SpecialChargeHUDControl> specialCharges = new();
        private Stack<SpecialChargeHUDControl> readySpecialCharges = 
            new();
        private Stack<SpecialChargeHUDControl> usedSpecialCharges =
            new();
        private const float SPECIAL_CHARGE_LOCAL_Y_OFFSET = -160f;
        private const float SPECIAL_CHARGE_DISTANCE = 18f;

        /// <summary>
        /// Destroy existing special charges and clear list
        /// </summary>
        public void ResetSpecialCharges()
        {
            specialCharges.ForEach(s => Destroy(s));
            specialCharges.Clear();
            readySpecialCharges.Clear();
            usedSpecialCharges.Clear();
        }

        /// <summary>
        /// Set number of special charges to display
        /// </summary>
        /// <param name="charges">Number of max charges to display</param>
        public void SetSpecialCharges(int charges)
        {
            if (charges <= 0)
            {
                Debug.LogWarning($"({name}) invalid amount of charges " +
                                 $"to add.");
                return;
            }
            
            if (specialCharges.Count > 0)
            {
                Debug.LogWarning($"({name}) cannot set special charges; " +
                                 $"there are existing special charges.");
                return;
            }

            for (var i = 0; i < charges; i++)
            {
                AddSpecialCharge();
            }
        }

        /// <summary>
        /// Adds 1 special charge to the gauge
        /// </summary>
        private void AddSpecialCharge()
        {
            var charge = Instantiate(specialChargePrefab,
                specialChargeContainer,
                false);
            var control = charge.gameObject
                .GetComponent<SpecialChargeHUDControl>();

            if (specialCharges.Count > 0)
            {
                var xPos =
                    specialCharges[^1].transform.localPosition.x +
                    SPECIAL_CHARGE_DISTANCE;
                charge.rectTransform.localPosition = new Vector2(
                    xPos,
                    SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                specialCharges.Add(control);

                var xOffset = (SPECIAL_CHARGE_DISTANCE / 2f);
                specialCharges.ForEach(
                    img => img.transform.localPosition =
                        new Vector2(
                            img.transform.localPosition.x - xOffset,
                            SPECIAL_CHARGE_LOCAL_Y_OFFSET));
            }
            else
            {
                charge.transform.localPosition = new Vector2(
                    0f, SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                specialCharges.Add(control);
            }
        }

        public void UseSpecialCharge(int charges = 1)
        {
            if (specialCharges.Count == 0 || 
                charges > specialCharges.Count)
            {
                Debug.LogError($"({name}) cannot use a special charge; " +
                        "not enough special charges available.");
                return;
            }

            if (readySpecialCharges.Count == 0 || 
                charges > readySpecialCharges.Count)
            {
                Debug.LogError($"({name}) cannot use a special charge; " +
                          "not enough ready special charges available.");
                return;
            }

            for (var i = 0; i < charges; i++)
            {
                var charge = readySpecialCharges.Pop();
                charge.SetSpriteUsed();
                usedSpecialCharges.Push(charge);
            }
        }

        public void RestoreSpecialCharge(int charges = 1)
        {
            if (specialCharges.Count == 0 || 
                charges > specialCharges.Count)
            {
                Debug.LogError($"({name}) cannot restore a special charge; " +
                        "not enough special charges available.");
                return;
            }

            if (usedSpecialCharges.Count == 0 || 
                charges > usedSpecialCharges.Count)
            {
                Debug.LogError($"({name}) cannot restore a special charge; " +
                          "not enough used special charges available.");
                return;
            }

            for (var i = 0; i < charges; i++)
            {
                var charge = usedSpecialCharges.Pop();
                charge.SetSpriteCharged();
                readySpecialCharges.Push(charge);
            }
        }
        private void Update()
        {
            //UpdateSpecialChargeHUD();
        }
        
        /*
        private void UpdateSpecialChargeHUD()
        {
            if (playerAtt.MaxSpecialCharges == 0) return;

            int currentCharge = playerAtt.SpecialCharges;

            if (specialCharges.Count != playerAtt.MaxSpecialCharges) // update max special charge count if necessary
            {
                int difference = playerAtt.MaxSpecialCharges - specialCharges.Count;

                if (difference == -specialCharges.Count)
                {
                    ClearSpecialCharges();
                    return; // if there are no charges, there are no HUD elements to update: return early
                }
                else if (difference > 0)
                {
                    AddSpecialCharges(difference);
                }
                else if (difference < 0)
                {
                    RemoveSpecialCharges(difference);
                }
            }

            // control appearance of charges
            if (currentCharge == 0)
            {
                specialCharges[0].SetSpriteUsed();
                return;
            }

            if (currentCharge == playerAtt.MaxSpecialCharges)
            {
                specialCharges[^1].SetSpriteCharged();
                return;
            }

            specialCharges[currentCharge].SetSpriteUsed();
            specialCharges[currentCharge - 1].SetSpriteCharged();
        }

        private void AddSpecialCharges(int amount)
        {
            amount = Mathf.Abs(amount);

            if (amount == 0) return;

            for (int i = 0; i < amount; i++)
            {
                var charge = Instantiate(specialChargePrefab, specialChargeContainer, false);
                var control = charge.gameObject.GetComponent<SpecialChargeHUDControl>();

                if (specialCharges.Count > 0)
                {
                    charge.rectTransform.localPosition = new Vector2(specialCharges[^1].transform.localPosition.x + SPECIAL_CHARGE_DISTANCE, SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                    specialCharges.Add(control);
                }
                else
                {
                    charge.transform.localPosition = new Vector2(0f, SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                    specialCharges.Add(control);
                    return;
                }

            }

            float xOffset = amount * (SPECIAL_CHARGE_DISTANCE / 2f);
            specialCharges.ForEach(img => img.transform.localPosition = new Vector2(img.transform.localPosition.x - xOffset, SPECIAL_CHARGE_LOCAL_Y_OFFSET));
        }

        private void RemoveSpecialCharges(int amount)
        {
            amount = Mathf.Abs(amount); // amount must be nonnegative for this to work

            if (amount == 0) return; // don't do anything if amount is zero

            if (specialCharges.Count < amount) return; // don't do anything if the removed amount exceeds what the list has

            for (int i = 0; i < amount; i++)
            {
                Destroy(specialCharges[^(i + 1)]); // destroy last charge in list
                specialCharges.RemoveAt(Mathf.Clamp(specialCharges.Count - i, 0, 100));
            }

            float xOffset = amount * (SPECIAL_CHARGE_DISTANCE / 2f);
            specialCharges.ForEach(img => img.transform.localPosition = new Vector2(img.transform.localPosition.x + xOffset, SPECIAL_CHARGE_LOCAL_Y_OFFSET));
        }

        private void ClearSpecialCharges()
        {
            foreach (var img in specialCharges)
            {
                Destroy(img);
            }

            specialCharges.Clear();
        }
        */
    }
}
