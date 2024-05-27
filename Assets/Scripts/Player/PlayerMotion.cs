using Flamenccio.Effects.Visual;
using System.Collections;
using UnityEngine;

namespace Flamenccio.Effects
{
    /// <summary>
    /// Allows outside classes to temporarily affect player movement and physics.
    /// </summary>
    public class PlayerMotion : MonoBehaviour
    {
        public bool MovementRestricted { get => GetRestrictedBit(Restrictions.Movement); }
        public bool ActionRestricted { get; private set; }
        public bool AimRestricted { get => GetRestrictedBit(Restrictions.Aim); }
        public static PlayerMotion Instance { get; private set; }
        public Vector2 PlayerPosition { get => transform.position; }
        public Transform PlayerTransform { get => transform; }
        public Vector2 PlayerVelocity { get => rb.velocity; }
        [SerializeField] private Rigidbody2D rb;

        private enum Restrictions
        {
            Movement,
            Aim,
            Ability
        }

        private const float KNOCKBACK_DURATION = 6f / 60f;
        private int playerRestrictions;

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
        /// Instantly and instantaneously moves player to given global coordinate.
        /// </summary>
        /// <param name="coord">Global coordinate to move to.</param>
        public void TeleportTo(Vector2 coord)
        {
            transform.position = coord;
            CameraEffects.Instance.CutToPosition(new Vector3(coord.x, coord.y, -10));
        }

        /// <summary>
        /// Instantly and instantaneously moves player to position relative to some transform.
        /// </summary>
        /// <param name="offset">Offset from transform's position.</param>
        /// <param name="origin">Transform to teleport relative to.</param>
        public void TeleportRelativeTo(Vector2 offset, Transform origin)
        {
            TeleportTo(offset + (Vector2)origin.position);
        }

        /// <summary>
        /// Prevents player from moving via input for some given amount of time. <para>Note that only <b>one</b> restriction may exist at any time; repeatedly restricting the player character will not accumulate restriction time.</para>
        /// </summary>
        /// <param name="t">Time in seconds.</param>
        public bool RestrictMovement(float t)
        {
            if (MovementRestricted) return false;
            StartCoroutine(RestrictPlayer(t, Restrictions.Movement));
            return true;
        }

        /// <summary>
        /// Prevents player from taking any actions (e.g. using weapons) for some given amount of time.
        /// </summary>
        /// <param name="t">Time in seconds.</param>
        public bool RestrictAbility(float t)
        {
            if (ActionRestricted) return false;
            StartCoroutine(RestrictPlayer(t, Restrictions.Ability));
            return true;
        }

        public bool RestrictAim(float t)
        {
            if (AimRestricted) return false;
            StartCoroutine(RestrictPlayer(t, Restrictions.Aim));
            return true;
        }

        private IEnumerator RestrictPlayer(float t, Restrictions restriction)
        {
            int r = (int)restriction;
            playerRestrictions |= (1 << r); // set bit
            yield return new WaitForSeconds(t);
            playerRestrictions &= ~(1 << r); // clear bit
        }

        private bool GetRestrictedBit(Restrictions restrictions)
        {
            int x = playerRestrictions | (1 << (int)restrictions);
            return x == playerRestrictions;
        }

        /// <summary>
        /// Exert an impulse on the player character.
        /// </summary>
        public void Shove(Vector2 direction, float magnitude)
        {
            direction.Normalize();
            rb.AddForce(direction * magnitude, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Exert a force on the player character.
        /// </summary>
        public void Push(Vector2 direction, float magnitude)
        {
            direction.Normalize();
            rb.AddForce(direction * magnitude, ForceMode2D.Force);
        }

        /// <summary>
        /// Linearly move the player in specified direction, speed, and time.
        /// </summary>
        public void Move(Vector2 direction, float speed, float t)
        {
            direction.Normalize();
            RestrictMovement(t);
            rb.velocity = direction * speed;
        }

        /// <summary>
        /// Shorthand for moving player character with fixed duration.
        /// </summary>
        public void Knockback(Vector2 direction, float power)
        {
            Move(direction, power, KNOCKBACK_DURATION);
        }

        /// <summary>
        /// Allows player to pass through enemies (not bullets!) for specified time.
        /// </summary>
        public void Blink(float duration)
        {
            if (gameObject.layer.Equals(LayerMask.NameToLayer("PlayerIntangible"))) return;
            StartCoroutine(BlinkCoroutine(duration));
        }

        private IEnumerator BlinkCoroutine(float t)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerIntangible");
            yield return new WaitForSeconds(t);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}