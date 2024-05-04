using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Flamenccio.Effects.Visual
{
    public interface ITrailPool
    {
        ObjectPool<Trail> Pool { get; set; }
    }
    public class TrailPool : MonoBehaviour
    {
        public enum Trails
        {
            StarFlyTrail,
            EnemyMissileTrail,
        }
        [SerializeField] private GameObject starFlyTrailPrefab;
        [SerializeField] private GameObject enemyMissileTrailPrefab;
        private const int DEFAULT_STAR_TRAIL = 20;
        private const int MAX_STAR_FLY_TRAIL = 40;
        private const int DEFAULT_E_MISSILE_TRAIL = 20;
        private const int MAX_E_MISSILE_TRAIL = 40;
        private ObjectPool<Trail> starTrailPool;
        private ObjectPool<Trail> enemyMissileTrailPool;
        private Dictionary<Trails, ObjectPool<Trail>> trails = new();
        public Dictionary<Trails, ObjectPool<Trail>> TrailsPool { get => trails; }

        private void Start()
        {
            starTrailPool = new(CreateStarTrail, GetTrail, ReleaseTrail, DestroyTrail, true, DEFAULT_STAR_TRAIL, MAX_STAR_FLY_TRAIL);
            enemyMissileTrailPool = new(CreateEnemyMissileTrail, GetTrail, ReleaseTrail, DestroyTrail, true, DEFAULT_E_MISSILE_TRAIL, MAX_E_MISSILE_TRAIL);

            trails.Add(Trails.StarFlyTrail, starTrailPool);
            trails.Add(Trails.EnemyMissileTrail, enemyMissileTrailPool);
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
        private void GetTrail(Trail s)
        {
            s.transform.position = Vector2.zero;
            s.transform.rotation = Quaternion.identity;
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
