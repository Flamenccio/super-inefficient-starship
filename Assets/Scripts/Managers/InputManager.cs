using Flamenccio.Core;
using Flamenccio.Effects;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Flamenccio.Utility
{
    /// <summary>
    /// Manages and abstracts the aiming and movement inputs so other classes don't have to worry about differentiating control schemes.
    /// </summary>
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
        [SerializeField] private PlayerInput inputMap;
        private AllAngle aimInput = new();
        private AllAngle moveInput = new();
        private bool startCalled = false;
        private Transform playerTransform;

        public enum ControlScheme
        {
            KBM,
            Gamepad
        };

        public enum ControlActionMap
        {
            Game,
            Menu
        };

        public ControlScheme CurrentScheme { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("InputManager instance destroyed");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            startCalled = true;
            playerTransform = PlayerMotion.Instance.PlayerTransform;
        }

        private void Update()
        {
            if (CurrentScheme == ControlScheme.KBM)
            {
                MouseAim();
            }
        }


        /// <summary>
        /// Change the game's current action mapping
        /// </summary>
        public void ChangeActionMap(ControlActionMap newMap)
        {
            switch (newMap)
            {
                case ControlActionMap.Game:
                    inputMap.SwitchCurrentActionMap("Game");
                    return;
                case ControlActionMap.Menu:
                    inputMap.SwitchCurrentActionMap("Menu");
                    return;
            }
        }

        public void OnControlSchemeChange(PlayerInput input)
        {
            UpdateControlScheme(input);
        }

        private void UpdateControlScheme(PlayerInput input)
        {
            if (input.currentControlScheme.Equals("Gamepad"))
            {
                MousePositionDistance = 100f;
                CurrentScheme = ControlScheme.Gamepad;
            }
            else if (input.currentControlScheme.Equals("KBM"))
            {
                CurrentScheme = ControlScheme.KBM;
            }

            WaitForGameEventManager();
        }

        private async Task WaitForGameEventManager()
        {
            await Task.Run(() =>
            {
                while (!GameEventManager.Instanced || !startCalled) { }

                GameEventManager.OnControlSchemeChange(CurrentScheme);
            });
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
            aimInput.Vector = (mousePos - (Vector2)playerTransform.position).normalized;
            MousePositionDistance = Vector2.Distance(playerTransform.position, mousePos);
        }
    }
}