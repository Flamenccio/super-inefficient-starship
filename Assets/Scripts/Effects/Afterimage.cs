using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// A trail effect that appears on the player when they use a dash.
    /// </summary>
    public class Afterimage : MonoBehaviour
    {
        [SerializeField] private float LIFETIME = 1.0f;
        [SerializeField] private SpriteRenderer spriteRen;
        private float lifeTimer = 0f;

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