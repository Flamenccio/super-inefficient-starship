using TMPro;
using UnityEngine;

namespace Flamenccio.HUD
{
    public class FlyingTextScript : MonoBehaviour
    {
        // lifetime in seconds
        private const float LIFETIME = 1.0f;
        private const float FADEOUT_THRESHOLD = LIFETIME / 2f;
        private const float FADEOUT_SPEED = 1f / (LIFETIME - FADEOUT_THRESHOLD);
        private float age = 0f;
        private float flyingSpeed = 0.0f;
        private TMP_Text text;
        private void Start()
        {
        }
        private void Awake()
        {
            text = gameObject.GetComponent<TMP_Text>();
            text.transform.localScale = new Vector3(5f, 5f);

        }
        private void Update()
        {
            if (text.transform.localScale.x > 1f)
            {
                text.transform.localScale -= new Vector3(0.5f, 0.5f);
            }
            float alpha = text.color.a;
            age += Time.deltaTime;
            transform.Translate(transform.up * flyingSpeed);

            if (age >= FADEOUT_THRESHOLD)
            {
                alpha -= FADEOUT_SPEED * Time.deltaTime;
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            }
            if (age >= LIFETIME) Destroy(gameObject);
        }
    }
}
