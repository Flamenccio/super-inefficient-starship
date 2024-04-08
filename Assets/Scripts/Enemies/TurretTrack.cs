using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTrack : EnemyShootBase, IEnemy
{
    // stays in place and fires at player
    protected float faceAngle = 0.0f;
    protected float yDiff = 0.0f;
    protected float xDiff = 0.0f;
    public int Tier { get => tier; }
    protected override void Behavior()
    {
        if (fireTimer >= fireRate) // attack periodically
        {
            if (player == null) player = SearchPlayer();
            Attack();
        }
        if (player != null)
        {
            // face the player
            yDiff = player.transform.position.y - transform.position.y;
            xDiff = player.transform.position.x - transform.position.x;
            faceAngle = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, faceAngle, 0.08f)));
            base.Behavior();
        }
        else
        {
            player = SearchPlayer();
        }
    }
    protected virtual void Attack()
    {
        if (player == null) return;
        if (Vector2.Distance(player.transform.position, transform.position) < searchRadius)
        {
            Fire(player.transform.position);
        }
    }
}
