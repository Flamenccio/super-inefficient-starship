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
        [SerializeField] protected int hp = 1;
        [SerializeField] protected bool rotationIsStatic = true;
        [SerializeField] protected bool canIgnoreStageEdge = false;
        [SerializeField] protected int damage = 1; // default damage
        protected CameraEffects cameraEff = CameraEffects.Instance;
        protected Vector2 origin = Vector2.zero;

        public int Damage { get => damage; }
        public int KnockbackMultiplier { get => (int)knockbackMultiplier; }
        public float Range { get => maxDistance; }
        public float Speed { get => moveSpeed; }
        public int HP { get => hp; }

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
        protected virtual void Trigger(Collider2D collider) // TODO this should go into PlayerBullet
        {
            if (collider.CompareTag("PrimaryWall") && !canIgnoreStageEdge) hp = 0;
            else if (!collider.CompareTag("PrimaryWall")) hp--;

            if (hp <= 0) Destroy(this.gameObject);
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
}
