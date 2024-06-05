using Flamenccio.Utility;
using UnityEngine;

namespace Flamenccio.Attack
{
    /// <summary>
    /// Controls a circular hitbox. It's size, lifetime, and affiliation can be edited. 
    /// <para>By default, its affiliation is neutral.</para>
    /// </summary>
    public class Hitbox : BulletControl
    {
        public enum HitboxAffiliation
        {
            Neutral,
            Player,
            Enemy,
        }

        [SerializeField] private HitboxAffiliation type = HitboxAffiliation.Neutral;
        [SerializeField] private CircleCollider2D circleCollider;
        private float lifetime = 2f / 60f; // how long the hitbox will last. 2/60 second is default.
        private float timer = 0f; // amount of time in seconds this hitbox has been active

        /// <summary>
        /// Edit the properties of this hitbox.
        /// </summary>
        /// <param name="lifetime">If <= 0, uses default lifetime (2/60th of a second).</param>
        /// <param name="radius">If <= 0, uses default radius (1 unit).</param>
        /// <param name="damage">If <= 0, uses default damage (1 damage).</param>
        /// <param name="attackType">The source of the hitbox.</param>
        /// <param name="knockback">Power of knockback.</param>
        public void EditProperties(float lifetime, float radius, int damage, HitboxAffiliation attackType, KnockbackPower knockback)
        {
            type = attackType;
            ChangeAttackType(attackType);
            this.knockbackMultiplier = knockback;
            timer = 0; // reset the timer just in case

            if (lifetime > 0)
            {
                this.lifetime = lifetime;
            }
            if (radius > 0)
            {
                circleCollider.radius = radius;
            }
            if (damage > 0)
            {
                this.playerDamage = damage;
            }

            Activate();
        }

        /// <summary>
        /// Edit the properties of this hitbox using default knockback power.
        /// </summary>
        /// <param name="lifetime">If <= 0, uses default lifetime (2/60th of a second).</param>
        /// <param name="radius">If <= 0, uses default radius (1 unit).</param>
        /// <param name="damage">If <= 0, uses default damage (1 damage).</param>
        /// <param name="attackType">The source of the hitbox.</param>
        public void EditProperties(float lifetime, float radius, int damage, HitboxAffiliation attackType)
        {
            EditProperties(lifetime, radius, damage, attackType, knockbackMultiplier);
        }

        protected override void Behavior()
        {
            if (!circleCollider.enabled) return;

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }

            timer += Time.deltaTime;
        }

        private void ChangeAttackType(HitboxAffiliation attackType)
        {
            switch (attackType)
            {
                case HitboxAffiliation.Neutral:
                    gameObject.tag = TagManager.GetTag(Tag.NeutralBullet);
                    gameObject.layer = LayerMask.NameToLayer("Default"); // TODO temporary layer.
                    break;

                case HitboxAffiliation.Enemy:
                    gameObject.tag = TagManager.GetTag(Tag.EnemyBullet);
                    gameObject.layer = LayerManager.GetLayer(Layer.Enemy);
                    break;

                case HitboxAffiliation.Player:
                    gameObject.tag = TagManager.GetTag(Tag.PlayerBullet);
                    gameObject.layer = LayerManager.GetLayer(Layer.PlayerBullet);
                    break;
            }
        }

        protected override void Collide(Collision2D collision)
        {
            // don't do anything on collisions
        }

        protected override void Trigger(Collider2D collider)
        {
            // don't do anything on triggers
        }

        public void Activate()
        {
            circleCollider.enabled = true;
        }
    }
}