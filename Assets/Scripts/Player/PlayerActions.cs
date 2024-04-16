using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.Interactions;

public class PlayerActions : MonoBehaviour
{
    private enum AttackState
    {
        Tap,
        Hold,
    };

    private AttackState mainAttackState = AttackState.Tap;
    private AllAngle moveInput; // directional input
    private Vector2 aimInput;
    private Rigidbody2D rb;
    private AllAngle aimAngle; // this value is the dampened value of ainInput, which is the raw player input
    private bool mainPressed = false;
    private float mainHold = 0.0f;
    private float acceleration = 1.0f;
    private float deceleration = 1.0f;
    private float aimResponsiveness = 0.6f;

    private const float HOLD_THRESHOLD = 0.50f;

    [SerializeField] private PowerupManager powerManager;
    [SerializeField] private PlayerAttributes playerAtt;
    private PlayerMotion playerMotion;

    public Rigidbody2D Rigidbody { get => rb; }

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
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
        Fire1Hold();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!playerMotion.MovementRestricted) moveInput.Vector = context.ReadValue<Vector2>(); // store the input vector
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!playerMotion.AimRestricted) aimInput = context.ReadValue<Vector2>(); // store the input vector
    }

    // TODO i want to generalize this so i don't have to copy the same code for the other 3 actions
    public void OnFire1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float a;
            if (AimAssist.instance.Target != null)
            {
                GameObject t = AimAssist.instance.Target;
                a = Mathf.Rad2Deg * Mathf.Atan2(t.transform.position.y - transform.position.y, t.transform.position.x - transform.position.x);
            }
            else
            {
                a = aimAngle.Degree;
            }

            powerManager.MainAttackTap(a, moveInput.Degree, transform.position);
        }
        mainPressed = context.ReadValueAsButton();
    }
    private void Fire1Hold()
    {
        if (mainPressed)
        {
            if (mainHold >= HOLD_THRESHOLD)
            {
                if (mainAttackState == AttackState.Tap)
                {
                    powerManager.MainAttackHoldEnter(aimAngle.Degree, moveInput.Degree, transform.position);
                    mainAttackState = AttackState.Hold;
                }
                else
                {
                    powerManager.MainAttackHold(aimAngle.Degree, moveInput.Degree, transform.position);
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
                powerManager.MainAttackHold(aimAngle.Degree, moveInput.Degree, transform.position);
                mainAttackState = AttackState.Tap;
            }
            mainHold = 0f;
        }
    }
    public void OnFire2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            powerManager.SubAttackTap(aimAngle.Degree, moveInput.Degree, transform.position);
        }
    }
    public void OnSpecial(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            powerManager.SpecialAttackTap(aimAngle.Degree, moveInput.Degree, transform.position);
        }
    }
    private void Movement()
    {
        if (moveInput.Vector != Vector2.zero) // if the player is providing direcitonal input:
        {
            Vector2 newVelocity = new((moveInput.Vector.x * acceleration) + rb.velocity.x, (moveInput.Vector.y * acceleration) + rb.velocity.y);

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
        // if the player actively aiming, update the aim angle
        // otherwise, update the aim angle to wherever the player is moving toward
        if (aimInput != Vector2.zero)
        {
            aimAngle.Degree = Mathf.LerpAngle(aimAngle.Degree, Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg, aimResponsiveness);
        }
        else if (moveInput.Vector != Vector2.zero)
        {
            aimAngle.Degree = Mathf.LerpAngle(aimAngle.Degree, Mathf.Atan2(moveInput.Vector.y, moveInput.Vector.x) * Mathf.Rad2Deg, aimResponsiveness);
        }
        // turn the player to face the aim angle
        rb.rotation = Mathf.LerpAngle(rb.rotation, aimAngle.Degree, aimResponsiveness);
    }
    public void OnDebug1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BuffBase b = new Agility_MovementSpeed();
            powerManager.AddBuff(b);
        }
    }
}
