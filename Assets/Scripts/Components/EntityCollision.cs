using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Flamenccio.Components
{
    /// <summary>
    /// Invokes UnityEvents when the attached GameObject enters a trigger or collision
    /// </summary>
    [Tooltip("Invokes UnityEvents when the attached GameObject enters a trigger or collision")]
    public class EntityCollision : MonoBehaviour
    {
        [Serializable]
        public class CollisionEvent
        {
            [Tooltip("What should be notified when this GameObject collides with another with the given Tag?")] public UnityEvent<Collider2D> Collision;
            [Tooltip("The tag of the GameObject that triggers the collision event")] public string Tag;

            public void Collide(string tag, Collider2D collider)
            {
                if (tag.Equals(Tag))
                {
                    Collision?.Invoke(collider);
                }
            }
        }

        [Tooltip("Events that are invoked when this GameObject enters a collision collider"), SerializeField] private List<CollisionEvent> collisionEvents = new();
        [Tooltip("Events that are invoked when this GameObject enters a trigger collider"), SerializeField] private List<CollisionEvent> triggerEvents = new();

        private Action<string, Collider2D> collide;
        private Action<string, Collider2D> trigger;

        private void Awake()
        {
            collisionEvents
                .ForEach(x => collide += x.Collide);

            triggerEvents
                .ForEach(x => trigger += x.Collide);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            collide?.Invoke(collision.gameObject.tag, collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            trigger?.Invoke(collider.gameObject.tag, collider);
        }
    }
}