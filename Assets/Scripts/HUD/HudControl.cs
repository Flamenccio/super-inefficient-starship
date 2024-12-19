using Flamenccio.Core;
using Flamenccio.Powerup;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls appearance and behavior of core HUD elements (HUD elements located at the top of the screen).
    /// </summary>
    public class HudControl : MonoBehaviour
    {
        // tmp text elements
        [SerializeField] private TMP_Text scoreDisplay;
        [SerializeField] private TMP_Text timeDisplay;
        [SerializeField] private TMP_Text progressDisplay;
        [SerializeField] private TMP_Text killPointsDisplay;
        [SerializeField] private TMP_Text killPointBonusDisplay;
        [SerializeField] private TMP_Text hpDisplay;

        // other classes
        [SerializeField] private GameState gState;
        [SerializeField] private PlayerAttributes playerAtt;
        [SerializeField] private CrosshairAmmoController crosshairAmmo;

        private bool hidden = false;

        private void Start()
        {
            UIEventManager.DisplayWeapons += (_) => SetDisplayVisibility(false);
            UIEventManager.DisplayGameHUD += () => SetDisplayVisibility(true);
        }

        private void Update()
        {
            UpdateDisplays();
        }

        private void SetDisplayVisibility(bool visible)
        {
            hidden = !visible;
            scoreDisplay.enabled = visible;
            timeDisplay.enabled = visible;
            killPointsDisplay.enabled = visible;
            progressDisplay.enabled = visible;
            killPointBonusDisplay.enabled = visible;
            hpDisplay.enabled = visible;
        }

        private void UpdateDisplays()
        {
            if (hidden) return;

            // update score and timers
            progressDisplay.text = $"{gState.Progress} / {gState.DifficultyCurve(gState.Level + 1)}";
            scoreDisplay.text = playerAtt.Ammo.ToString();
            crosshairAmmo.UpdateAmmo(playerAtt.Ammo);
            timeDisplay.text = CorrectTimerDisplay(gState.Timer);
            hpDisplay.text = playerAtt.HP.ToString();
            killPointsDisplay.text = $"+{playerAtt.KillPoints}";
            killPointBonusDisplay.text = $"×{playerAtt.KillPointBonus}";
        }

        private string CorrectTimerDisplay(float num)
        {
            string result = num.ToString("F" + 2);
            if (num < 10)
            {
                result = $"0{result}";
            }
            return result;
        }

    }
}
