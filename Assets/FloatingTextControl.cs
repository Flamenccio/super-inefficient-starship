using Flamenccio.Effects;
using System;
using TMPro;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls behavior of floating text UI objects.
    /// </summary>
    public class FloatingTextControl : MonoBehaviour
    {
        public enum TextAnimation
        {
            None,
            Fade,
            ZoomOut,
            ZoomIn,
        }
        public string TextContent
        {
            get;
            set;
        }
        public float TargetFontSize
        {
            get => targetFontSize;
            set
            {
                if (!playingAnimation)
                {
                    targetFontSize = Mathf.Clamp(value, MIN_FONT_SIZE, MAX_FONT_SIZE);
                }
            }
        }
        public Color TargetColor
        {
            get => targetColor;
            set
            {
                if (!playingAnimation)
                {
                    targetColor = value;
                }
            }
        }
        public float Duration
        {
            get => duration;
            set
            {
                if (!playingAnimation)
                {
                    duration = Mathf.Clamp(value, MIN_DURATION_SECONDS, Mathf.Infinity);
                }
            }
        }
        public TextAnimation EnterAnimation
        {
            get => enterAnimation;
            set
            {
                if (!playingAnimation)
                {
                    enterAnimation = value;
                }
            }
        }
        public TextAnimation ExitAnimation
        {
            get => exitAnimation;
            set
            {
                if (!playingAnimation)
                {
                    exitAnimation = value;
                }

            }
        }
        public bool SpawnInGameWorld 
        {
            get => spawnInGameWorld;
            set
            {
                if (!playingAnimation)
                {
                    spawnInGameWorld = value;
                }
            }
        }
        private enum AnimationState
        {
            Enter,
            Intermediate,
            Exit,
        };
        private TMP_Text textObject;
        private bool playingAnimation = false;
        private float lifeTimer = 0f;
        private TextAnimation enterAnimation;
        private TextAnimation exitAnimation;
        private AnimationState state = AnimationState.Enter;
        private float duration;
        private Color targetColor;
        private float targetFontSize;
        private Vector2 worldOrigin; // origin of text in game world
        private const float ENTER_DURATION = 0.1f;
        private const float EXIT_DURATION = 0.1f;
        private const float MIN_DURATION_SECONDS = 0.5f;
        private const float MAX_FONT_SIZE = 100f;
        private const float MIN_FONT_SIZE = 1f;

        private Action EnterAnimationAction;
        private Action ExitAnimationAction;
        private bool spawnInGameWorld;

        private void Awake()
        {
            // default animations
            EnterAnimation = TextAnimation.None;
            ExitAnimation = TextAnimation.None;
            textObject = GetComponent<TMP_Text>();
            Duration = 1.0f;
            textObject.rectTransform.localScale = Vector3.one;
            SpawnInGameWorld = false;
        }

        private void Start()
        {
            var pos = Camera.main.ScreenToWorldPoint(transform.position);
            worldOrigin = new(pos.x, pos.y);
        }

        private void Update()
        {
            textObject.text = TextContent;
            textObject.color = new(TargetColor.r, TargetColor.g, TargetColor.b, textObject.color.a);

            if (SpawnInGameWorld) transform.position = Camera.main.WorldToScreenPoint(worldOrigin);

            if (!playingAnimation) return;

            lifeTimer += Time.deltaTime;

            if (lifeTimer <= ENTER_DURATION)
            {
                state = AnimationState.Enter;
            }
            else if (lifeTimer >= Duration - EXIT_DURATION)
            {
                state = AnimationState.Exit;
            }
            else
            {
                state = AnimationState.Intermediate;
            }

            if (lifeTimer >= Duration)
            {
                Destroy(gameObject);
            }

            switch (state)
            {
                case AnimationState.Enter:
                    EnterAnimationAction?.Invoke();
                    break;
                case AnimationState.Intermediate:
                    textObject.fontSize = TargetFontSize;
                    break;
                case AnimationState.Exit:
                    ExitAnimationAction?.Invoke();
                    break;
            }
        }

        public void PlayAnimation()
        {
            if (playingAnimation) return;

            // prepare the text for entrance animation, if necessary
            switch (EnterAnimation)
            {
                case TextAnimation.Fade:
                    textObject.alpha = 0.0f;
                    break;
                case TextAnimation.ZoomOut:
                    textObject.fontSize = MAX_FONT_SIZE;
                    break;
                case TextAnimation.ZoomIn:
                    textObject.fontSize = MIN_FONT_SIZE;
                    break;
            }

            // set entrance animation
            switch (EnterAnimation)
            {
                case TextAnimation.None:
                    break;
                case TextAnimation.Fade:
                    EnterAnimationAction = FadeIn;
                    break;
                case TextAnimation.ZoomOut:
                    EnterAnimationAction = ZoomOutEnter;
                    break;
                case TextAnimation.ZoomIn:
                    EnterAnimationAction = ZoomInEnter;
                    break;
            }

            // set exit animation
            switch (ExitAnimation)
            {
                case TextAnimation.None:
                    break;
                case TextAnimation.Fade:
                    ExitAnimationAction = FadeOut;
                    break;
                case TextAnimation.ZoomOut:
                    ExitAnimationAction = ZoomOutExit;
                    break;
                case TextAnimation.ZoomIn:
                    ExitAnimationAction = ZoomInExit;
                    break;
            }

            playingAnimation = true;
        }

        #region text effects
        private void FadeIn()
        {
            textObject.alpha += (1f / ENTER_DURATION) * Time.deltaTime;
        }
        private void FadeOut()
        {
            textObject.alpha -= (1f / EXIT_DURATION) * Time.deltaTime;
        }
        private void ZoomOutEnter()
        {
            if (textObject.fontSize <= TargetFontSize)
            {
                return;
            }

            float deltaSize = (TargetFontSize - MAX_FONT_SIZE) / ENTER_DURATION;
            textObject.fontSize += deltaSize * Time.deltaTime;
        }
        private void ZoomInEnter()
        {
            if (textObject.fontSize >= TargetFontSize)
            {
                return;
            }

            float deltaSize = (MIN_FONT_SIZE - TargetFontSize) / ENTER_DURATION;
            textObject.fontSize -= deltaSize * Time.deltaTime;
        }
        private void ZoomOutExit()
        {
            float deltaSize = (MIN_FONT_SIZE - TargetFontSize) / ENTER_DURATION;
            textObject.fontSize += deltaSize * Time.deltaTime;
        }
        private void ZoomInExit()
        {
            float deltaSize = (MIN_FONT_SIZE - TargetFontSize) / ENTER_DURATION;
            textObject.fontSize -= deltaSize * Time.deltaTime;
        }
        #endregion
    }
}
