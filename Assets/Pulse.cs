using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRen;
    private float t;
    private const float FREQUENCY = 2f;
    private const float AMPLITUDE = 0.2f;
    private const float MIDDLE = 1;
    private const float PERIOD = (2 * Mathf.PI) / FREQUENCY;
    private const float MAX_OPACITY = 0.50f;

    private void Update()
    {
        t += Time.deltaTime;
        t %= PERIOD;
        transform.localScale = new Vector2((AMPLITUDE * Mathf.Sin(FREQUENCY * t)) + MIDDLE, (AMPLITUDE * Mathf.Sin(FREQUENCY * t)) + MIDDLE);
        spriteRen.color = new Color(spriteRen.color.r, spriteRen.color.g, spriteRen.color.b, (AMPLITUDE * Mathf.Cos(FREQUENCY * t)) + (MAX_OPACITY - AMPLITUDE));
    }
}
