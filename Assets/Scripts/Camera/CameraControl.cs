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
        public bool InterruptSizeUpdate { get; private set; } = false;
        public bool InterruptPositionUpdate { get; private set; } = false;
        public float PreviousSize { get; private set; }
        public float TargetSize { get; private set; }

        private Transform target;
        private InputManager input;
        private Camera cam;
        private Vector2 cameraOffset = Vector2.zero; // modifies the camera's position from the player's position
        private float interpolateSize = 0f;
        private float cameraMoveSpeed;
        private Action UpdateCameraPosition;

        private const float CAMERA_SIZE_CHANGE = 0.5f; // amount to increase size by every time stage is expanded
        private const float CAMERA_MOVE_SPEED_GAMEPAD = 0.03f; // the speed the camera will move to its new location
        private const float CAMERA_MOVE_SPEED_KBM = 0.08f;
        private const float PEEK_DISTANCE_Y = 4.0f; // the distance the camera will move in the direction the player is travelling (y axis)
        private const float PEEK_DISTANCE_X = 8.0f; // same thing but for x axis
        private const float DEFAULT_SIZE = 6.0f;
        private const float MAX_SIZE = 12.0f;

        private void Awake()
        {
            cam = Camera.main;
            cam.orthographicSize = DEFAULT_SIZE;
            PreviousSize = DEFAULT_SIZE;
            TargetSize = DEFAULT_SIZE;
        }

        private void Start()
        {
            input = InputManager.Instance;
            Follow(PlayerMotion.Instance.transform);

            GameEventManager.OnLevelUp += (_) => IncreaseCameraSize();
            GameEventManager.OnControlSchemeChange += (x) => UpdateControlScheme(x);
        }

        private void FixedUpdate()
        {
            if (input == null) input = InputManager.Instance;

            UpdateSize();
            UpdatePosition();
        }

        /// <summary>
        /// Tell the camera to follow a Transform. Fails if given Transform is null.
        /// </summary>
        /// <param name="target">Transform to follow.</param>
        public void Follow(Transform target)
        {
            if (target == null) return;

            this.target = target;
        }

        /// <summary>
        /// Temporarily prevents this class from controlling the main camera's orthographic size. Has no effect if another interrupt is in progress.
        /// </summary>
        /// <param name="durationSeconds">How long to stop the size updates.</param>
        public bool StopSizeUpdate(float durationSeconds)
        {
            if (InterruptSizeUpdate) return false;

            StartCoroutine(StopSizeUpdateCoroutine(durationSeconds));
            return true;
        }

        private IEnumerator StopSizeUpdateCoroutine(float durationSeconds)
        {
            InterruptSizeUpdate = true;
            yield return new WaitForSecondsRealtime(durationSeconds);
            InterruptSizeUpdate = false;
        }

        /// <summary>
        /// Temporarily prevents this class from controlling the main camera's position. Has no effect if another interrupt is in progress.
        /// </summary>
        /// <param name="durationSeconds">How long to sotp the position updates.</param>
        public bool StopPositionUpdate(float durationSeconds)
        {
            if (InterruptPositionUpdate) return false;

            StartCoroutine(StopPositionUpdateCoroutine(durationSeconds));
            return true;
        }

        private IEnumerator StopPositionUpdateCoroutine(float durationSeconds)
        {
            InterruptPositionUpdate = true;
            yield return new WaitForSecondsRealtime(durationSeconds);
            InterruptPositionUpdate = false;
        }

        /// <summary>
        /// Temporarily stop both position and size updates of the camera.
        /// </summary>
        /// <param name="positionSeconds">How long to stop position updates.</param>
        /// <param name="sizeSeconds">How long to stop size updates.</param>
        /// <param name="enforceSimultaneous">Passing <b>true</b> will ensure that either both updates succeed; if one is unable to succeed, neither interrupts will be run.</param>
        public bool StopPositionAndSizeUpdates(float positionSeconds, float sizeSeconds, bool enforceSimultaneous)
        {
            bool isInterrupted = InterruptSizeUpdate || InterruptPositionUpdate;

            if (enforceSimultaneous && isInterrupted) return false;

            StopPositionUpdate(positionSeconds);
            StopSizeUpdate(sizeSeconds);
            return true;
        }

        private void UpdateSize()
        {
            if (InterruptSizeUpdate) return;

            if (interpolateSize < 1f)
            {
                float logZoom = Mathf.SmoothStep(Mathf.Log(PreviousSize), Mathf.Log(TargetSize), interpolateSize);
                cam.orthographicSize = Mathf.Exp(logZoom);
                interpolateSize = Mathf.Clamp01(interpolateSize + Time.unscaledDeltaTime);
            }
            else
            {
                PreviousSize = cam.orthographicSize;
            }
        }

        private void UpdatePosition()
        {
            if (InterruptPositionUpdate) return;

            UpdateCameraPosition?.Invoke();

            Vector3 cameraNewPosition = new(target.position.x + cameraOffset.x, target.position.y + cameraOffset.y, transform.position.z);
            transform.position = new(Mathf.Lerp(transform.position.x, cameraNewPosition.x, cameraMoveSpeed), Mathf.Lerp(transform.position.y, cameraNewPosition.y, cameraMoveSpeed), cameraNewPosition.z);
        }

        private void UpdateControlScheme(InputManager.ControlScheme scheme)
        {
            switch (scheme)
            {
                case InputManager.ControlScheme.KBM:
                    cameraMoveSpeed = CAMERA_MOVE_SPEED_KBM;
                    UpdateCameraPosition = UpdateCameraPositionKBM;
                    break;

                case InputManager.ControlScheme.Gamepad:
                    cameraMoveSpeed = CAMERA_MOVE_SPEED_GAMEPAD;
                    UpdateCameraPosition = UpdateCameraPositionGamepad;
                    break;
            }
        }

        private void UpdateCameraPositionKBM()
        {
            cameraOffset = Vector2.zero;
        }

        private void UpdateCameraPositionGamepad()
        {
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
            SetCameraSize(cam.orthographicSize + CAMERA_SIZE_CHANGE);
        }

        /// <summary>
        /// Smoothly changes camera's orthographic size.
        /// </summary>
        /// <param name="newSize">New orthographic size.</param>
        public void SetCameraSize(float newSize)
        {
            if (cam.orthographicSize >= MAX_SIZE) return;

            interpolateSize = 0f;
            TargetSize = newSize;
            PreviousSize = cam.orthographicSize;
        }
    }
}