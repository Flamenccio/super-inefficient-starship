using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Controls an enemy. Slowly spins and continuously fires phantom bullets.
    /// </summary>
    public class TurretSpin : EnemyShootBase, IEnemy
    {
        public int Tier { get => tier; }
        [SerializeField] private float rotationSpeed = 1f;
        private int rotationDirection = 1;

        private readonly float[] angles = new float[4]
        {
            45f,
            135f,
            225f,
            315f
        };

        protected override void OnSpawn()
        {
            tier = 3;
            rotationDirection = Mathf.RoundToInt(Mathf.Pow(-1, Random.Range(0, 2)));
        }

        protected override void Behavior()
        {
            rb.transform.Rotate(new Vector3(0f, 0f, rotationSpeed * rotationDirection));

            if (fireTimer >= fireRate)
            {
                Attack();
            }

            base.Behavior();
        }

        protected void Attack()
        {
            foreach (float f in angles)
            {
                Fire(f + rb.transform.localRotation.eulerAngles.z);
            }
        }
    }
}