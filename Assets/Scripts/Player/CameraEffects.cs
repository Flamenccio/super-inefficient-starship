using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects instance;

    private const float SCREEN_SHAKE_DURATION = 0.25f; // duration of screen shake effect in seconds
    private const float SCREEN_SHAKE_WEAK = 0.20f;
    private const float SCREEN_SHAKE_NORMAL = 0.40f;
    private const float SCREEN_SHAKE_STRONG = 0.75f;
    private const float SCREEN_SHAKE_EXTREME = 1.0f;
    private const float MAX_SHAKE_DISTANCE = 40.0f;

    private float screenShakeTime = 0f; // when this is above 0f, screenshake occurs.
    private float screenShakeMagnitude = 0f;
    private float screenShakeMagnitudeDecay = 0f;
    private Vector3 offset = new Vector3(0f, 0f, -10f); // default z axis offset
    private Vector3 screenShakeOffset; // the vector3 offset added on to the camera's global position to create the screenshake
    private float[] screenShakeIntesities =
    {
        SCREEN_SHAKE_WEAK,
        SCREEN_SHAKE_NORMAL,
        SCREEN_SHAKE_STRONG,
        SCREEN_SHAKE_EXTREME,
    };

    [SerializeField] private Transform playerTransform;
    public enum ScreenShakeIntensity
    {
        Weak,
        Normal,
        Strong,
        Extreme
    };
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (screenShakeTime > 0f)
        {
            screenShakeOffset = (Vector3)Random.insideUnitCircle * screenShakeMagnitude;
            transform.position += screenShakeOffset;
            screenShakeMagnitude -= screenShakeMagnitudeDecay * Time.deltaTime;
            Mathf.Clamp(screenShakeMagnitude, 0f, screenShakeMagnitude);
            screenShakeTime -= Time.deltaTime;
        }
        else
        {
            screenShakeTime = 0f;
        }
    }
    public void ScreenShake(CameraEffects.ScreenShakeIntensity intensity, Vector2 source)
    {
        float distance = Mathf.Abs(Vector2.Distance(source, playerTransform.position));
        screenShakeMagnitude = screenShakeIntesities[(int)intensity];
        float scaledMagnitude = screenShakeMagnitude - screenShakeMagnitude * Mathf.Pow((distance / MAX_SHAKE_DISTANCE), 3f);
        screenShakeMagnitude = Mathf.Clamp(scaledMagnitude, 0f, screenShakeMagnitude);
        screenShakeMagnitudeDecay = screenShakeMagnitude / SCREEN_SHAKE_DURATION;
        screenShakeTime = SCREEN_SHAKE_DURATION;
    }
}
