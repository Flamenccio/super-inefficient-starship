using Flamenccio.Effects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        [SerializeField, Tooltip("Monobehaviour components placed here must implement the IDistanceDisable interface.")] private List<MonoBehaviour> scripts = new();
        private Transform player;
        private bool scriptsEnabled = true;

        private void Awake()
        {
            scripts = FilterScriptList(scripts);
        }

        private void Start()
        {
            player = PlayerMotion.Instance.PlayerTransform;
        }

        private void Update()
        {
            if (Vector2.Distance(transform.position, player.position) > distanceThreshold)
            {
                EnableScripts(false);
            }
            else
            {
                EnableScripts(true);
            }
        }

        /// <summary>
        /// Filters a given list of MonoBehaviours.
        /// </summary>
        /// <returns>A list of MonoBehaviours that implement the IDistanceDisable interface.</returns>
        private List<MonoBehaviour> FilterScriptList(List<MonoBehaviour> scriptList)
        {
            return scriptList
                .Where(script => script is IDistanceDisable)
                .ToList();
        }

        private void EnableScripts(bool enable)
        {
            if (scriptsEnabled == enable) return;

            scriptsEnabled = enable;

            if (enable)
            {
                foreach (var script in scripts)
                {
                    (script as IDistanceDisable).Enable();
                }
            }
            else
            {
                foreach (var script in scripts)
                {
                    (script as IDistanceDisable).Disable();
                }
            }
        }
    }
}