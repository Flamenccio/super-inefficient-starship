using System.Collections;
using UnityEngine;
using Flamenccio.Item;
using Flamenccio.Attack;
using Flamenccio.Effects.Audio;
using Flamenccio.Effects;

namespace Flamenccio.Core.Player
{
    public class PlayerCollisions : MonoBehaviour
    {
        [SerializeField] private GameState gState;
        [SerializeField] private GameObject hitEffect;
        private PlayerActions pActions;
        private const float HURT_INVULN_DURATION = 10f / 60f;
        private bool invulnerable = false;

        private void Awake()
        {
            pActions = gameObject.GetComponent<PlayerActions>();
        }
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Star"))
            {
                // add point
                gState.ReplenishTimer();
                gState.CollectStar(collision.GetComponent<Star>().Value);
            }
            if (collision.CompareTag("MiniStar"))
            {
                gState.ReplenishTimer(0.5f);
                gState.CollectMiniStar(collision.GetComponent<MiniStar>().Value);
            }
            if ((collision.CompareTag("EBullet") || collision.CompareTag("NBullet")) && !invulnerable)
            {
                BulletControl bullet = collision.GetComponent<BulletControl>();
                StartCoroutine(HurtInvuln());
                gState.RemoveLife(bullet.Damage);
                Vector2 knockbackVector = CalculateKnockbackAngle(pActions.Rigidbody.velocity, collision.attachedRigidbody.velocity);
                PlayerMotion.Instance.Knockback(knockbackVector, bullet.KnockbackMultiplier);
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.playerHurt, transform.position);
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            if (collision.CompareTag("Heart"))
            {
                gState.ReplenishLife(1);
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
