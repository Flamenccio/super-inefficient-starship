using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Item
{
    public class Heart : Item
    {
        protected override void TriggerEffect(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.heartCollect, transform.position);
            }
            base.TriggerEffect(collider);
        }
    }
}
