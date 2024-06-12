using System.Collections;
using UnityEngine;
using Flamenccio.Utility;
using Flamenccio.Effects;
using System;

namespace Flamenccio.Core
{
    /// <summary>
    /// Controls movement of the camera.
    /// </summary>
    public class CameraControl : MonoBehaviour
    {
        private Transform playerPosition;
        private InputManager input;
        private Camera cam;
        private Vector2 cameraOffset = Vector2.zero; // modifies the camera's position from the player's position
        private float currentSize = 0.0f;
        private float previousSize = 0.0f;
        private float t = 0;
        private bool interruptAdjustment = false;
        private float cameraMoveSpeed;
        private Action UpdateCameraPosition;

        private const float CAMERA_SIZE_CHANGE = 0.5f; // amount to increase size by every time stage is expanded
        private const float CAMERA_MOVE_SPEED_GAMEPAD = 0.03f; // the speed the camera will move to its new location
        private const float CAMERA_MOVE_SPEED_KBM = 0.08f;
        private const float PEEK_DISTANCE_Y = 4.0f; // the distance the camera will move in the direction the player is travelling (y axis)
        private const float PEEK_DISTANCE_X = 8.0f; // same thing but for x axis
        private const float DEFAULT_SIZE = 6.0f;
        private const float MAX_SIZE = 12.0f;
        private const float MINIMUM_SIZE_DIFFERENCE = 0.1f;
        private const float HURT_ZOOM_DURATION = 0.25f;

        private void Awake()
        {
            cam = Camera.main;
            cam.orthographicSize = DEFAULT_SIZE;
            currentSize = DEFAULT_SIZE;
            previousSize = DEFAULT_SIZE;
        }

        private void Start()
        {
            input = InputManager.Instance;
            playerPosition = PlayerMotion.Instance.transform;

            GameEventManager.OnPlayerHit += (_) => HurtZoom();
            GameEventManager.OnLevelUp += (_) => IncreaseCameraSize();
            GameEventManager.OnControlSchemeChange += (x) => UpdateControlScheme(x);
        }

        private void FixedUpdate()
        {
            if (input == null) input = InputManager.Instance;

            UpdateSize();
            UpdatePosition();
        }

        private void UpdateSize()
        {
            float sizeDifference = Mathf.Abs(currentSize - cam.orthographicSize);

            if (sizeDifference > MINIMUM_SIZE_DIFFERENCE && !interruptAdjustment)
            {
                t += Time.unscaledDeltaTime;
                cam.orthographicSize = Mathf.Lerp(previousSize, currentSize, t);
            }
            else
            {
                t = 0;
            }
        }

        private void UpdatePosition()
        {
            UpdateCameraPosition?.Invoke();

            Vector3 cameraNewPosition = new(playerPosition.position.x + cameraOffset.x, playerPosition.position.y + cameraOffset.y, gameObject.transform.position.z);
            gameObject.transform.position = new(Mathf.Lerp(transform.position.x, cameraNewPosition.x, cameraMoveSpeed), Mathf.Lerp(transform.position.y, cameraNewPosition.y, cameraMoveSpeed), cameraNewPosition.z);
        }

        private void UpdateControlScheme(InputManager.ControlScheme scheme)
        {
            switch (scheme)
            {
                case InputManager.ControlScheme.KBM:
                    UpdateCameraPosition = UpdateCameraPositionKBM;
                    break;

                case InputManager.ControlScheme.XBOX:
                    UpdateCameraPosition = UpdateCameraPositionGamepad;
                    break;
            }
        }

        private void UpdateCameraPositionKBM()
        {
            cameraOffset = Vector2.zero;
            cameraMoveSpeed = CAMERA_MOVE_SPEED_KBM;
        }

        private void UpdateCameraPositionGamepad()
        {
            cameraMoveSpeed = CAMERA_MOVE_SPEED_GAMEPAD;
            if (input.AimInputVector != Vector2.zero)
            {
                cameraOffset.x = input.AimInputVector.x * PEEK_DISTANCE_X;
                cameraOffset.y = input.AimInputVector.y * PEEK_DISTANCE_Y;
            }
            else if (input.MoveInputVector != Vector2.zero)
            {
                cameraOffset.x = input.MoveInputVector.x * (PEEK_DISTANCE_X - 4);
                cameraOffset.y = input.MoveInputVector.y * (PEEK_DISTANCE_Y - 1);
            }
            else // reset camera position
            {
                cameraOffset = Vector2.zero;
            }
        }

        public void IncreaseCameraSize()
        {
            if (cam.orthographicSize >= MAX_SIZE) return;

            previousSize = cam.orthographicSize;
            currentSize = cam.orthographicSize + CAMERA_SIZE_CHANGE;
        }

        public void HurtZoom()
        {
            StartCoroutine(HurtZoomAnimation());
        }

        private IEnumerator HurtZoomAnimation()
        {
            interruptAdjustment = true;
            float normalSize = cam.orthographicSize;
            float newSize = normalSize - 1;
            cam.orthographicSize = Mathf.Lerp(normalSize, newSize, 1.0f);

            yield return new WaitForSecondsRealtime(HURT_ZOOM_DURATION);

            cam.orthographicSize = Mathf.Lerp(newSize, normalSize, 1.0f);
            interruptAdjustment = false;
        }
    }
}