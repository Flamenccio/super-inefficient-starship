using UnityEngine.InputSystem;
using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Powerup;
using System;
using Flamenccio.Effects;

namespace Flamenccio.Core.Player
{
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
        private AttackState mainAttackState = AttackState.Tap;
        private Rigidbody2D rb;
        private AllAngle rotationAngle; // used for rotating player sprite 
        private bool mainPressed = false;
        private float mainHold = 0.0f;
        private float acceleration = 1.0f;
        private float deceleration = 1.0f;
        private float kbmFireTimer = 0f;
        private readonly float aimResponsiveness = 0.6f;
        private const float HOLD_THRESHOLD = 0.50f;
        private const float KBM_FIRE_TIMER_MAX = 2.0f;
        private PlayerMotion playerMotion;
        private InputManager input;

        public Rigidbody2D Rigidbody { get => rb; }

        private void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            input = InputManager.Instance;
            acceleration = playerAtt.MoveSpeed / 4f;
            deceleration = playerAtt.MoveSpeed / 26f;
            playerMotion = PlayerMotion.Instance;
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
            Aim();
        }
        private void Update()
        {
            if (kbmFireTimer > 0f) kbmFireTimer -= Time.deltaTime;

            Fire1Hold();
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
                a = input.AimInputDegrees;
            }

            attack?.Invoke(a, input.MoveInputDegrees, transform.position);
        }
        private void AttackTap(Action<float, float, Vector2> attack, bool aimAssist)
        {
            if (aimAssist)
            {
                AttackWithAimAssist(attack);
            }
            else
            {
                attack?.Invoke(input.AimInputDegrees, input.MoveInputDegrees, transform.position); // TODO maybe avoid using delegates...
            }
        }
        private void Fire1Hold()
        {
            if (mainPressed)
            {
                kbmFireTimer = KBM_FIRE_TIMER_MAX;

                if (mainHold >= HOLD_THRESHOLD)
                {
                    if (mainAttackState == AttackState.Tap)
                    {
                        powerManager.MainAttackHoldEnter?.Invoke(input.AimInputDegrees,input.MoveInputDegrees, transform.position);
                        mainAttackState = AttackState.Hold;
                    }
                    else
                    {
                        powerManager.MainAttackHold?.Invoke(input.AimInputDegrees, input.MoveInputDegrees, transform.position);
                    }
                }
                else
                {
                    mainHold += Time.deltaTime;
                }
            }
            else
            {
                if (mainAttackState == AttackState.Hold)
                {
                    powerManager.MainAttackHoldExit?.Invoke(input.AimInputDegrees, input.MoveInputDegrees, transform.position);
                    mainAttackState = AttackState.Tap;
                }
                mainHold = 0f;
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
        private void Aim()
        {
            if (input.CurrentScheme == InputManager.ControlScheme.XBOX)
            {
                GamepadAim();
            }
            if (input.CurrentScheme == InputManager.ControlScheme.KBM)
            {
                MouseAim();
            }
        }
        private void GamepadAim()
        {
            if (input.AimInputVector != Vector2.zero)
            {
                rotationAngle.Degree = Mathf.LerpAngle(rotationAngle.Degree, input.AimInputDegrees, aimResponsiveness);
            }
            else if (input.MoveInputVector != Vector2.zero)
            {
                rotationAngle.Degree = Mathf.LerpAngle(rotationAngle.Degree, input.MoveInputDegrees, aimResponsiveness);
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
        }
        public void OnDebug1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                BuffBase b = new MovementSpeed();
                powerManager.AddBuff(b);
            }
        }
    }
}
