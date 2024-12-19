using UnityEngine.InputSystem;
using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Powerup;
using System;
using Flamenccio.Effects;
using Flamenccio.Powerup.Weapon;

namespace Flamenccio.Core.Player
{
    /// <summary>
    /// Manages the execution of the player's actions.
    /// </summary>
    public class PlayerActions : MonoBehaviour
    {
        public Rigidbody2D Rigidbody { get => rb; }

        [SerializeField] private PlayerAttributes playerAtt;
        [SerializeField] private AimAssist aimAssist;
        [SerializeField] private WeaponManager weaponManager;

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

            Attack(ref mainPressed, ref mainAttackState, ref mainHold, weaponManager.MainAttackHold, weaponManager.MainAttackHoldEnter, weaponManager.MainAttackHoldExit, weaponManager.MainAttackTap, weaponManager.MainAttackAimAssisted);
            Attack(ref subPressed, ref subAttackState, ref subHold, weaponManager.SubAttackHold, weaponManager.SubAttackHoldEnter, weaponManager.SubAttackHoldExit, weaponManager.SubAttackTap, weaponManager.SubAttackAimAssisted);
        }


        #region Attacking
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
        /// Manages button behavior on attacks (hold and tap).
        /// </summary>
        /// <param name="actionPressed">The boolean value signifying that an action is pressed.</param>
        /// <param name="state">The AttackState enum signifying the state of an attack.</param>
        /// <param name="holdTimer">The timer used to confirm a button hold.</param>
        /// <param name="hold">Action called every frame button is held.</param>
        /// <param name="enter">Action called once entering hold attack state.</param>
        /// <param name="exit">Action called once exiting hold attack state.</param>
        /// <param name="tap">Action called if button is released before a hold attack is registered.</param>
        /// <param name="aimAssist">Does this attack use aim assist?</param>
        private void Attack(ref bool actionPressed, ref AttackState state, ref float holdTimer, Action<float, float, Vector2> hold, Action<float, float, Vector2> enter, Action<float, float, Vector2> exit, Action<float, float, Vector2> tap, bool aimAssist)
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
                else if (holdTimer > 0f)
                {
                    AttackTap(tap, aimAssist);
                }

                holdTimer = 0f;
            }
        }
        #endregion

        #region Unity events
        public void OnMain(InputAction.CallbackContext context)
        {
            mainPressed = context.ReadValueAsButton();
        }

        public void OnDefense(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackTap(weaponManager.DefenseAttackTap, weaponManager.DefenseAttackAimAssisted);
            }
        }

        public void OnSpecial(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                kbmFireTimer = KBM_FIRE_TIMER_MAX;
                AttackTap(weaponManager.SpecialAttackTap, weaponManager.SpecialAttackAimAssisted);
            }
        }

        public void OnSub(InputAction.CallbackContext context)
        {
            subPressed = context.ReadValueAsButton();
        }

        public void OnDebug1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // debug
            }
        }
        #endregion

        private void Movement()
        {
            if (input.MoveInputVector != Vector2.zero) // if the player is providing direcitonal input:
            {
                Vector2 newVelocity = new((input.MoveInputVector.x * acceleration) + rb.linearVelocity.x, (input.MoveInputVector.y * acceleration) + rb.linearVelocity.y);
                newVelocity = Vector2.ClampMagnitude(newVelocity, playerAtt.MoveSpeed);
                rb.linearVelocity = newVelocity;
            }
            else if (rb.linearVelocity.magnitude > 0f) // otherwise, if no directional input is given:
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x - (rb.linearVelocity.x * deceleration), rb.linearVelocity.y - (rb.linearVelocity.y * deceleration));
            }
        }

        #region Aiming
        private void SwitchAimScheme(InputManager.ControlScheme scheme)
        {
            switch (scheme)
            {
                case InputManager.ControlScheme.Gamepad:
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
        #endregion
    }
}