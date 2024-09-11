using Flamenccio.Effects.Visual;
using UnityEngine;

namespace Flamenccio.Objects
{
    /// <summary>
    /// Controls an object that allows the player to teleport to another associated portal.
    /// </summary>
    public class Portal : MonoBehaviour, Core.IObject
    {
        public Color PortalColor
        {
            get => portalColor;
            set
            {
                portalColor = value;
                spriteren.color = portalColor;
            }
        }
        public string GetObjectName { get => objectName; }
        [SerializeField] private Portal destination;
        [SerializeField] private PortalEffects effects;
        [SerializeField] private SpriteRenderer spriteren;
        [SerializeField] private string objectName;
        private const float COOLDOWN = 1.0f;
        private float cooldownTimer = 0f; // whenever player uses portal, cooldown is used to prevent player from rapidly moving between portals
        private Color portalColor;

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
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
    }
}