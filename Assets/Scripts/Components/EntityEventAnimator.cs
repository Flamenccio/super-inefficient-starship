using Flamenccio.Effects.Visual;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flamenccio.Components
{
    /// <summary>
    /// Controls animation flags in response to UnityEvents
    /// </summary>
    [Tooltip("Controls animation flags in response to UnityEvents")]
    public class EntityEventAnimator : MonoBehaviour
    {
        // This only works for sprite animation, since it's mainly controlled by flags
        [SerializeField] private Animator animator;

        private List<AnimatorControllerParameter> animatorParameters = new();
        private IEnumerator currentAnimation;
        private string currentAnimationName;

        private void Awake()
        {
            if (animator == null && !TryGetComponent<Animator>(out animator))
            {
                Debug.LogError("No animator attached");
                return;
            }

            animatorParameters = new(animator.parameters);
        }

        /// <summary>
        /// Plays an animation temporarily; immediately stops last animation, if any
        /// </summary>
        public void PlayAnimationTemporarily(TemporalAnimationInfo info)
        {
            if (currentAnimation != null)
            {
                // Cancel the current animation
                StopAnimation(currentAnimationName);
                StopCoroutine(currentAnimation);
            }

            currentAnimation = TempAnimation(info.AnimationName, info.AnimationDuration);
            currentAnimationName = info.AnimationName;
            StartCoroutine(currentAnimation);
        }

        /// <summary>
        /// Plays animation indefinetely
        /// </summary>
        /// <param name="animationConditionName">
        /// Name of condition that enables the animation
        /// </param>
        public void PlayAnimation(string animationConditionName)
        {
            if (string.IsNullOrEmpty(animationConditionName))
            {
                Debug.LogError("Given animation condition is null or empty");
                return;
            }

            if (!animatorParameters.Any(p => p.name.Equals(animationConditionName)))
            {
                Debug.LogError($"No animation condition with name {animationConditionName}");
                return;
            }

            animator.SetBool(animationConditionName, true);
        }

        /// <summary>
        /// Stops animation
        /// </summary>
        /// <param name="animationConditionName">
        /// Name of condition that enables the stopped animation
        /// </param>
        public void StopAnimation(string animationConditionName)
        {
            if (string.IsNullOrEmpty(animationConditionName))
            {
                return;
            }

            animator.SetBool(animationConditionName, false);
        }

        private IEnumerator TempAnimation(string animationConditionName, float timeSeconds)
        {
            PlayAnimation(animationConditionName);
            yield return new WaitForSeconds(timeSeconds);
            StopAnimation(animationConditionName);
        }
    }
}