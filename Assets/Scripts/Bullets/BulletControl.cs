using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Attack
{
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
        [SerializeField] protected List<string> ignoredTags = new();
        [SerializeField] protected KnockbackMultipiers knockbackMultiplier;
        [SerializeField] protected int durability = 1;
        [SerializeField] protected bool rotationIsStatic = true;
        [SerializeField] protected bool canIgnoreStageEdge = false;
        [SerializeField] protected int playerDamage = 1; // default damage done to player
        [SerializeField] protected int objectDamage = 1; // default damage done to object
        protected CameraEffects cameraEff = CameraEffects.Instance;
        protected Vector2 origin = Vector2.zero;

        public int PlayerDamage { get => playerDamage; }
        public int ObjectDamage { get => objectDamage; }
        public int KnockbackMultiplier { get => (int)knockbackMultiplier; }
        public float Range { get => maxDistance; }
        public float Speed { get => moveSpeed; }
        public int Durability { get => durability; }

        [SerializeField] protected Rigidbody2D rb;

        protected void Awake()
        {
            cameraEff = CameraEffects.Instance;
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
            if (rotationIsStatic) rb.transform.rotation = Quaternion.identity;
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
            if ((collider.CompareTag("PrimaryWall") || collider.CompareTag("InvisibleWall")) && !canIgnoreStageEdge) durability = 0;
            else if (!collider.CompareTag("PrimaryWall") && !collider.CompareTag("InvisibleWall")) durability--;

            if (durability <= 0) Destroy(this.gameObject);
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
            return ignoredTags.Contains(compareTag);

            /*
            foreach (string tag in ignoredTags)
            {
                if (compareTag.Equals(tag)) return true;
            }
            return false;
            */
        }
    }
}
