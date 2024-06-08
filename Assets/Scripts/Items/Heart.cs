using UnityEngine;
using Flamenccio.Effects.Audio;

namespace Flamenccio.Item
{
    public class Heart : Item
    {
        protected override void TriggerEffect(Collider2D collider)
        {
            if (collider.CompareTag(PLAYER_TAG))
            {
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.heartCollect, transform.position);
            }

            base.TriggerEffect(collider);
        }
    }
}
