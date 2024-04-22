using UnityEngine;

namespace Enemy
{
    public class ImpSpawner : EnemyBase, IEnemy
    {
        public int Tier { get { return tier; } }
        [SerializeField] private GameObject impPrefab;
        private const int SPAWN_OFFSET = 5; // possible absolute value amount representing x and y offset imp spawn locations.
        private const float SPAWN_FREQUENCY = 3.0f; // the amount of time in seconds between each spawn tick.
        private float timer = 0f;
        private int impsSpawned = 0;
        private const int SPAWN_COUNT = 4; // the number of imps to spawn.
        private const int ANIM_FRAME_RATE = 12;
        private const int ANIM_TOTAL_FRAMES = 6;

        protected override void OnSpawn()
        {
            // calculate the animation speed to match the spawn time
            float baseAnimTime = (float)ANIM_TOTAL_FRAMES / (float)ANIM_FRAME_RATE;
            float animTimeScale = SPAWN_FREQUENCY / baseAnimTime;
            animator.speed = 1f / animTimeScale;
        }
        protected override void Behavior()
        {
            if (timer >= SPAWN_FREQUENCY)
            {
                impsSpawned += SPAWN_COUNT;
                loot = impsSpawned; // the loot will scale proportionally to the number of imps spawned.
                timer = 0f;

                for (int i = 0; i < SPAWN_COUNT; i++)
                {
                    float xOff = Random.Range(-SPAWN_OFFSET, SPAWN_OFFSET + 1);
                    float yOff = Random.Range(-SPAWN_OFFSET, SPAWN_OFFSET + 1);
                    Vector2 offset = new(xOff, yOff);
                    Instantiate(impPrefab, transform.position + (Vector3)offset, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))); // spawn in an imp at randomized offset and rotation.
                }
            }

            timer += Time.deltaTime;
        }
    }
}
