using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    public class Afterimage : MonoBehaviour
    {
        [SerializeField] private float LIFETIME = 1.0f;
        private float lifeTimer = 0f;
        [SerializeField] private SpriteRenderer spriteRen;

        private void FixedUpdate()
        {
            if (lifeTimer >= LIFETIME)
            {
                Destroy(gameObject);
            }
            lifeTimer += Time.deltaTime;
            spriteRen.color = new Color(spriteRen.color.r, spriteRen.color.g, spriteRen.color.b, spriteRen.color.a - 0.1f);
        }
    }
}
