using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeBullet : EnemyBulletNormal 
{
    [SerializeField] private GameObject littleGrape;
    private List<Vector2> burstDirections = new List<Vector2>()
    {
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(1, 1),
        new Vector2(-1, -1),
        new Vector2(1, -1),
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(0, -1)
    };
    protected override void Trigger(Collider2D collider)
    {
        AllAngle fireAngle = new AllAngle();
        foreach (Vector2 direction in burstDirections)
        {
            fireAngle.Vector = direction;
            Instantiate(littleGrape, transform.position, Quaternion.Euler(0f, 0f, fireAngle.Degree));
        }
        Destroy(this.gameObject);
    }
    protected override void Collide(Collision2D collision)
    {
        Trigger(collision.collider);
    }
}
