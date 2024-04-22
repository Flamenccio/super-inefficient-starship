using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Flamenccio.Core
{
    public class CameraControl : MonoBehaviour
    {
        private Vector2 moveDir;
        private Vector2 aimDir;
        private Vector3 cameraNewPosition;
        private Camera cam;

        // modifies the camera's position from the player's position
        private float cameraPositionModX = 0.0f;
        private float cameraPositionModY = 0.0f;
        private float cameraSizeChange = 0.5f; // amount to increase size by every time stage is expanded
        private float currentSize = 0.0f;
        private float previousSize = 0.0f;
        private float t = 0;

        private const float cameraMoveSpeed = 0.03f; // the speed the camera will move to its new location
        private const float peekDistanceY = 4.0f; // the distance the camera will move in the direction the player is travelling (y axis)
        private const float peekDistanceX = 8.0f; // same thing but for x axis
        private const float defaultSize = 6.0f;
        private const float maxSize = 12.0f;

        private bool interruptAdjustment = false;
        private Color newColor = Color.white;
        private Color oldColor = Color.black;
        private bool lockColor = true;

        [SerializeField] private Transform playerPosition;

        public void Awake()
        {
            cam = gameObject.GetComponent<Camera>();
            cam.orthographicSize = defaultSize;
            currentSize = defaultSize;
            previousSize = defaultSize;
        }
        private void Update()
        {
            if (!lockColor)
            {
                cam.backgroundColor = Color.Lerp(oldColor, newColor, 0.5f);
            }

        }
        public void FixedUpdate()
        {
            // TODO increase readability
            // if the size of the stage changes,
            if (cam.orthographicSize != currentSize && !interruptAdjustment)
            {
                t += Time.unscaledDeltaTime;
                cam.orthographicSize = Mathf.Lerp(previousSize, currentSize, t);

            }
            else
            {
                t = 0;
            }

            // follow the player around smoothly

            // but if the player is aiming, move the camera to "peek" in the direction of aim
            if (aimDir != Vector2.zero)
            {
                cameraPositionModX = aimDir.x * peekDistanceX;
                cameraPositionModY = aimDir.y * peekDistanceY;
            }
            // otherwise, if the player is moving without aiming, move the camera in the direction that the player is travelling in
            else if (moveDir != Vector2.zero)
            {
                cameraPositionModX = moveDir.x * (peekDistanceX - 4);
                cameraPositionModY = moveDir.y * (peekDistanceY - 1);
            }
            // if nothing else, reset the camera position
            else
            {
                cameraPositionModX = 0.0f;
                cameraPositionModY = 0.0f;
            }

            cameraNewPosition = new Vector3(playerPosition.position.x + cameraPositionModX, playerPosition.position.y + cameraPositionModY, gameObject.transform.position.z);

            gameObject.transform.position = new Vector3(Mathf.Lerp(transform.position.x, cameraNewPosition.x, cameraMoveSpeed), Mathf.Lerp(transform.position.y, cameraNewPosition.y, cameraMoveSpeed), cameraNewPosition.z);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // store the moving direction
            moveDir = context.ReadValue<Vector2>();
        }

        public void IncreaseCameraSize()
        {
            if (cam.orthographicSize == maxSize) return;
            previousSize = cam.orthographicSize;
            currentSize = cam.orthographicSize + cameraSizeChange;
        }
        public void HurtZoom()
        {
            StartCoroutine(HurtZoomAnimation());

        }
        private IEnumerator HurtZoomAnimation()
        {
            interruptAdjustment = true;
            float time = 2.0f;
            float normalSize = cam.orthographicSize;
            float newSize = normalSize - 1;
            cam.orthographicSize = Mathf.Lerp(normalSize, newSize, time);

            yield return new WaitForSecondsRealtime(0.25f);
            cam.orthographicSize = Mathf.Lerp(newSize, normalSize, time);
            interruptAdjustment = false;
        }
        public void OnAim(InputAction.CallbackContext context)
        {
            // store aim direction
            aimDir = context.ReadValue<Vector2>();
        }
    }
}
