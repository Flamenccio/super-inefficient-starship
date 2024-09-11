using UnityEngine;

namespace Flamenccio.Utility.SpriteEffects
{
    public class Pulse : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRen;
        [SerializeField] private float brightnessFrequency = 2f;
        [SerializeField] private float sizeFrequency = 2f;
        [SerializeField] private float maxOpacity = 1;
        [SerializeField] private float minOpacity = 0;
        [SerializeField] private float maxSize = 1.0f;
        [SerializeField] private float minSize = 0.25f;

        private float tBrightness;
        private float tSize;
        private Color originalColor;
        private Color white = Color.white;
        private Color colorAmplitude;
        private float brightnessAmplitude;
        private float sizeAmplitude;
        private float brightnessPeriod = 2 * Mathf.PI;
        private float sizePeriod = 2 * Mathf.PI;

        private void Start()
        {
            originalColor = spriteRen.color;
            colorAmplitude = new((white.r - originalColor.r) / 2f, (white.g - originalColor.g) / 2f, (white.b - originalColor.b) / 2f);

            brightnessPeriod = (2 * Mathf.PI) / brightnessFrequency;
            maxOpacity = Mathf.Clamp01(Mathf.Abs(maxOpacity));
            minOpacity = Mathf.Clamp01(Mathf.Abs(minOpacity));
            brightnessAmplitude = (maxOpacity - minOpacity) / 2;

            sizePeriod = (2 * Mathf.PI) / sizeFrequency;
            maxSize = Mathf.Abs(maxSize);
            minSize = Mathf.Abs(minSize);
            sizeAmplitude = (maxSize - minSize) / 2;
        }

        private void Update()
        {
            tBrightness += Time.deltaTime;
            tBrightness %= brightnessPeriod;
            tSize += Time.deltaTime;
            tSize %= sizePeriod;

            // modify scale
            transform.localScale = new
                ((sizeAmplitude * Mathf.Sin(sizeFrequency * tSize)) + (maxSize - sizeAmplitude),
                (sizeAmplitude * Mathf.Sin(sizeFrequency * tSize)) + (maxSize - sizeAmplitude));

            // modify alpha
            spriteRen.color = new
                (spriteRen.color.r,
                spriteRen.color.g,
                spriteRen.color.b,
                (brightnessAmplitude * Mathf.Cos(brightnessFrequency * tBrightness)) + (maxOpacity - brightnessAmplitude));

            // modify r, g, and b
            spriteRen.color = new
                ((colorAmplitude.r * Mathf.Cos(sizeFrequency * tSize)) + (white.r - colorAmplitude.r),
                (colorAmplitude.g * Mathf.Cos(sizeFrequency * tSize)) + (white.g - colorAmplitude.g),
                (colorAmplitude.b * Mathf.Cos(sizeFrequency * tSize)) + (white.b - colorAmplitude.b),
                spriteRen.color.a);
        }
    }
}