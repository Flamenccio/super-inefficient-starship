using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Control the behavior of a special charge HUD element.
    /// </summary>
    public class SpecialChargeHUDControl : MonoBehaviour
    {
        [SerializeField] private Sprite chargedSprite;
        [SerializeField] private Sprite usedSprite;
        private Image spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<Image>();
            spriteRenderer.sprite = chargedSprite;
        }

        public void SetSpriteUsed()
        {
            spriteRenderer.sprite = usedSprite;
        }

        public void SetSpriteCharged()
        {
            spriteRenderer.sprite = chargedSprite;
        }
    }
}
