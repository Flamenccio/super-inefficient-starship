using UnityEngine;
using Flamenccio.Effects.Audio;
using UnityEditor;

namespace Flamenccio.Item
{
    public class Star : Item
    {
        public int Value { get => val; }
        [SerializeField] protected int val;

        protected override void CollectEffect(Transform player)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GetAudioEvent("ItemStarCollect"), transform.position);
        }
    }
}
