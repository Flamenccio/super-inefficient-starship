using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Device;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class HUDControl : MonoBehaviour
{
    // hud elements
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private TMP_Text timeDisplay;
    [SerializeField] private TMP_Text progressDisplay;
    [SerializeField] private TMP_Text killPointsDisplay;
    [SerializeField] private TMP_Text killPointBonusDisplay;
    [SerializeField] private TMP_Text levelDisplay;
    [SerializeField] private TMP_Text hpDisplay;
    [SerializeField] private RawImage vignette;
    [SerializeField] private RawImage levelUpBackground;
    [SerializeField] private RawImage hurtLines;
    [SerializeField] private UnityEngine.UI.Image specialChargePrefab;
    [SerializeField] private Sprite specialChargeUsed;
    [SerializeField] private Sprite specialChargeFilled;
    [SerializeField] private GameObject levelupComponents;
    [SerializeField] private Transform specialChargeContainer;
    [SerializeField] private GameObject scoreFlyText;
    [SerializeField] private CrosshairAmmoController crosshairAmmo;
    [SerializeField] private PlayerAttributes playerAtt;

    private bool vignetteCrossfading = false;
    private int levelUpAnimating = 0;
    private int currentCharge = 0;
    private GameState gState;
    private List<UnityEngine.UI.Image> specialCharges = new();
    private const float SPECIAL_CHARGE_LOCAL_Y_OFFSET = -140f;
    private const float SPECIAL_CHARGE_DISTANCE = 18f;

    private Vector2 levelUpBackgroundSize = Vector2.zero;

    private void Awake()
    {
        gState = GameState.instance;
        scoreDisplay.text = "0";
        levelupComponents.SetActive(false);
        vignette.color = new Color(255f, 0f, 0f, 0f);
        hurtLines.gameObject.SetActive(false);
        levelUpBackgroundSize = levelUpBackground.rectTransform.sizeDelta;
    }
    private void Start()
    {
    }

    private void Update()
    {
        if (gState == null)
        {
            gState = GameState.instance;
        }

        UpdateDisplays();
        UpdateVignette();
        UpdateLevelUpBanner();
        UpdateSpecialChargeHUD();
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
    private void UpdateVignette()
    {
        // fade vignette in as time starts to run out
        if (gState.Timer <= 5.0f && !vignetteCrossfading)
        {
            Color fixedColor = vignette.color;
            fixedColor.a = 1;
            vignette.color = fixedColor;
            vignette.CrossFadeAlpha(0f, 0f, true);
            vignette.CrossFadeAlpha(0.75f, 5.0f, false);
            vignetteCrossfading = true;
        }
        else if (gState.Timer > 5.0f)
        {
            vignetteCrossfading = false;
            vignette.CrossFadeAlpha(0f, 1.0f, false);
        }
    }
    private void UpdateLevelUpBanner()
    {
        // level up text animations
        if (levelUpAnimating == 1)
        {
            levelUpBackground.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(levelUpBackground.rectTransform.sizeDelta.x, levelUpBackgroundSize.x, Time.deltaTime * 3), levelUpBackgroundSize.y);
        }
        else if (levelUpAnimating == 2)
        {
            levelUpBackground.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(levelUpBackground.rectTransform.sizeDelta.x, 0.0f, Time.deltaTime * 6), levelUpBackgroundSize.y);
        }
    }
    private void UpdateSpecialChargeHUD()
    {
        currentCharge = playerAtt.SpecialCharges;

        if (currentCharge == 0)
        {
            specialCharges[0].sprite = specialChargeUsed;
            return;
        }

        if (currentCharge == playerAtt.MaxSpecialCharges)
        {
            specialCharges[^1].sprite = specialChargeFilled;
            return;
        }

        specialCharges[currentCharge].sprite = specialChargeUsed;
        specialCharges[currentCharge - 1].sprite = specialChargeFilled;
    }
    public void DisplayLevelUpText(int level)
    {
        StartCoroutine(LevelUpTextAnimation(level));
    }
    public void DisplayHurtLines()
    {
        StartCoroutine(HurtLinesAnimation());
    }
    public void DisplayScoreFlyText(int score) 
    {
        string text = $"+{score}";
        DisplayFlyText(text, Color.yellow, scoreDisplay.transform.position);
    }
    public void DisplayHealthFlyText(int healthGained)
    {
        string text = $"+{healthGained}";
        DisplayFlyText(text, Color.green, hpDisplay.transform.position);
    }
    public void AddSpecialCharges(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var charge = Instantiate(specialChargePrefab, specialChargeContainer, false);

            if (specialCharges.Count > 0)
            {
                charge.rectTransform.localPosition = new Vector2(specialCharges[^1].transform.localPosition.x + SPECIAL_CHARGE_DISTANCE, SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                specialCharges.Add(charge);
                foreach(var img in specialCharges)
                {
                    img.transform.localPosition = new Vector2(img.transform.localPosition.x - (SPECIAL_CHARGE_DISTANCE / 2f), SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                }
            
            }
            else
            {
                charge.transform.localPosition = new Vector2(0f, SPECIAL_CHARGE_LOCAL_Y_OFFSET);
                specialCharges.Add(charge);
            }
        }
    }
    public void ClearSpecialCharges()
    {
        foreach (var img in specialCharges)
        {
            Destroy(img);
        }

        specialCharges.Clear();
    }
    private void DisplayFlyText(string text, Color color, Vector2 position)
    {
        GameObject instance = Instantiate(scoreFlyText, position, Quaternion.identity, transform);
        instance.transform.position = position;
        TMP_Text instanceTMP = instance.GetComponent<TMP_Text>();
        instanceTMP.text = text;
        instanceTMP.color = color;
    }
    private IEnumerator LevelUpTextAnimation(int level)
    {
        levelupComponents.SetActive(true);
        levelDisplay.text = level.ToString();
        yield return new WaitForSeconds(1.5f);
        levelupComponents.SetActive(false);
    }
    private IEnumerator HurtLinesAnimation()
    {
        hurtLines.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        hurtLines.gameObject.SetActive(false);
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
