using Flamenccio.Effects.Audio;
using Flamenccio.Effects.Visual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Components
{
    /// <summary>
    /// Class that allows visual and audio effects to play at attached GameObject's position
    /// </summary>
    public class EntityEffect : MonoBehaviour
    {
        /// <summary>
        /// Play a sound effect at the current position
        /// </summary>
        /// <param name="soundName">Name of sound effect</param>
        public void PlaySound(string soundName)
        {
            AudioManager.Instance.PlayOneShot(soundName, transform.position);
        }

        /// <summary>
        /// Spawn a particle effect at the current position
        /// </summary>
        /// <param name="particleName">Name of the particle effect</param>
        public void PlayVisualParticle(string particleName)
        {
            EffectManager.Instance.SpawnEffect(particleName, transform.position);
        }
    }
}
