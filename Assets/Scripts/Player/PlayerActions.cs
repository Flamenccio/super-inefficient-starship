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
    private Vector2 moveInput; // directional input
    private Vector2 aimInput;
    private Rigidbody2D rb;

    private AllAngle aimAngle;
    private float dashCD = 1.0f;
    private bool mainPressed = false;
    private float mainHold = 0.0f;
    private float acceleration = 1.0f;
    private float deceleration = 1.0f;
    private float aimResponsiveness = 0.6f;

    private const float DASH_DURATION = 5f / 60f;
    private const float DASH_COOLDOWN = 1.0f;
    private const float DASH_SPEED = 50.0f;
    private const float HOLD_THRESHOLD = 0.50f;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private CooldownControl cdControl;
    [SerializeField] private GameState gameState;
    [SerializeField] private PowerupManager powerManager;
    [SerializeField] private PlayerAttributes playerAtt;

    public Rigidbody2D Rigidbody { get => rb; }

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        acceleration = playerAtt.MoveSpeed / 4f;
        deceleration = playerAtt.MoveSpeed / 26f;
    }
    private void FixedUpdate()
    {
        if (!PlayerMotion.Instance.MovementRestricted)
        {
            Movement();
        }
        Aim();

        // HACK temporary
        if (dashCD > 0.0f)
        {
            dashCD -= Time.deltaTime;
        }
        else
        {
            dashCD = 0.0f;
        }
        if (DASH_COOLDOWN - dashCD <= DASH_DURATION)
        {
            Instantiate(afterImagePrefab, transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
        }
    }
    private void Update()
    {
        // MAIN WEAPON HOLD ATTACK
        if (mainPressed)
        {
            if (mainHold >= HOLD_THRESHOLD)
            {
                if (mainAttackState == AttackState.Tap)
                {
                    powerManager.MainAttackEffectHoldEnter(rb);
                    mainAttackState = AttackState.Hold;
                }
                else
                {
                    powerManager.MainAttackEffectHoldContinuous(rb);
                }
                powerManager.MainAttackHold(aimAngle.Degree, transform.position, playerAtt.UseAmmo, Time.deltaTime);
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
                powerManager.MainAttackEffectHoldExit(rb);
                mainAttackState = AttackState.Tap;
            }
            mainHold = 0f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // store the input vector
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>(); // store the input vector
    }

    public void OnFire1(InputAction.CallbackContext context)
    {
        // if held down, then do charged fire
        // if released without holding down long enough, do normal fire
        if (context.performed)
        {
            //AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShoot, transform.position);
            if (AimAssist.instance.Target != null)
            {
                GameObject t = AimAssist.instance.Target;
                float a = Mathf.Rad2Deg * Mathf.Atan2(t.transform.position.y - transform.position.y, t.transform.position.x - transform.position.x);

                powerManager.MainAttackTap(a, transform.position, playerAtt.UseAmmo);
                //powerManager.mainAttackTap(a, transform.position, playerAtt.UseAmmo);
            }
            else
            {
                powerManager.MainAttackTap(aimAngle.Degree, transform.position, playerAtt.UseAmmo);
                //powerManager.mainAttackTap(aimAngle.Degree, transform.position, playerAtt.UseAmmo);
            }
        }
        mainPressed = context.ReadValueAsButton();
    }
    public void OnFire2(InputAction.CallbackContext context) // HACK temporary
    {
        /*
        if (context.performed && dashCD == 0 && !PlayerMotion.Instance.MovementRestricted)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDash, transform.position);
            // dash in the left stick direction
            //StartCoroutine(Dash());
            DashTemp();
            dashCD = DASH_COOLDOWN;

            // activate cooldown gauge
            //cdControl.Display(1.0f);
        }
        */
        if (context.performed)
        {
            powerManager.SubAttackTap(aimAngle.Degree, transform.position, playerAtt.UseAmmo);
        }
    }
    private void Movement()
    {
        // if the player is providing direcitonal input:
        if (moveInput != Vector2.zero)
        {
            Vector2 newVelocity = new Vector2(moveInput.x * acceleration + rb.velocity.x, moveInput.y * acceleration + rb.velocity.y);

            //newVelocity = Vector2.ClampMagnitude(newVelocity, MAX_SPEED);
            newVelocity = Vector2.ClampMagnitude(newVelocity, playerAtt.MoveSpeed);

            rb.velocity = newVelocity;
        }
        // otherwise, if no directional input is given:
        else if (rb.velocity.magnitude > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x - rb.velocity.x * deceleration, rb.velocity.y - rb.velocity.y * deceleration);
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
        else if (moveInput != Vector2.zero)
        {
            aimAngle.Degree = Mathf.LerpAngle(aimAngle.Degree, Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg, aimResponsiveness);
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
            Debug.Log(playerAtt.MoveSpeed.ToString());
        }
    }
}
