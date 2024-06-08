using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Controls the visuals for aim assist.
    /// </summary>
    public class AimAssistTarget : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Transform target;

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (target != null)
            {
                transform.position = target.position;
                spriteRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = false;
                Hide();
            }
        }

        /// <summary>
        /// Hide the aim assist target.
        /// </summary>
        public void Hide()
        {
            target = null;
        }

        /// <summary>
        /// Place the aim assist target on some transform.
        /// </summary>
        /// <param name="target">The transform for the aim assist target to follow.</param>
        public void Show(Transform target)
        {
            if (this.target != target)
            {
                this.target = target;
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetAudioEvent("PlayerLockon"), target.position);
            }
        }
    }
}