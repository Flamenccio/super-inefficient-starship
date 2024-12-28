using UnityEngine;
using UnityEngine.Events;

namespace Flamenccio.Components
{
    /// <summary>
    /// Disables scripts if the distance from the player exceeds a given amount.
    /// </summary>
    public class DistanceBehaviorCull : MonoBehaviour
    {
        public interface IDistanceDisable
        {
            public void Disable();
            public void Enable();
        }

        [SerializeField] private float distanceThreshold = 20f; // 20 is default distance.
        private Transform player;
        private bool scriptsEnabled = true;
        [Tooltip("Methods that will be invoked when this GameObject is too far from player.")] public UnityEvent OnDistanceDisable;
        [Tooltip("Methods that will be invoked when this GameObject returns to working distance from player.")] public UnityEvent OnDistanceEnable;
        
        private void Update()
        {
            if (Vector2.Distance(transform.position, player.position) > distanceThreshold)
            {
                SetScriptsActive(false);
            }
            else
            {
                SetScriptsActive(true);
            }
        }

        private void SetScriptsActive(bool enable)
        {
            if (scriptsEnabled == enable) return;

            scriptsEnabled = enable;

            if (enable)
            {
                OnDistanceEnable?.Invoke();
            }
            else
            {
                OnDistanceDisable?.Invoke();
            }
        }
    }
}