using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Text
{
    public class JsonReader : MonoBehaviour
    {
        public static JsonReader Instance { get; private set; }

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

        public TextAsset LoadJson(string path)
        {
            return Resources.Load<TextAsset>(path);
        }

        public WeaponDescriptions LoadWeaponDescription(string path)
        {
            var json = LoadJson(path);
            var desc = JsonUtility.FromJson<WeaponDescriptions>(json.text);

            return desc;
        }
    }
}
