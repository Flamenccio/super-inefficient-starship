using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    [CreateAssetMenu(fileName = "New TemporalAnimationInfo", menuName = "Create new TemporalAnimationInfo", order = 1)]
    public class TemporalAnimationInfo : ScriptableObject
    {
        public string AnimationName;
        public float AnimationDuration;
    }
}
