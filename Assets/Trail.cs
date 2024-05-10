using UnityEngine;
using UnityEngine.Pool;

namespace Flamenccio.Effects.Visual
{
    public class Trail : MonoBehaviour, ITrailPool
    {
        public ObjectPool<Trail> Pool { get; set; }
        [SerializeField] protected float maxLifetime = 1.0f;
        [SerializeField] protected SpriteRenderer spriteren;
        protected float lifeTimer = 0f;
        protected float fadeoutSpeed = 1f;

        protected void Awake()
        {
            float frames = (60f * maxLifetime);
            fadeoutSpeed = 1f / frames;
        }
        protected void FixedUpdate()
        {
            if (lifeTimer >= maxLifetime)
            {
                lifeTimer = 0f;
                spriteren.color = Color.white;
                Pool.Release(this);
            }
            else
            {
                lifeTimer += Time.deltaTime;
                spriteren.color = new Color(spriteren.color.r, spriteren.color.g, spriteren.color.b, spriteren.color.a - fadeoutSpeed);
            }
        }

    }
}
