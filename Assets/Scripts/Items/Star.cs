using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Item
{
    public class Star : Item
    {
        public int Value { get => val; }
        [SerializeField] protected int val;

        protected override void CollectEffect(Transform player)
        {
            AudioManager.Instance.PlayOneShot(collectSfx, transform.position);
        }
    }
}
