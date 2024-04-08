using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BounceBullet : EnemyBulletNormal 
{
    private int bounces = 0;
    private RaycastHit2D cast;
    [SerializeField] private int maxBounces = 10;
    [SerializeField] private LayerMask wallsLayer;

    protected override void Behavior()
    {
        //RaycastHit2D cast = Physics2D.CircleCast(transform.position, 0.33f, rb.velocity, 0.33f, wallsLayer);

        if (bounces >= maxBounces)
        {
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    protected override void Trigger(Collider2D collider)
    {
        cast = Physics2D.CircleCast(transform.position, 0.33f, Vector2.MoveTowards(transform.position, collider.transform.position, 0.50f), 0.50f, wallsLayer);
        rb.velocity = Vector2.Reflect(rb.velocity, cast.normal);
        bounces++;
    }
    protected override void Collide(Collision2D collision)
    {
        // do nothing
    }

}
