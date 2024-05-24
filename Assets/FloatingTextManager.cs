using TMPro;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// <para>Allows other classes to spawn <b>floating text displays</b>: text elements that can be instanced anywhere on screen.</para>
    /// <para>Floating text elements have customizable content, size, duration, animation, and color.</para>
    /// </summary>
    public class FloatingTextManager : MonoBehaviour
    {
        public static FloatingTextManager Instance { get; set; }

        [SerializeField] private TMP_Text floatTextPrefab;
        [SerializeField] private Transform hpLocation;
        [SerializeField] private Transform ammoLocation;
        [SerializeField] private Transform canvasTransform;
        private readonly Color defaultColor = Color.white;
        private readonly float defaultSize = 20.0f;
        private readonly float defaultDuration = 1.0f;
        private readonly Vector2 defaultOffset = new(40f, 0f); // offset used when placing floating text on top of hp or ammo gui element

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            
        }

        /// <summary>
        /// Spawn floating text with specified paramters.
        /// </summary>
        /// <param name="content">What the text will say.</param>
        /// <param name="worldPosition">Where in the game world the text will spawn.</param>
        public void DisplayText(string content, Vector2 worldPosition)
        {
            DisplayText(content, worldPosition, defaultColor, defaultDuration, defaultSize, FloatingTextControl.TextAnimation.None, FloatingTextControl.TextAnimation.None);
        }

        /// <summary>
        /// Spawn floating text with specified paramters.
        /// </summary>
        /// <param name="content">What the text will say.</param>
        /// <param name="worldPosition">Where in the game world the text will spawn.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="duration">How long the text will last before disappearing.</param>
        /// <param name="size">The <b>font size</b> of the text.</param>
        public void DisplayText(string content, Vector2 worldPosition, Color color, float duration, float size, FloatingTextControl.TextAnimation enterAnimation, FloatingTextControl.TextAnimation exitAnimation)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            //var instance = Instantiate(floatTextPrefab, screenPosition, Quaternion.identity, canvasTransform).GetComponent<FloatingTextControl>();
            var instance = Instantiate(floatTextPrefab, canvasTransform, true).GetComponent<FloatingTextControl>();
            instance.transform.position = screenPosition;
            instance.TargetColor = color;
            instance.Duration = duration;
            instance.TargetFontSize = size;
            instance.EnterAnimation = enterAnimation;
            instance.ExitAnimation = exitAnimation;
            instance.TextContent = content;
            instance.PlayAnimation();
        }

        /// <summary>
        /// A preset for DisplayText(), specifically for displaying gained health.
        /// </summary>
        public void DisplayHealthText(int healthGained)
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint((Vector2)hpLocation.position - defaultOffset);
            DisplayText($"+{healthGained}", worldPosition, Color.green, defaultDuration, defaultSize, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.Fade);
        }

        /// <summary>
        /// A preset for DisplayText(), specifically for displaying gained ammo.
        /// </summary>
        public void DisplayAmmoText(int ammoGained)
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint((Vector2)ammoLocation.position + defaultOffset);
            DisplayText($"+{ammoGained}", worldPosition, Color.yellow, defaultDuration, defaultSize, FloatingTextControl.TextAnimation.ZoomOut, FloatingTextControl.TextAnimation.Fade);
        }
    }
}
