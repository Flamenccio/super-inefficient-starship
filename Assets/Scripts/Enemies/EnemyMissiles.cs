using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyMissiles : TurretTrack, IEnemy
    {
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
    }
}
