using Flamenccio.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls screen effects; usually these are visual effects that appear after an event happens.
    /// </summary>
    public class ScreenEffectsControl : MonoBehaviour
    {
        [SerializeField] private RawImage vignette;
        [SerializeField] private RawImage hurtLines;

        // other classes
        [SerializeField] private GameState gState;

        private bool vignetteCrossfading = false;

        private void Start()
        {
            GameEventManager.OnPlayerHit += (_) => DisplayHurtLines();
        }

        private void Update()
        {
            UpdateVignette();
        }

        private void DisplayHurtLines()
        {
            StartCoroutine(HurtLinesAnimation());
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

        private IEnumerator HurtLinesAnimation()
        {
            hurtLines.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            hurtLines.gameObject.SetActive(false);
        }
    }
}
