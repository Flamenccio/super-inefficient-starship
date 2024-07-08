using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;

namespace Flamenccio.Attack
{
    /// <summary>
    /// Base class for all attacks.
    /// </summary>
    public class BulletControl : MonoBehaviour
    {
        public enum KnockbackPower
        {
            None = 0,
            Low = 10,
            Average = 15,
            High = 20,
            Extreme = 25
        }
        public int PlayerDamage { get => playerDamage; }
        public int ObjectDamage { get => objectDamage; }
        public int KnockbackMultiplier { get => (int)knockbackMultiplier; }
        public float Range { get => maxDistance; }
        public float Speed { get => moveSpeed; }
        public int Durability { get => durability; }


        [SerializeField] protected float moveSpeed = 20.0f; // default move speed
        [SerializeField] protected float maxDistance = 1f;
        protected List<string> collisionTags = new(); // Any GameObjects with these tags will trigger this bullet.
        [SerializeField] protected KnockbackPower knockbackMultiplier;
        [SerializeField] protected int durability = 1; // number of entities bullet can touch before destroying
        [SerializeField] protected bool rotationIsStatic = true;
        [SerializeField] protected bool canIgnoreStageEdge = false;
        [SerializeField] protected int playerDamage = 1; // default damage done to player
        [SerializeField] protected int objectDamage = 1; // default damage done to object
        [SerializeField] protected Rigidbody2D rb;

        protected CameraEffects cameraEff = CameraEffects.Instance;
        protected Vector2 origin = Vector2.zero;

        protected void Awake()
        {
            cameraEff = CameraEffects.Instance;
            origin = transform.position;
            Startup();
            Launch();
        }

        /// <summary>
        /// Additional behaviors that is called on Awake().
        /// </summary>
        protected virtual void Startup() { }

        /// <summary>
        /// Called right after Startup().
        /// </summary>
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

        /// <summary>
        /// Run on FixedUpdate().
        /// </summary>
        protected virtual void DeathTimer()
        {
            if (Vector2.Distance(transform.position, origin) >= maxDistance)
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// How the bullet will behave. Runs on FixedUpdate().
        /// </summary>
        protected virtual void Behavior() { }

        /// <summary>
        /// Behavior that happens when this bullet collides with a game object whose tag isn't ignored.
        /// </summary>
        protected virtual void Collide(Collision2D collision)
        {
            Death();
        }

        /// <summary>
        /// Behavior that happens when this bullet enters a trigger with a game object whose tag isn't ignored.
        /// </summary>
        protected virtual void Trigger(Collider2D collider)
        {
            string primaryWall = TagManager.GetTag(Tag.PrimaryWall);
            string invisibleWall = TagManager.GetTag(Tag.InvisibleWall);

            if ((collider.CompareTag(primaryWall) || collider.CompareTag(invisibleWall)) && !canIgnoreStageEdge) durability = 0;
            else if (!collider.CompareTag(primaryWall) && !collider.CompareTag(invisibleWall)) durability--;

            if (durability <= 0) Death();
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsCollisionTag(collision.gameObject.tag))
            {
                Collide(collision);
            }
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsCollisionTag(collision.gameObject.tag))
            {
                Trigger(collision);
            }
        }

        private bool IsCollisionTag(string compareTag)
        {
            return collisionTags.Contains(compareTag);
        }

        /// <summary>
        /// What happens when this bullet's durability reaches 0?
        /// </summary>
        protected virtual void Death()
        {
            Destroy(gameObject);
        }
    }
}
