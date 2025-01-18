using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline;

namespace Flamenccio.Utility
{
    /// <summary>
    /// Allows requests to find GameObjects with specified conditions.
    /// </summary>
    public static class EntityScanner
    {
        /// <summary>
        /// Returns nearest GameObject with given tag.
        /// </summary>
        /// <param name="tags">The possible tags that valid GameObjects can have.</param>
        /// <param name="inLineOfSight">Do these GameObjects have to be within line of sight?</param>
        /// <param name="searchRadius">How far to search.</param>
        /// <param name="obstructingLayers">The layers that may obstruct line of sight.</param>
        /// <param name="targetLayers">The layers where the target GameObjects reside.</param>
        /// <param name="searchPoint">Point where search is made.</param>
        /// <returns>A GameObject. Null if there are no such GameObjects.</returns>
        public static GameObject SearchForNearestGameObjectsWithTag(List<string> tags, Vector2 searchPoint, bool inLineOfSight, float searchRadius, LayerMask targetLayers, LayerMask obstructingLayers)
        {
            var tagArray = tags.ToArray();
            return SearchForNearestGameObjectsWithTag(searchPoint, inLineOfSight, searchRadius, targetLayers, obstructingLayers, tagArray);
        }
        
        /// <summary>
        /// Returns a nearby GameObject with given tag.
        /// </summary>
        /// <param name="tags">The possible tags that valid GameObjects can have.</param>
        /// <param name="inLineOfSight">Do these GameObjects have to be within line of sight?</param>
        /// <param name="searchRadius">How far to search.</param>
        /// <param name="obstructingLayers">The layers that may obstruct line of sight.</param>
        /// <param name="targetLayers">The layers where the target GameObjects reside.</param>
        /// <returns>A GameObject. Null if there are no such GameObjects.</returns>
        public static GameObject SearchForNearestGameObjectsWithTag(Vector2 searchPoint, bool inLineOfSight, float searchRadius, LayerMask targetLayers, LayerMask obstructingLayers, params string[] tags)
        {
            SearchForAllGameObjectsWithTag(searchPoint, inLineOfSight, searchRadius, targetLayers, obstructingLayers, out var gameObjects, tags);
            var closestDistance = searchRadius + 1.0f;
            GameObject closestGameObject = null;

            foreach (var obj in gameObjects)
            {
                var currentDistance = Vector2.Distance(searchPoint, obj.transform.position);
                
                if (currentDistance < closestDistance)
                {
                    closestGameObject = obj;
                    closestDistance = currentDistance;
                }
            }
            
            return closestGameObject;
        }

        /// <summary>
        /// Finds all GameObjects with tag near point and places result in result list.
        /// </summary>
        /// <param name="searchPoint">Point in world to search.</param>
        /// <param name="lineOfSight">Must the game object be unobstructed?</param>
        /// <param name="searchRadius">Radius of search circle.</param>
        /// <param name="targetLayers">Possible layers of targets.</param>
        /// <param name="obstructingLayer">Layers that will obstruct search targets.</param>
        /// <param name="result">Results of all nearby GameObjects are placed here.</param>
        /// <param name="tags">Array of tags that targets must contain.</param>
        /// <returns>Number of GameObjects found with tag.</returns>
        public static int SearchForAllGameObjectsWithTag(Vector2 searchPoint, bool lineOfSight, float searchRadius, LayerMask targetLayers, LayerMask obstructingLayer, out List<GameObject> result, params string[] tags)
        {
            var colliders = new List<Collider2D>();
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(targetLayers);
            //var amt = Physics2D.OverlapCircle(searchPoint, searchRadius, contactFilter, colliders);
            colliders = Physics2D.OverlapCircleAll(searchPoint, searchRadius, targetLayers).ToList();
            
            // Filter colliders to conditions specified in parameters
            result = colliders.Where((x) =>
            {
                var inLineOfSight = IsInLineOfSight(
                    searchPoint, 
                    x.transform.position, 
                    Vector2.Distance(searchPoint, x.transform.position), 
                    obstructingLayer, 
                    targetLayers);
                
                var hasSpecifiedTags = tags.Contains(x.tag);
                
                return (inLineOfSight || !lineOfSight) && hasSpecifiedTags;
            })
            .Select(x => x.gameObject)
            .ToList();
            
            return result.Count;
        }
        
        public static bool IsInLineOfSight(Vector2 origin, Vector2 target, float maxDist, LayerMask obstructingLayers, LayerMask targetLayers)
        {
            Vector2 dir = (target - origin).normalized;
            RaycastHit2D ray = Physics2D.Raycast(
                origin, 
                dir, 
                maxDist, 
                obstructingLayers | targetLayers);

            if (!ray)
            {
                return false;
            }

            int hitLayer = ray.collider.gameObject.layer;
            

            return (targetLayers & (1 << hitLayer)) > 0;
        }
    }
}
