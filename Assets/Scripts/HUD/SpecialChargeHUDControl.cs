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
        [SerializeField] private GameObject replenishFlashPrefab;
        private Image spriteRenderer;
        private bool charged = true;

        private void Awake()
        {
            spriteRenderer = GetComponent<Image>();
            spriteRenderer.sprite = chargedSprite;
        }

        public void SetSpriteUsed()
        {
            if (!charged) return;

            spriteRenderer.sprite = usedSprite;
            charged = false;
        }

        public void SetSpriteCharged()
        {
            if (charged) return;

            var t = Instantiate(replenishFlashPrefab, transform.position, Quaternion.identity, transform).GetComponent<Image>();
            t.transform.localScale *= new Vector2(2f, 4f);
            spriteRenderer.sprite = chargedSprite;
            charged = true;
        }
    }
}
