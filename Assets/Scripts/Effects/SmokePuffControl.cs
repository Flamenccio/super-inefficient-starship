using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// Controls the behavior of smoke particles.
    /// </summary>
    public class SmokePuffControl : Trail
    {
        private Rigidbody2D rb;
        private const float ORIGINAL_SCALE = 1f;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            transform.localScale = new(ORIGINAL_SCALE, ORIGINAL_SCALE);
            spriteren.color = new(spriteren.color.r, spriteren.color.g, spriteren.color.b, 0.5f);
        }

        public void Launch(Vector2 velocity)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(velocity, ForceMode2D.Impulse);
        }

        protected override void Behavior()
        {
            // slowly grow in size
            var t = 0.75f / (10f * lifeTimer + 1);
            transform.localScale += new Vector3(t, t);
        }
    }
}
