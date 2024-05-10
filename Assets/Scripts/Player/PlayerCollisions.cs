using System.Collections;
using UnityEngine;
using Flamenccio.Item;
using Flamenccio.Attack;
using Flamenccio.Effects;
using Flamenccio.LevelObject;

namespace Flamenccio.Core.Player
{
    public class PlayerCollisions : MonoBehaviour
    {
        private const float HURT_INVULN_DURATION = 10f / 60f;
        private bool invulnerable = false;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Star"))
            {
                GameEventManager.OnStarCollect(GameEventManager.CreateGameEvent(collision.GetComponent<Star>().Value, collision.transform.position));
                return;
            }
            if (collision.CompareTag("MiniStar"))
            {
                GameEventManager.OnMiniStarCollect(GameEventManager.CreateGameEvent(collision.GetComponent<MiniStar>().Value, collision.transform.position));
                return;
            }
            if ((collision.CompareTag("EBullet") || collision.CompareTag("NBullet")) && !invulnerable)
            {
                StartCoroutine(HurtInvuln());
                BulletControl bullet = collision.GetComponent<BulletControl>();
                Vector2 knockbackVector = CalculateKnockbackAngle(PlayerMotion.Instance.PlayerVelocity, collision.attachedRigidbody.velocity);
                PlayerMotion.Instance.Knockback(knockbackVector, bullet.KnockbackMultiplier);
                GameEventManager.OnPlayerHit(GameEventManager.CreateGameEvent(bullet.Damage, transform));
                return;
            }
            if (collision.CompareTag("Heart"))
            {
                GameEventManager.OnHeartCollect(GameEventManager.CreateGameEvent(1f, transform));
                return;
            }
        }
        public void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Portal"))
            {
                Portal p = collision.gameObject.GetComponent<Portal>();
                Transform d = p.GetDestination();

                if (d == null) return;

                if (!p.TriggerCooldown()) return;

                PlayerMotion.Instance.TeleportTo(d.position);
            }
        }
        private IEnumerator HurtInvuln()
        {
            invulnerable = true;
            yield return new WaitForSeconds(HURT_INVULN_DURATION);
            invulnerable = false;
        }
        /// <summary>
        /// <para>Calculates the knockback that the player takes after being damaged.</para>
        /// <para>Note that only the direction of the velocities are used.</para>
        /// <para>Use the <b>multiplier</b> value to affect the velocity of the knockback.</para>
        /// <para>If the <b>multiplier</b> is negative, the knockback will be turned into a "draw-in" effect.</para>
        /// </summary>
        /// <param name="playerVelocity">the player's current velocity</param>
        /// <param name="attackerVelocity">the damaging object's velocity</param>
        /// <param name="multiplier">how much the player will be knocked back</param>
        /// <returns>the resulting vector</returns>
        private Vector2 CalculateKnockback(Vector2 playerVelocity, Vector2 attackerVelocity, float multiplier)
        {
            if (multiplier == 0) return new Vector2(0, 0);

            Vector2 pVelocityInversion = new(-playerVelocity.x, -playerVelocity.y);
            Vector2 knockback = pVelocityInversion + attackerVelocity;
            knockback.Normalize();

            return knockback * multiplier;
        }
        private Vector2 CalculateKnockbackAngle(Vector2 playerVelocity, Vector2 attackVelocity)
        {
            return CalculateKnockback(playerVelocity, attackVelocity, 1f);
        }
    }
}
