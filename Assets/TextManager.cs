using Flamenccio.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Text
{
    [System.Serializable]
    public class WeaponDescriptions
    {
        public List<WeaponDescription> weapons = new();
    }

    [System.Serializable]
    public class WeaponDescription
    {
        public string weaponName;
        public string descTap;
        public string descHold;
        public string descHoldEnter;
        public string descHoldExit;
        public string descEffect;
    }

    public class TextManager : MonoBehaviour
    {
        private string basePath = string.Empty;
        private JsonReader reader;
        private Dictionary<string, WeaponDescriptions> weapons = new();
        private string language = "en"; // default to En

        private void Start()
        {
            reader = JsonReader.Instance;
            LoadAllText(language);
        }
        public void ChangeLanguage(string newLang)
        {
            language = newLang.ToLower();
            LoadAllText(language);
        }

        public void LoadAllText(string language)
        {
            basePath = $"Text/{language}";
            LoadAllWeaponDescriptions();
        }

        public void LoadAllWeaponDescriptions()
        {
            weapons.Clear();
            weapons.Add("main", reader.LoadWeaponDescription($"{basePath}/Weapons/mains"));
            weapons.Add("sub", reader.LoadWeaponDescription($"{basePath}/Weapons/subs"));
            weapons.Add("special", reader.LoadWeaponDescription($"{basePath}/Weapons/specials"));

            /* DEBUG
            Debug.Log("hi");
            foreach (var w in weapons["main"].weapons)
            {
                Debug.Log($"{w.weaponName}");
            }
            */
        }
    }
}