using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Utility
{
    public class JsonParser : MonoBehaviour
    {
        public class WeaponDescription
        {
            public string Name { get; set; }
            public string Desc { get; set; }
        }

        public static JsonParser Instance { get; private set; }

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

        private TextAsset LoadJson(string path)
        {
            return Resources.Load<TextAsset>(path);
        }

        private void ReadJson(TextAsset json)
        {
            JsonUtility.FromJson(json.text, typeof(WeaponDescription));
        }
    }
}
