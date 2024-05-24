using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Flamenccio.Effects.Visual
{
    public interface ITrailPool
    {
        /// <summary>
        /// The object pool this object belongs to.
        /// </summary>
        ObjectPool<Trail> Pool { get; set; }
    }

    public class TrailPool : MonoBehaviour
    {
        public enum Trails
        {
            StarFlyTrail,
            EnemyMissileTrail,
            PortalTrail,
        }

        [SerializeField] private GameObject starFlyTrailPrefab;
        [SerializeField] private GameObject enemyMissileTrailPrefab;
        [SerializeField] private GameObject portalTrailPrefab;

        private const int DEFAULT_STAR_TRAIL = 20;
        private const int MAX_STAR_FLY_TRAIL = 40;
        private const int DEFAULT_E_MISSILE_TRAIL = 20;
        private const int MAX_E_MISSILE_TRAIL = 40;
        private const int DEFAULT_PORTAL_TRAIL = 10;
        private const int MAX_PORTAL_TRAIL = 50;

        private ObjectPool<Trail> starTrailPool;
        private ObjectPool<Trail> enemyMissileTrailPool;
        private ObjectPool<Trail> portalTrailPool;
        private Dictionary<Trails, ObjectPool<Trail>> trails = new();
        public Dictionary<Trails, ObjectPool<Trail>> TrailsPool { get => trails; }

        private void Start()
        {
            starTrailPool = new(CreateStarTrail, GetTrail, ReleaseTrail, DestroyTrail, true, DEFAULT_STAR_TRAIL, MAX_STAR_FLY_TRAIL);
            enemyMissileTrailPool = new(CreateEnemyMissileTrail, GetTrail, ReleaseTrail, DestroyTrail, true, DEFAULT_E_MISSILE_TRAIL, MAX_E_MISSILE_TRAIL);
            portalTrailPool = new(CreatePortalTrail, GetTrail, ReleaseTrail, DestroyTrail, true, DEFAULT_PORTAL_TRAIL, MAX_PORTAL_TRAIL);

            trails.Add(Trails.StarFlyTrail, starTrailPool);
            trails.Add(Trails.EnemyMissileTrail, enemyMissileTrailPool);
            trails.Add(Trails.PortalTrail, portalTrailPool);
        }

        private Trail CreateStarTrail()
        {
            Trail t = Instantiate(starFlyTrailPrefab).GetComponent<Trail>();
            t.Pool = starTrailPool;
            return t;
        }

        private Trail CreateEnemyMissileTrail()
        {
            Trail t = Instantiate(enemyMissileTrailPrefab).GetComponent<Trail>();
            t.Pool = enemyMissileTrailPool;
            return t;
        }

        private Trail CreatePortalTrail()
        {
            Trail t = Instantiate(portalTrailPrefab).GetComponent<Trail>();
            t.Pool = enemyMissileTrailPool;
            return t;
        }

        private void GetTrail(Trail s)
        {
            s.transform.SetPositionAndRotation(Vector2.zero, Quaternion.identity);
            s.gameObject.SetActive(true);
        }

        private void ReleaseTrail(Trail s)
        {
            s.gameObject.SetActive(false);
        }

        private void DestroyTrail(Trail s)
        {
            Destroy(s.gameObject);
        }
    }
}