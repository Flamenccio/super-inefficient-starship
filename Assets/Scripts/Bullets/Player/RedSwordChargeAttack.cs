using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Flamenccio.Attack.Player
{
    /// <summary>
    /// Controls a player bullet. Moves slowly and continuously deals damage to obstacles and enemies in its path.
    /// </summary>
    public class RedSwordChargeAttack : PlayerBullet
    {
        [SerializeField] private CircleCollider2D slashHitbox;
        private float attackTimer = 0f;
        private const float ATTACK_FREQUENCY = 15f / 60f;

        protected override void Behavior()
        {
            if (attackTimer >= ATTACK_FREQUENCY)
            {
                StartCoroutine(Slash());
                attackTimer = 0f;
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        private IEnumerator Slash()
        {
            slashHitbox.enabled = true;
            yield return new WaitForNextFrameUnit();
            slashHitbox.enabled = false;
        }
    }
}