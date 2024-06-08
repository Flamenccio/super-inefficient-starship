using Flamenccio.Core;
using Flamenccio.Powerup;
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

        private void Update()
        {
            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            // update score and timers
            progressDisplay.text = $"{gState.Progress} / {gState.DifficultyCurve(gState.Level + 1)}";
            scoreDisplay.text = playerAtt.Ammo.ToString();
            crosshairAmmo.UpdateAmmo(playerAtt.Ammo);
            timeDisplay.text = CorrectTimerDisplay(gState.Timer);
            hpDisplay.text = playerAtt.HP.ToString();
            killPointsDisplay.text = $"+{playerAtt.KillPoints}";
            killPointBonusDisplay.text = $"x{playerAtt.KillPointBonus}";
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
