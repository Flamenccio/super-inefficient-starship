using Flamenccio.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// Controls visual camera effects (e.g. screen shakes, zoom-ins, zoom-outs, etc.).
    /// <para>Allows other classes to trigger these effects.</para>
    /// </summary>
    public class CameraEffects : MonoBehaviour
    {
        public static CameraEffects Instance { get; private set; }
        private float screenShakeTime = 0f; // when this is above 0f, screenshake occurs.
        private float screenShakeMagnitude = 0f;
        private float screenShakeMagnitudeDecay = 0f;
        private float zoomTimer = 0f;
        private float zoomMagnitude = 0f;
        private float originalSize = 0f;
        private float zoomMaxTime = 0f; // how long it takes for the camera to reach final size
        private float zoomFinalTime = 0f; // how long the camera stays at final size before returning to original size

        private Action realtimeEffects;
        private Camera thisCamera;
        private CameraControl cameraControl;

        private const float SCREEN_SHAKE_DURATION = 0.25f; // duration of screen shake effect in seconds
        private const float SCREEN_SHAKE_WEAK = 0.20f;
        private const float SCREEN_SHAKE_NORMAL = 0.40f;
        private const float SCREEN_SHAKE_STRONG = 0.75f;
        private const float SCREEN_SHAKE_EXTREME = 1.0f;
        private const float MAX_SHAKE_DISTANCE = 40.0f;
        private const float MAX_SLOWMO_TIME_SECONDS = 10.0f;
        private const float MIN_SLOWMO_TIME_SECONDS = 0.1f;

        private readonly float[] screenShakeIntesities =
        {
            SCREEN_SHAKE_WEAK,
            SCREEN_SHAKE_NORMAL,
            SCREEN_SHAKE_STRONG,
            SCREEN_SHAKE_EXTREME,
        };

        public enum ScreenShakeIntensity
        {
            Weak,
            Normal,
            Strong,
            Extreme
        };

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

            thisCamera = Camera.main;
        }

        private void Start()
        {
            cameraControl = GetComponent<CameraControl>();
            GameEventManager.OnEnemyHit += (v) => ScreenShake(ScreenShakeIntensity.Weak, v.EventOrigin);
            GameEventManager.OnEnemyKill += (v) => ScreenShake(ScreenShakeIntensity.Normal, v.EventOrigin);
            GameEventManager.OnPlayerHit += (v) => Zoom(0.1f, 0.1f, -1);
        }

        private void Update()
        {
            realtimeEffects?.Invoke();
        }

        /// <summary>
        /// Cause the screen to shake for a fixed amount of time. If the source is too far away, then no screen shake will occur.
        /// </summary>
        /// <param name="intensity">How intense the screen shake should be.</param>
        /// <param name="source">Where the source of the impact is that caused the screen shake.</param>
        public void ScreenShake(CameraEffects.ScreenShakeIntensity intensity, Vector2 source)
        {
            realtimeEffects -= ScreenShakeUpdate;
            float distance = Mathf.Abs(Vector2.Distance(source, PlayerMotion.Instance.transform.position));
            screenShakeMagnitude = screenShakeIntesities[(int)intensity];
            float scaledMagnitude = screenShakeMagnitude - (screenShakeMagnitude * Mathf.Pow(distance / MAX_SHAKE_DISTANCE, 3f));
            screenShakeMagnitude = Mathf.Clamp(scaledMagnitude, 0f, screenShakeMagnitude);
            screenShakeMagnitudeDecay = screenShakeMagnitude / SCREEN_SHAKE_DURATION;
            screenShakeTime = SCREEN_SHAKE_DURATION;
            realtimeEffects += ScreenShakeUpdate;
        }

        private void ScreenShakeUpdate()
        {
            if (screenShakeTime > 0f)
            {
                Vector3 screenShakeOffset = (Vector3)UnityEngine.Random.insideUnitCircle * screenShakeMagnitude;
                transform.position += screenShakeOffset;
                screenShakeMagnitude -= screenShakeMagnitudeDecay * Time.deltaTime;
                Mathf.Clamp(screenShakeMagnitude, 0f, screenShakeMagnitude);
                screenShakeTime -= Time.unscaledDeltaTime;
            }
            else
            {
                screenShakeTime = 0f;
                screenShakeMagnitude = 0f;
                screenShakeMagnitudeDecay = 0f;
                realtimeEffects -= ScreenShakeUpdate;
            }
        }

        public void CutToPosition(Vector3 position)
        {
            Camera.main.transform.position = position;
        }

        /// <summary>
        /// Slows down game time for specified time.
        /// </summary>
        /// <param name="durationSeconds">How long to slow down game.</param>
        public void SlowMo(float durationSeconds)
        {
            StartCoroutine(SlowMoCoroutine(durationSeconds));
        }

        /// <summary>
        /// Temporarily zoom in.
        /// </summary>
        /// <param name="zoomInSeconds">How long it takes for the zoom in to reach final size.</param>
        /// <param name="durationSeconds">How long to zoom in seconds.</param>
        /// <param name="magnitude">How much to zoom. A <b>positive</b> value will <b>zoom out</b>. A <b>negative</b> value will <b>zoom in</b></param>.
        public void Zoom(float zoomInSeconds, float durationSeconds, float magnitude)
        {
            if (zoomTimer > 0f || !cameraControl.StopSizeUpdate(zoomInSeconds + durationSeconds)) return; // only one zoom in at a time and must be able to change camera's size

            zoomMaxTime = zoomInSeconds;
            zoomFinalTime = durationSeconds;
            zoomTimer = zoomMaxTime;
            zoomMagnitude = magnitude;
            originalSize = thisCamera.orthographicSize;
            realtimeEffects += ZoomUpdateStage1;
        }

        /// <summary>
        /// Zoom to final size over time defined by zoomMaxTime.
        /// </summary>
        private void ZoomUpdateStage1()
        {
            if (zoomTimer > 0f)
            {
                float t = zoomTimer / zoomMaxTime;
                thisCamera.orthographicSize = Mathf.SmoothStep(originalSize + zoomMagnitude, originalSize, t);
                zoomTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                zoomTimer = zoomFinalTime;
                realtimeEffects -= ZoomUpdateStage1;
                realtimeEffects += ZoomUpdateStage2;
            }
        }

        /// <summary>
        /// Stay at final size for time defined by zoomFinalTime.
        /// </summary>
        private void ZoomUpdateStage2()
        {
            if (zoomTimer > 0f)
            {
                zoomTimer -= Time.unscaledDeltaTime;
                thisCamera.orthographicSize = originalSize + zoomMagnitude;
            }
            else
            {
                zoomTimer = 0f;
                zoomMagnitude = 0f;
                zoomMaxTime = 0f;
                cameraControl.SetCameraSize(originalSize);
                realtimeEffects -= ZoomUpdateStage2;
            }
        }

        private IEnumerator SlowMoCoroutine(float durationSeconds)
        {
            durationSeconds = Mathf.Clamp(durationSeconds, MIN_SLOWMO_TIME_SECONDS, MAX_SLOWMO_TIME_SECONDS);
            Time.timeScale = 0.25f;
            yield return new WaitForSecondsRealtime(durationSeconds);
            Time.timeScale = 1.0f;
        }
    }
}