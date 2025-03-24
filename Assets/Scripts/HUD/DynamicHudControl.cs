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
        [SerializeField] private PlayerAttributes playerAtt;
        [SerializeField] private GameObject enemyRadarArrow;

        private void Awake()
        {
            GameEventManager.OnLevelUp += (x) => DisplayLevelUpText(Convert.ToInt32(x.Value));
            levelUpUIComponents.SetActive(false);
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

        public void DisplayBulletRadarArrow(Transform bullet, float maxDistance)
        {
            Debug.Log("Bullet arrow obsolete");
            /*
            if (bullet == null) return;

            var instance = Instantiate(enemyRadarArrow, transform)
                .GetComponent<EnemyRadarArrowControl>();
            instance.Target = bullet;
            
            // DEBUG VALUE
            instance.MaxDistanceFromEllipse = 1000f;
            instance.Ready = true;
            */
        }
    }
}
