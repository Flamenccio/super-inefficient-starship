using Flamenccio.Effects;
using Flamenccio.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Flamenccio.Core.Player
{
    public class InputManager : MonoBehaviour
    {
        public float AimInputRadian { get => aimInput.Radian; }
        public float AimInputDegrees { get => aimInput.Degree; }
        public Vector2 AimInputVector { get => aimInput.Vector; }
        public float MoveInputRadian { get =>  moveInput.Radian; }
        public float MoveInputDegrees { get => moveInput.Radian; }
        public Vector2 MoveInputVector { get => moveInput.Vector; }

        private AllAngle aimInput = new();
        private AllAngle moveInput = new();
        private PlayerInput playerInput;
        private PlayerMotion playerMotion;
        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            playerMotion = GetComponent<PlayerMotion>();
        }
        private void Update()
        {
            
        }
        public void OnAim(InputAction.CallbackContext context)
        {
            if (playerInput.currentControlScheme.Equals("XBOX"))
            {
                aimInput.Vector = context.ReadValue<Vector2>();
            }
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerMotion.MovementRestricted) return;

            moveInput.Vector = context.ReadValue<Vector2>();
        }
        private void Aim()
        {
            if (playerMotion.AimRestricted) return;

            if (playerInput.currentControlScheme.Equals("KBM"))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                aimInput.Vector = mousePos - (Vector2)transform.position;
            }
        }
    }
}
