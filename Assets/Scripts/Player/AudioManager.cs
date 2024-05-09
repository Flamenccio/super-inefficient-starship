using UnityEngine;
using FMODUnity;
using Flamenccio.Core;

namespace Flamenccio.Effects.Audio
{
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
            GameEventManager.OnEnemyHit += (v) => PlayOneShot(FMODEvents.Instance.enemyHurt, v.EventOrigin);
            GameEventManager.OnEnemyKilled += (v) => PlayOneShot(FMODEvents.Instance.enemyKill, v.EventOrigin);
            GameEventManager.OnPlayerHit += (v) => PlayOneShot(FMODEvents.Instance.playerHurt, v.EventOrigin);
        }
        public void PlayOneShot(EventReference sfx, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sfx, worldPos);
        }
    }
}
