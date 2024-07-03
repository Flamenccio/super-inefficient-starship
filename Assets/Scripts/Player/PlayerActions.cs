using UnityEngine.InputSystem;
using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Powerup;
using System;
using Flamenccio.Effects;

namespace Flamenccio.Core.Player
{
    /// <summary>
    /// Manages the execution of the player's actions.
    /// </summary>
    public class PlayerActions : MonoBehaviour
    {
        [SerializeField] private PowerupManager powerManager;
        [SerializeField] private PlayerAttributes playerAtt;
        [SerializeField] private AimAssist aimAssist;

        private enum AttackState
        {
            Tap,
            Hold,
        };

        private Action dynamicAim;
        private AttackState mainAttackState = AttackState.Tap;
        private AttackState subAttackState = AttackState.Tap;
        private Rigidbody2D rb;
        private AllAngle rotationAngle; // used for rotating player sprite
        private PlayerMotion playerMotion;
        private InputManager input;
        private bool mainPressed = false;
        private bool subPressed = false;
        private float mainHold = 0.0f;
        private float subHold = 0.0f;
        private float acceleration = 1.0f;
        private float deceleration = 1.0f;
        private float kbmFireTimer = 0f;
        private float attackAngleDegrees = 0f; // where to attack, different from input.aimangle
        private readonly float aimResponsiveness = 0.6f;
        private const float HOLD_THRESHOLD = 0.26f;
        private const float KBM_FIRE_TIMER_MAX = 2.0f;

        public Rigidbody2D Rigidbody { get => rb; }

        private void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            input = InputManager.Instance;
            acceleration = playerAtt.MoveSpeed / 4f;
            deceleration = playerAtt.MoveSpeed / 26f;
            playerMotion = PlayerMotion.Instance;
            GameEventManager.OnControlSchemeChange += SwitchAimScheme;
        }

        private void FixedUpdate()
        {
            if (playerMotion == null)
            {
                playerMotion = PlayerMotion.Instance;
            }

            if (!playerMotion.MovementRestricted)
            {
                Movement();
            }

            dynamicAim?.Invoke();
        }

        private void Update()
        {
            if (kbmFireTimer > 0f) kbmFireTimer -= Time.deltaTime;

            HoldAttack(ref mainPressed, ref mainAttackState, ref mainHold, powerManager.MainAttackHold, powerManager.MainAttackHoldEnter, powerManager.MainAttackHoldExit);
            HoldAttack(ref subPressed, ref subAttackState, ref subHold, powerManager.SubAttackHold, powerManager.SubAttackHoldEnter, powerManager.SubAttackHoldExit);
        }

