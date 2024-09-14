using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Flamenccio.Core;
using UnityEngine.UI;
using Flamenccio.Powerup;
using System;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls HUD elements whose appearance or behavior changes based on events.
    /// </summary>
    public class DynamicHudControl : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelDisplay;
        [SerializeField] private GameObject levelUpUIComponents;
        [SerializeField] private Transform specialChargeContainer;
        [SerializeField] private PlayerAttributes playerAtt;
        [SerializeField] private Image specialChargePrefab;
        [SerializeField] private GameObject enemyRadarArrow;

        private List<SpecialChargeHUDControl> specialCharges = new();
        private const float SPECIAL_CHARGE_LOCAL_Y_OFFSET = -160f;
        private const float SPECIAL_CHARGE_DISTANCE = 18f;

        private void Awake()
        {
            GameEventManager.OnLevelUp += (x) => DisplayLevelUpText(Convert.ToInt32(x.Value));
            levelUpUIComponents.SetActive(false);
        }

        private void Update()
        {
            UpdateSpecialChargeHUD();
        }

        public void DisplayLevelUpText(int level)
        {
            StartCoroutine(LevelUpTextAnimation(level));
        }

        private IEnumerator LevelUpTextAnimation(int level)
        {
            levelUpUIComponents.SetActive(true);
            levelDisplay.text = level.ToString();
            yield return new WaitForSeconds(1.5f);
            levelUpUIComponents.SetActive(false);
        }

        public void DisplayBulletRadarArrow(Transform bullet, float minDistanceX, float minDistanceY, float maxDistance)
        {
            if (bullet == null) return;

            var instance = Instantiate(enemyRadarArrow, transform).GetComponent<EnemyRadarArrowControl>();
            instance.Target = bullet;
            instance.SetRange(minDistanceX, maxDistance, minDistanceY, maxDistance);
            instance.Ready = true;
        }

        #region special charges
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
                specialCharges.RemoveAt(specialCharges.Count - i);
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
        #endregion
    }
}
