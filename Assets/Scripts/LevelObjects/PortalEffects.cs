using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// Controls and manages all effects from a portal.
    /// </summary>
    public class PortalEffects : MonoBehaviour
    {
        public Transform Destination { get; set; }
        private EffectManager effectManager;
        private const float TRAIL_FREQUENCY = 0.2f;
        private float trailTimer = TRAIL_FREQUENCY;
        private PlayerMotion playerMotion;

        private void Awake()
        {
            effectManager = EffectManager.Instance;
            playerMotion = PlayerMotion.Instance;
        }
        private void Update()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerMotion.PlayerPosition);

            if (distanceToPlayer >= 20f) return;

            if (Destination == null) return;

            if (trailTimer <= 0f)
            {
                trailTimer = TRAIL_FREQUENCY;
                PortalLineEffect t = (PortalLineEffect)effectManager.SpawnTrail(TrailPool.Trails.PortalTrail, transform.position);
                t.Init(transform.position, Destination.position);
            }
            else
            {
                trailTimer -= Time.deltaTime;
            }
        }
    }
}
