using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.HUD
{
    public class AimAssistTarget : MonoBehaviour // this script controls the visuals for aim assist
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
        public void Hide()
        {
            target = null;
        }
        public void Show(Transform target)
        {
            if (this.target != target)
            {
                this.target = target;
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.crosshairsLockon, target.position);
            }
        }
    }
}
