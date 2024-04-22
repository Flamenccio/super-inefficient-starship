using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Item
{
    public class Star : Item
    {
        [SerializeField] protected int val;
        public int Value { get => val; }
        protected override void CollectEffect(Transform player)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.starCollect, transform.position);
        }
    }
}
