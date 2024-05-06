using Flamenccio.Effects.Visual;
using UnityEngine;

namespace Flamenccio.LevelObject
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Portal destination;
        [SerializeField] private PortalEffects effects;
        [SerializeField] private SpriteRenderer spriteren;
        public Color PortalColor { get; set; }
        private const float COOLDOWN = 1.0f;
        private float cooldownTimer = 0f; // whenever player uses portal, cooldown is used to prevent player from rapidly moving between portals

        private void Start()
        {
            if (destination != null) effects.Destination = destination.transform;
        }
        public bool SetDestination(Portal destination)
        {
            if (destination == null) return false;

            if (this.destination != null) return false;

            if (destination.gameObject.GetInstanceID() == gameObject.GetInstanceID()) return false;

            this.destination = destination;
            effects.Destination = this.destination.transform;
            return true;
        }
        public Transform GetDestination()
        {
            if (destination == null) return null;

            return destination.transform;
        }
        public bool TriggerCooldown()
        {
            if (cooldownTimer > 0f) return false;

            cooldownTimer = COOLDOWN;
            destination.ForceTriggerCooldown();
            return true;
        }
        public void ForceTriggerCooldown()
        {
            cooldownTimer = COOLDOWN;
        }
        private void Update()
        {
            spriteren.color = PortalColor;

            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
    }
}
