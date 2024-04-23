using UnityEngine;
using FMODUnity;

namespace Flamenccio.Effects.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            /*
            if (Instance != null)
            {
                Debug.LogError("There is more than one AudioManager in the scene!");
            }
            Instance = this;
            */
            
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

        }

        public void PlayOneShot(EventReference sfx, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sfx, worldPos);
        }
    }
}
