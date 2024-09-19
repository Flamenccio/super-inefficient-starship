using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.HUD
{
    /// <summary>
    /// Animates ui elements
    /// </summary>
    public class UiElementMovement : MonoBehaviour
    {
        public enum TransitionAnimation
        {
            [Tooltip("No animation")] None,
            Slide
        }

        [SerializeField, Tooltip("UI elements to control")] private List<GameObject> uiElements = new();
        [SerializeField, Tooltip("Used to move UI elements")] private Transform parentTransform;
        [SerializeField] private TransitionAnimation enterAnimation = TransitionAnimation.None;
        [SerializeField] private TransitionAnimation exitAnimation = TransitionAnimation.None;
        private Action doEnterAnimation;
        private Action doExitAnimation;
        private Vector2 origin;

        private void Start()
        {
            /* set animations
             * if slide animation:
             *      set origin position
             */
        }

        private Action GetEnterAnimation(TransitionAnimation animation)
        {
            return () => { }; // placeholder
        }

        private Action GetExitAnimation(TransitionAnimation animation)
        {
            return () => { }; // placeholder
        }
    }
}
