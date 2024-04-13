using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Allows outside classes to temporarily affect player movement and physics.
/// </summary>
public class PlayerMotion : MonoBehaviour
{
    private const float KNOCKBACK_DURATION = 6f / 60f;
    [SerializeField] private Rigidbody2D rb;
    public bool MovementRestricted { get; private set; }
    public bool ActionRestricted { get; private set; }
    public static PlayerMotion Instance { get; private set; }
    public Vector2 PlayerPosition { get => transform.position; }
    private void Start()
    {
        Instance = this;
        MovementRestricted = false;
        ActionRestricted = false;
    }
    /// <summary>
    /// Instantly and instantaneously moves player to given global coordinate.
    /// </summary>
    /// <param name="coord">Global coordinate to move to.</param>
    public void TeleportTo(Vector2 coord)
    {
        transform.position = coord;
    }
    /// <summary>
    /// Instantly and instantaneously moves player to position relative to some transform.
    /// </summary>
    /// <param name="offset">Offset from transform's position.</param>
    /// <param name="origin">Transform to teleport relative to.</param>
    public void TeleportRelativeTo(Vector2 offset, Transform origin)
    {
        TeleportTo(offset + (Vector2)origin.position);
    }
    /// <summary>
    /// Prevents player from moving via input for some given amount of time. <para>Note that only <b>one</b> restriction may exist at any time; repeatedly restricting the player character will not accumulate restriction time.</para>
    /// </summary>
    /// <param name="t">Time in seconds.</param>
    public bool RestrictMovement(float t)
    {
        // TODO this will cause problems if two actions need to edit the same stat--please fix!
        if (MovementRestricted) return false;
        StartCoroutine(RestrictMovementCoroutine(t));
        return true;
    }
    private IEnumerator RestrictMovementCoroutine(float t)
    {
        MovementRestricted = true;
        yield return new WaitForSeconds(t);
        MovementRestricted = false;
    }
    /// <summary>
    /// Prevents player from taking any actions for some given amount of time.
    /// </summary>
    /// <param name="t">Time in seconds.</param>
    public void RestrictAction(float t)
    {
        if (ActionRestricted) return;
        StartCoroutine(RestrictActionCoroutine(t));
    }
    private IEnumerator RestrictActionCoroutine(float t)
    {
        ActionRestricted = true;
        yield return new WaitForSeconds(t);
        ActionRestricted = false;
    }
    /// <summary>
    /// Exert an impulse on the player character.
    /// </summary>
    public void Shove(Vector2 direction, float magnitude)
    {
        direction.Normalize();
        rb.AddForce(direction * magnitude, ForceMode2D.Impulse);
    }
    /// <summary>
    /// Exert a force on the player character.
    /// </summary>
    public void Push(Vector2 direction, float magnitude)
    {
        direction.Normalize();
        rb.AddForce(direction * magnitude, ForceMode2D.Force);
    }
    /// <summary>
    /// Linearly move the player in specified direction, speed, and time.
    /// </summary>
    public void Move(Vector2 direction, float speed, float t)
    {
        direction.Normalize();
        RestrictMovement(t);
        rb.velocity = direction * speed;
    }
    /// <summary>
    /// Shorthand for moving player character with fixed duration.
    /// </summary>
    public void Knockback(Vector2 direction, float power)
    {
        Move(direction, power, KNOCKBACK_DURATION);
    }
}
