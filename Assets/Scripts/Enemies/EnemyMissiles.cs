using System.Collections;
using Flamenccio.Enemy;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Controls an enemy. Aims at player and periodically fires a barrage of missiles.
    /// </summary>
    public class EnemyMissiles : EnemyBehavior
    {
        /*
        private const float BURST_INTERVAL = 0.50f; // the amount of time between each shot in burst
        private const int BURST_AMOUNT = 3;

        protected override void Attack()
        {
            if (player == null) return;

            if (Vector2.Distance(player.position, transform.position) < searchRadius)
            {
                StartCoroutine(BurstAttack());
            }
        }

        private IEnumerator BurstAttack()
        {
            for (int i = 0; i < BURST_AMOUNT; i++)
            {
                Fire(player.position);
                yield return new WaitForSeconds(BURST_INTERVAL);
            }
        }
        */
    }
}