using UnityEngine;
using UnityEngine.Pool;

namespace Flamenccio.Effects.Visual
{
    public class Trail : MonoBehaviour, ITrailPool
    {
        public ObjectPool<Trail> Pool { get; set; }
        [SerializeField] private float maxLifetime = 1.0f;
        [SerializeField] private SpriteRenderer spriteren;
        private float lifeTimer = 0f;

        private void FixedUpdate()
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
                spriteren.color = new Color(spriteren.color.r, spriteren.color.g, spriteren.color.b, spriteren.color.a - 0.1f);
            }
        }

    }
}
