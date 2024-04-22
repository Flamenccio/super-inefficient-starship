using UnityEngine;
using FMODUnity;

namespace Flamenccio.Effects.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance { get; private set; }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("There is more than one AudioManager in the scene!");
            }
            instance = this;
        }

        public void PlayOneShot(EventReference sfx, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sfx, worldPos);
        }
    }
}
