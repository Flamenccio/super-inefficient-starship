using UnityEngine;
using FMODUnity;
using Flamenccio.Core;

namespace Flamenccio.Effects.Audio
{
    /// <summary>
    /// Manages the playback of sound effects. 
    /// <para>Other classes can reference an instance of this class to play certain sound effects in conjunction with FMODEvents.</para>
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            GameEventManager.OnEnemyHit += (v) => PlayOneShot(FMODEvents.Instance.GetAudioEvent("EnemyHurt"), v.EventOrigin);
            GameEventManager.OnEnemyKill += (v) => PlayOneShot(FMODEvents.Instance.GetAudioEvent("EnemyKill"), v.EventOrigin);
            GameEventManager.OnPlayerHit += (v) => PlayOneShot(FMODEvents.Instance.GetAudioEvent("PlayerHurt"), v.EventOrigin);
        }

        /// <summary>
        /// Play a one shot sound effect.
        /// </summary>
        /// <param name="sfx">The sound effect to play.</param>
        /// <param name="worldPos">Where to play the sound effect in the game world.</param>
        public void PlayOneShot(EventReference sfx, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sfx, worldPos);
        }
    }
}