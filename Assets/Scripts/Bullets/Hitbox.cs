using UnityEngine;

namespace Flamenccio.Attack
{
    public class Hitbox : BulletControl
    {
        // this is basically a generic circular static hitbox.
        // useful attacks that need a instantaneous hitbox (i.e., explosions) 
        // default attack type is neutral
        public enum AttackType
        {
            Neutral,
            Player,
            Enemy,
        }
        [SerializeField] private AttackType type = AttackType.Neutral;
        [SerializeField] private CircleCollider2D circleCollider;
        private float lifetime = 2f / 60f; // how long the hitbox will last. 2/60 second is default.
        private float timer = 0f; // amount of time in seconds this hitbox has been active

        /// <summary>
        /// <para>If lifetime is less than or equal to 0, default lifetime is kept.</para>
        /// <para>If radius is less than or equal to 0, default radius is kept.</para>
        /// <para>If damage is less than or equal to 0, default damage is kept.</para>
        /// </summary>
        public void EditProperties(float lifetime, float radius, int damage, AttackType attackType, KnockbackMultipiers knockback)
        {
            type = attackType;
            ChangeAttackType(attackType);
            this.knockbackMultiplier = knockback;

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
                this.damage = damage;
            }

            timer = 0; // reset the timer just in case
            Activate();
        }
        public void EditProperties(float lifetime, float radius, int damage, AttackType attackType)
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
        private void ChangeAttackType(AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.Neutral:
                    gameObject.tag = "NBullet";
                    gameObject.layer = LayerMask.NameToLayer("Default"); // TODO temporary
                    break;
                case AttackType.Enemy:
                    gameObject.tag = "EBullet";
                    gameObject.layer = LayerMask.NameToLayer("Enemies");
                    break;
                case AttackType.Player:
                    gameObject.tag = "PBullet";
                    gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
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
