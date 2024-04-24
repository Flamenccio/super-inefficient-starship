using UnityEngine;
using UnityEngine.InputSystem;

namespace Flamenccio.Utility
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public float AimInputDegrees { get => aimInput.Degree; }
        public float AimInputRadians { get => aimInput.Radian; }
        public Vector2 AimInputVector { get => aimInput.Vector; }
        public float MoveInputDegrees { get => moveInput.Degree; }
        public float MoveInputRadians { get => moveInput.Radian; }
        public Vector2 MoveInputVector { get => moveInput.Vector; }
        public float MousePositionDistance { get; private set; }
        private AllAngle aimInput = new();
        private AllAngle moveInput = new();
        public enum ControlScheme
        {
            KBM,
            XBOX
        };
        public ControlScheme CurrentScheme { get; private set; }
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
        private void Update()
        {
            if (CurrentScheme == ControlScheme.KBM)
            {
                MouseAim();
            }
        }
        public void OnControlSchemeChange(PlayerInput input)
        {
            UpdateControlScheme(input);
        }
        private void UpdateControlScheme(PlayerInput input)
        {
            if (input.currentControlScheme.Equals("XBOX"))
            {
                MousePositionDistance = 100f;
                CurrentScheme = ControlScheme.XBOX;
            }
            else if (input.currentControlScheme.Equals("KBM"))
            {
                CurrentScheme = ControlScheme.KBM;
            }
        }
        public void OnAim(InputAction.CallbackContext context)
        {
            if (CurrentScheme == ControlScheme.KBM) return;

            aimInput.Vector = context.ReadValue<Vector2>();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput.Vector = context.ReadValue<Vector2>();
        }
        private void MouseAim()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimInput.Vector = (mousePos - (Vector2)transform.position).normalized;
            MousePositionDistance = Vector2.Distance(transform.position, mousePos);
        }
    }
}
