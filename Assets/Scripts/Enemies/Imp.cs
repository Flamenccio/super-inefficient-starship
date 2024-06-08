using UnityEngine;
using Flamenccio.Attack;
using Flamenccio.Effects.Visual;
using Flamenccio.Utility;

namespace Enemy
{
    /// <summary>
    /// Controls an enemy. Slowly follows player, ignoring collisions. "Bites" the player upon contact and dies.
    /// <para>When its spawner is destroyed, enters an enraged state where it moves and turns faster.</para>
    /// </summary>
    public class Imp : EnemyShootBase, IEnemy
    {
        public int Tier { get { return tier; } }

        [SerializeField] private GameObject hitboxPrefab;
        [SerializeField] private float hitRadius = 1.0f;

        private const float TURNING_SPEED = 0.01f;
        private float ENRAGED_SPEED;

        protected override void OnSpawn()
        {
            searchRadius = 100f; // huuuge search radius, basically inescapable (like responsibility)
            ENRAGED_SPEED = moveSpeed * 2;
        }

        protected override void Trigger(Collider2D col)
        {
            if (col.gameObject.CompareTag(TagManager.GetTag(Tag.Player)))
            {
                // destroy this gameObject, but don't drop stars
                EffectManager.Instance.SpawnEffect(EffectManager.Effects.EnemyKill, transform.position);
                GameObject ouch = Instantiate(hitboxPrefab, transform.position, Quaternion.identity);
                ouch.GetComponent<Hitbox>().EditProperties(0f, hitRadius, 0, Hitbox.HitboxAffiliation.Enemy);
                Destroy(gameObject); // destroy self
            }
        }

        protected override void Behavior()
        {
            float yDiff = player.position.y - transform.position.y;
            float xDiff = player.position.x - transform.position.x;
            float faceAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg, TURNING_SPEED);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, faceAngle));
            rb.velocity = new Vector2(transform.right.x * moveSpeed, transform.right.y * moveSpeed);
        }

        public void Enrage()
        {
            moveSpeed = ENRAGED_SPEED;
        }
    }
}