        public void OnFire1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                kbmFireTimer = KBM_FIRE_TIMER_MAX;
                AttackTap(powerManager.MainAttackTap, powerManager.MainAttackAimAssisted);
            }

            mainPressed = context.ReadValueAsButton();
        }

        private void AttackWithAimAssist(Action<float, float, Vector2> attack)
        {
            float a;

            if (aimAssist.Target != null)
            {
                GameObject t = aimAssist.Target;
                a = Mathf.Rad2Deg * Mathf.Atan2(t.transform.position.y - transform.position.y, t.transform.position.x - transform.position.x);
            }
            else
            {
                a = attackAngleDegrees;
            }

            attack(a, input.MoveInputDegrees, transform.position);
        }

        private void AttackTap(Action<float, float, Vector2> attack, bool aimAssist)
        {
            if (aimAssist)
            {
                AttackWithAimAssist(attack);
            }
            else
            {
                attack(attackAngleDegrees, input.MoveInputDegrees, transform.position);
            }
        }

        /// <summary>
        /// Manages hold attacks.
        /// </summary>
        /// <param name="actionPressed">The boolean value signifying that an action is pressed.</param>
        /// <param name="state">The AttackState enum signifying the state of an attack.</param>
        /// <param name="holdTimer">The timer used to confirm a button hold.</param>
        /// <param name="hold">Action called every frame button is held.</param>
        /// <param name="enter">Action called once entering hold attack state.</param>
        /// <param name="exit">Action called once exiting hold attack state.</param>
        private void HoldAttack(ref bool actionPressed, ref AttackState state, ref float holdTimer, Action<float, float, Vector2> hold, Action<float, float, Vector2> enter, Action<float, float, Vector2> exit)
        {
            if (actionPressed)
            {
                kbmFireTimer = KBM_FIRE_TIMER_MAX;

                if (holdTimer >= HOLD_THRESHOLD)
                {
                    if (state == AttackState.Tap)
                    {
                        enter(attackAngleDegrees, input.MoveInputDegrees, transform.position);
                        state = AttackState.Hold;
                    }
                    else
                    {
                        hold(attackAngleDegrees, input.MoveInputDegrees, transform.position);
                    }
                }
                else
                {
                    holdTimer += Time.deltaTime;
                }
            }
            else
            {
                if (state == AttackState.Hold)
                {
                    exit(attackAngleDegrees, input.MoveInputDegrees, transform.position);
                    state = AttackState.Tap;
                }

                holdTimer = 0f;
            }
        }

        public void OnDefense(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackTap(powerManager.DefenseAttackTap, powerManager.DefenseAttackAimAssisted);
            }
        }

        public void OnSpecial(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                kbmFireTimer = KBM_FIRE_TIMER_MAX;
                AttackTap(powerManager.SpecialAttackTap, powerManager.SpecialAttackAimAssisted);
            }
        }

        public void OnSub(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                kbmFireTimer = KBM_FIRE_TIMER_MAX;
                AttackTap(powerManager.SubAttackTap, powerManager.SubAttackAimAssisted);
            }

            subPressed = context.ReadValueAsButton();
        }

        private void Movement()
        {
            if (input.MoveInputVector != Vector2.zero) // if the player is providing direcitonal input:
            {
                Vector2 newVelocity = new((input.MoveInputVector.x * acceleration) + rb.velocity.x, (input.MoveInputVector.y * acceleration) + rb.velocity.y);

                newVelocity = Vector2.ClampMagnitude(newVelocity, playerAtt.MoveSpeed);

                rb.velocity = newVelocity;
            }
            else if (rb.velocity.magnitude > 0f) // otherwise, if no directional input is given:
            {
                rb.velocity = new Vector2(rb.velocity.x - (rb.velocity.x * deceleration), rb.velocity.y - (rb.velocity.y * deceleration));
            }
        }

        private void SwitchAimScheme(InputManager.ControlScheme scheme)
        {
            switch (scheme)
            {
                case InputManager.ControlScheme.XBOX:
                    dynamicAim = GamepadAim;
                    break;

                case InputManager.ControlScheme.KBM:
                    dynamicAim = MouseAim;
                    break;
            }
        }

        private void GamepadAim()
        {
            if (input.AimInputVector != Vector2.zero)
            {
                rotationAngle.Degree = Mathf.LerpAngle(rotationAngle.Degree, input.AimInputDegrees, aimResponsiveness);
                attackAngleDegrees = input.AimInputDegrees;
            }
            else if (input.MoveInputVector != Vector2.zero)
            {
                rotationAngle.Degree = Mathf.LerpAngle(rotationAngle.Degree, input.MoveInputDegrees, aimResponsiveness);
                attackAngleDegrees = input.MoveInputDegrees;
            }

            rb.rotation = Mathf.LerpAngle(rb.rotation, rotationAngle.Degree, aimResponsiveness);
        }

        private void MouseAim()
        {
            if (kbmFireTimer > 0f)
            {
                rotationAngle.Degree = Mathf.LerpAngle(rotationAngle.Degree, input.AimInputDegrees, aimResponsiveness);
                rb.rotation = Mathf.LerpAngle(rb.rotation, rotationAngle.Degree, aimResponsiveness);
            }
            else if (input.MoveInputVector != Vector2.zero)
            {
                rb.rotation = Mathf.LerpAngle(rb.rotation, input.MoveInputDegrees, aimResponsiveness);
            }

            attackAngleDegrees = input.AimInputDegrees;
        }

        public void OnDebug1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // debug
            }
        }
    }
}