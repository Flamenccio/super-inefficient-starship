using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Flamenccio.Core
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private Transform playerPosition;
        private Vector2 moveDir;
        private Vector2 aimDir;
        private Camera cam;
        // modifies the camera's position from the player's position
        private float cameraPositionModX = 0.0f;
        private float cameraPositionModY = 0.0f;
        private float currentSize = 0.0f;
        private float previousSize = 0.0f;
        private float t = 0;
        private bool interruptAdjustment = false;
        private const float CAMERA_SIZE_CHANGE = 0.5f; // amount to increase size by every time stage is expanded
        private const float CAMERA_MOVE_SPEED = 0.03f; // the speed the camera will move to its new location
        private const float PEEK_DISTANCE_Y = 4.0f; // the distance the camera will move in the direction the player is travelling (y axis)
        private const float PEEK_DISTANCE_X = 8.0f; // same thing but for x axis
        private const float DEFAULT_SIZE = 6.0f;
        private const float MAX_SIZE = 12.0f;
        private const float MINIMUM_SIZE_DIFFERENCE = 0.1f;
        private const float HURT_ZOOM_DURATION = 0.25f;

        public void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
            cam.orthographicSize = DEFAULT_SIZE;
            currentSize = DEFAULT_SIZE;
            previousSize = DEFAULT_SIZE;
            Application.targetFrameRate = 60;
        }
        public void FixedUpdate()
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

            if (aimDir != Vector2.zero)
            {
                cameraPositionModX = aimDir.x * PEEK_DISTANCE_X;
                cameraPositionModY = aimDir.y * PEEK_DISTANCE_Y;
            }
            else if (moveDir != Vector2.zero)
            {
                cameraPositionModX = moveDir.x * (PEEK_DISTANCE_X - 4);
                cameraPositionModY = moveDir.y * (PEEK_DISTANCE_Y - 1);
            }
            else // reset camera position
            {
                cameraPositionModX = 0.0f;
                cameraPositionModY = 0.0f;
            }

            Vector3 cameraNewPosition = new(playerPosition.position.x + cameraPositionModX, playerPosition.position.y + cameraPositionModY, gameObject.transform.position.z);

            gameObject.transform.position = new(Mathf.Lerp(transform.position.x, cameraNewPosition.x, CAMERA_MOVE_SPEED), Mathf.Lerp(transform.position.y, cameraNewPosition.y, CAMERA_MOVE_SPEED), cameraNewPosition.z);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveDir = context.ReadValue<Vector2>();
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
        public void OnAim(InputAction.CallbackContext context)
        {
            aimDir = context.ReadValue<Vector2>();
        }
    }
}
