using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public enum RotateDirection
    {
        Clockwise = -1,
        ClounterClockwise = 1
    }
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private RotateDirection direction = RotateDirection.Clockwise;

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, speed * (int)direction));
    }

}
