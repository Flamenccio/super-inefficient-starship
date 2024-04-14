using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public enum KnockbackMultipiers
    {
        None = 0,
        Low = 10,
        Average = 15,
        High = 20,
        Extreme = 25
    }
    // default move speed
    [SerializeField] protected float moveSpeed = 20.0f;
    [SerializeField] protected float maxDistance = 1f;
    [SerializeField] protected GameObject parryEffect;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected List<string> ignoredTags = new();
    [SerializeField] protected KnockbackMultipiers knockbackMultiplier;
    protected CameraEffects cameraEff = CameraEffects.instance;
    protected Vector2 origin = Vector2.zero;


    // default damage
    [SerializeField] protected int damage = 1;
    public int Damage { get => damage; }
    public int KnockbackMultiplier { get => (int)knockbackMultiplier; }
    public float Range { get => maxDistance; }
    public float Speed { get =>  moveSpeed; }

    [SerializeField] protected Rigidbody2D rb;

    protected void Awake()
    {
        origin = transform.position;
        Startup();
        Launch();
    }
    protected virtual void Startup()
    {
        // additional startup behaviors
    }
    protected virtual void Launch()
    {
        if (rb.bodyType != RigidbodyType2D.Static)
        {
            rb.velocity = transform.right * moveSpeed;
        }
        rb.transform.rotation = Quaternion.identity;
    }

    protected void FixedUpdate()
    {
        Behavior();
        DeathTimer();
    }
    protected virtual void DeathTimer()
    {
        if (Vector2.Distance(transform.position, origin) >= maxDistance)
        {
            Destroy(this.gameObject);
        }
    }
    protected virtual void Behavior()
    {

    }
    protected virtual void Collide(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
    protected virtual void Trigger(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("EBullet"))
        {
            Instantiate(parryEffect, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IgnoreTags(collision.gameObject.tag))
        {
            Collide(collision);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IgnoreTags(collision.gameObject.tag))
        {
            Trigger(collision);
        }
    }
    private bool IgnoreTags(string compareTag)
    {
        foreach (string tag in ignoredTags)
        {
            if (compareTag.Equals(tag)) return true;
        }
        return false;
    }
}
