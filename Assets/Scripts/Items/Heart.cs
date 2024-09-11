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
                AudioManager.Instance.PlayOneShot(collectSfx, transform.position);
            }

            base.TriggerEffect(collider);
        }
    }
}
