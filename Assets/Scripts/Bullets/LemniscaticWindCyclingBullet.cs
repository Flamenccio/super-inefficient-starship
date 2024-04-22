using UnityEngine;

namespace Flamenccio.Attack
{
    public class LemniscaticWindCyclingBullet : PlayerBullet
    {
        private float timer = 0f;
        private float maxTimer = 0.10f;
        public int EnemiesHit { get; private set; }

        protected override void DeathTimer()
        {
            timer += Time.deltaTime;
            if (timer >= maxTimer)
            {
                Destroy(gameObject);
            }
        }
        protected override void Launch()
        {
        }
        protected override void Trigger(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("EBullet"))
            {
                Instantiate(parryEffect, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
            if (collider.gameObject.CompareTag("Enemy"))
            {
                EnemiesHit++;
            }
        }
    }
}
