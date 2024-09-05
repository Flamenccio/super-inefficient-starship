using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flamenccio.Localization;
using UnityEngine.Localization;
using System.Linq;
using Flamenccio.Powerup.Weapon;
using UnityEngine.Events;

namespace Flamenccio
{
    public class Test : MonoBehaviour, IDescribable
    {
        public string Variable { get; set; }
        [SerializeField] private LocalizedString localString;

        public LocalizedString CompleteDescription(LocalizedString description)
        {
            return localString;
        }
    }
}
