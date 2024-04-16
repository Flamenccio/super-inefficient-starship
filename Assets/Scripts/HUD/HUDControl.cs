using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Device;

public class HUDControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    // hud elements
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private TMP_Text timeDisplay;
    [SerializeField] private TMP_Text progressDisplay;
    [SerializeField] private GameObject levelupComponents;
    [SerializeField] private TMP_Text levelDisplay;
    [SerializeField] private TMP_Text hpDisplay;
    [SerializeField] private RawImage vignette;
    [SerializeField] private RawImage levelUpBackground;
    [SerializeField] private RawImage hurtLines;
    [SerializeField] private GameObject scoreFlyText;
    [SerializeField] private CrosshairAmmoController crosshairAmmo;
    [SerializeField] private TMP_Text killPointsDisplay;
    [SerializeField] private PlayerAttributes playerAtt;
    [SerializeField] private TMP_Text killPointBonusDisplay;

    private bool vignetteCrossfading = false;
    private int levelUpAnimating = 0;
    private Color vignetteColor = new(255f, 0f, 0f);
    private GameState gState;

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
        // update score and timers
        //progressDisplay.text = gState.TotalPoints + " / " + gState.DifficultyCurve(gState.Level + 1);
        progressDisplay.text = gState.Progress + " / " + gState.DifficultyCurve(gState.Level + 1);
        scoreDisplay.text = playerAtt.Ammo.ToString();
        crosshairAmmo.UpdateAmmo(playerAtt.Ammo);
        timeDisplay.text = CorrectTimerDisplay(gState.Timer);
        hpDisplay.text = playerAtt.HP.ToString();
        killPointsDisplay.text = $"+{playerAtt.KillPoints}";
        killPointBonusDisplay.text = $"x{playerAtt.KillPointBonus}";

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
        //Vector2 position = scoreDisplay.rectTransform.position + new Vector3(0f, 0f);
        string text = $"+{score}";
        DisplayFlyText(text, Color.yellow, scoreDisplay.transform.position);
    }
    public void DisplayHealthFlyText(int healthGained)
    {
        //Vector2 position = hpDisplay.rectTransform.position + new Vector3(0f, 0f);
        string text = $"+{healthGained}";
        DisplayFlyText(text, Color.green, hpDisplay.transform.position);
    }
    public void DisplayFlyText(string text, Color color, Vector2 position)
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
        
        //levelUpBackground.rectTransform.sizeDelta = new Vector2(0f, levelUpBackgroundSize.y);

        //levelUpAnimating = 1;
        
        levelDisplay.text = level.ToString();
        
        yield return new WaitForSeconds(1.5f);
        
        /*
        levelUpAnimating = 2;

        yield return new WaitForSeconds(1.0f);
        levelUpAnimating = 0;
        */
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
        string result = "";
        result = num.ToString("F" + 2);
        if (num < 10)
        {
            result = string.Concat("0", result);
        }
        return result;
    }
}
