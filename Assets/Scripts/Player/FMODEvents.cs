using UnityEngine;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;

namespace Flamenccio.Effects.Audio
{
    [System.Serializable]
    public struct AudioEvent
    {
        [field: SerializeField] public EventReference Audio { get; set; }
        [field: SerializeField, Tooltip("Naming convention: <Category><Source><Event>. No spaces, PascalCasing, no plurals, no empty strings.")] public string Name { get; set; }
    }

    public class FMODEvents : MonoBehaviour
    {
        public static FMODEvents Instance { get; private set; }

        [SerializeField] private List<AudioEvent> audioEvents = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("More than one instance of FMODEvents exists.");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            InitializeAudioList();
        }

        private void InitializeAudioList()
        {
            audioEvents = audioEvents
                .Where(x => !x.Name.Equals(string.Empty))
                .ToList();
        }

        public EventReference GetAudioEvent(string name)
        {
            return audioEvents
                .Find(x => x.Name.Equals(name))
                .Audio;
        }
    }
}
