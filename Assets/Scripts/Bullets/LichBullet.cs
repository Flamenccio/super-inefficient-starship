using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichBullet : EnemyBulletNormal
{
    [SerializeField] private GameObject phantomBullet;
    private int direction = 1;
    private float spawnFrequency = 20f / 60f;
    private float spawnTimer = 0.0f;
    protected override void Behavior()
    {
        if (spawnTimer >= spawnFrequency)
        {
            AllAngle fireAngle = new AllAngle();
            fireAngle.Vector = Vector2.Perpendicular(rb.velocity * direction);
            spawnTimer = 0.0f;
            GameObject x = Instantiate(phantomBullet, transform.position, Quaternion.Euler(0f, 0f, fireAngle.Degree));
            direction = direction * -1;
        }
        spawnTimer += Time.deltaTime;
    }
}
