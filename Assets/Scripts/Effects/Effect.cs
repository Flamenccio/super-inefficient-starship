using UnityEngine;

namespace Flamenccio.Effects.Visual
{
    /// <summary>
    /// Base class that handles visual effects; in this game, they are just animated sprites whose animation plays once, then dissappears.
    /// </summary>
    public class Effect : MonoBehaviour
    {
        public string EffectName { get => effectName; }
        [SerializeField] protected string effectName;
        [SerializeField] protected Animator animator;
        protected float timer = 0f;
        protected float animLength = 0f;

        protected virtual void Start()
        {
            animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        }

        protected void FixedUpdate()
        {
            if (timer >= animLength)
            {
                End();
            }

            Behavior();
        }

        protected virtual void End()
        {
            Destroy(gameObject);
        }

        protected virtual void Behavior()
        {
            timer += Time.deltaTime;
        }
    }
}