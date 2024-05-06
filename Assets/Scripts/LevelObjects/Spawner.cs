using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Flamenccio.Utility;
using Flamenccio.LevelObject.Stages;
using Flamenccio.LevelObject;
using Flamenccio.LevelObject.Walls;
using Flamenccio.Effects.Visual;

namespace Flamenccio.Core
{
    public class Spawner : MonoBehaviour
    {
        // prefabs
        [SerializeField] private GameObject stagePrefab;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private GameObject flyingStarPrefab;
        [SerializeField] private GameObject portalPrefab;

        // other necessary classes/objects
        [SerializeField] private GameObject stageContainer;
        [SerializeField] private List<Stage> stageList = new();
        private EnemyList enemyList;

        // layer masks
        [SerializeField] private LayerMask stageLayer; // layer of all stages
        [SerializeField] private LayerMask entityLayers; // layer of all game objects that need their own unique space
        [SerializeField] private LayerMask wallLayer; // layer of all walls
        [SerializeField] private LayerMask itemLayer; // layer of all items
        [SerializeField] private LayerMask raycastEnterLayer; // layer used for finding point in stage
        [SerializeField] private LayerMask raycastExitLayers; // layer of invisible walls

        // this class handles all spawning activity (enemies, walls, stages)
        private int stages = 0;
        private int walls = 0;
        private const int MAX_WALLS = 10000;
        private const int STAGE_LENGTH = 8;

        private struct SpawnToolkit
        {
            public Vector2 globalSpawnCoords;
            public Vector2 localSpawnCoords;
            public int root;
            public Stage rootStage;
            public bool spawnReady;
            public int spawnAttempts;
            public Collider2D stageCheck;
        }
        // properties
        public int Stages { get => stages; }

        // constants
        private const float ENEMY_SPAWN_RADIUS = 5.0f; // the minimum space required between the player and enemy for it (the enemy) to spawn
        private void Start()
        {
            stageList.Add(FindObjectOfType<Stage>());
            enemyList = gameObject.GetComponent<EnemyList>();
        }
        public void DecreaseWallCount()
        {
            if (walls <= 0) return;

            walls--;
        }
        public GameObject SpawnEnemy(int difficulty)
        {
            // if the level is too low to spawn anything, return nothing
            if (difficulty < enemyList.MinimumEnemySpawningLevel) return null;

            SpawnToolkit tk = new();
            Collider2D target;
            List<Collider2D> targets = new();

            while (!tk.spawnReady && tk.spawnAttempts < 5)
            {
                tk.spawnAttempts++;
                tk.root = GenerateRandomRootStage(); // pick random root stage
                tk.rootStage = stageList[tk.root];
                tk.globalSpawnCoords = GenerateGlobalPositionOnGrid(stageList[tk.root].transform, tk.rootStage);
                target = Physics2D.OverlapPoint(tk.globalSpawnCoords, wallLayer); // check if a wall is already at that location

                if (target != null) continue;

                target = Physics2D.OverlapCircle(tk.globalSpawnCoords, ENEMY_SPAWN_RADIUS, LayerMask.GetMask("Player")); // check if the player is within the spawn radius (enemySpawnRadius)

                if (target != null) continue;

                target = Physics2D.OverlapPoint(tk.globalSpawnCoords, stageLayer); // check if the point is viable (within the stage)

                if (target == null) continue;

                tk.spawnReady = true;
            }
            if (!tk.spawnReady) return null;

            GameObject enemy = enemyList.GetRandomEnemy(difficulty);

            if (enemy == null) return null;

            targets.AddRange(Physics2D.OverlapCircleAll(tk.globalSpawnCoords, 2.0f, wallLayer)); // destroy walls around the spawn location

            foreach (Collider2D wallCollider in targets)
            {
                wallCollider.GetComponent<Wall>().Die();
                DecreaseWallCount();
            }

            return Instantiate(enemy, tk.globalSpawnCoords, Quaternion.Euler(0f, 0f, 0f));
        }

        /// <summary>
        /// spawn a star on stage
        /// </summary>
        /// <returns>returns the spanwed star object</returns>
        public GameObject SpawnStar()
        {
            // spawn a star at a random location
            SpawnToolkit tk = InitializeNewToolkit();
            tk.root = GenerateRandomRootStage(); // choose a root stage to spawn in
            tk.rootStage = stageList[tk.root];
            tk.globalSpawnCoords = FindPointInStage(tk.rootStage);
            tk.globalSpawnCoords = AlignPosition(tk.globalSpawnCoords);
            GameObject star = Instantiate(starPrefab, tk.rootStage.transform); // spawn the star
            star.transform.position = tk.globalSpawnCoords;
            return star;
        }

        public void SpawnStage()
        {
            stages++; // increase number of stage count
            SpawnToolkit toolkit = InitializeNewToolkit();
            Directions.Cardinals localSpawnDirection = new()
            {
                Vector = Vector2.up
            };
            Stage newStage;

            do
            {
                toolkit.root = GenerateRandomRootStage();
                toolkit.rootStage = stageList[toolkit.root]; // get stage from stage number
                toolkit.rootStage.ScanNearbyStages(); // make sure things are connected
                localSpawnDirection.Direction = Directions.Instance.RandomDirection();
                toolkit.spawnReady = toolkit.rootStage.LinkableInDirection(localSpawnDirection.Direction); // check if the root stage can be extended in direction
            } while (!toolkit.spawnReady);

            List<StageVariant.Variants> blacklisted = new(StageResources.Instance.GetStageVariant(toolkit.rootStage.Variant).Links.First(v => v.LinkDirection == localSpawnDirection.Direction).BlackListedVariants); // copy blacklisted variants of stage link in chosen direction
            List<StageVariant.Variants> variants = new(StageResources.Instance.GetVariantsExtendableInDirection(Directions.Instance.OppositeOf(localSpawnDirection.Direction)).Except(blacklisted)); // basically, find all stage variants that can extend in the opposite direction of localSpawnDirection.Direction and then remove variants blacklisted by the roots variant.
            StageVariant.Variants v = variants[Random.Range(1, variants.Count)]; // pull a random variant from the list (excluding NORMAL variant)
            newStage = Instantiate(stagePrefab, stageContainer.transform).GetComponent<Stage>(); // instantiate new stage
            newStage.UpdateVariant(v);
            toolkit.rootStage.LinkStageUnsafe(localSpawnDirection.Direction, newStage);
            newStage.LinkStageUnsafe(Directions.Instance.OppositeOf(localSpawnDirection.Direction), toolkit.rootStage);
            toolkit.globalSpawnCoords = (Vector2)toolkit.rootStage.transform.position + (STAGE_LENGTH * 2 * localSpawnDirection.Vector);
            newStage.transform.position = toolkit.globalSpawnCoords; // place the new stage in the right place
            newStage.ScanNearbyStages(); // scan for nearby stages
            stageList.Add(newStage); // add the new stage to the stage list
        }
        public void SpawnWall(int wallLevel)
        {
            if (walls >= MAX_WALLS) return;

            SpawnToolkit toolkit = InitializeNewToolkit();
            toolkit.root = GenerateRandomRootStage();
            toolkit.rootStage = stageList[toolkit.root];
            Collider2D wall;
            toolkit.globalSpawnCoords = GenerateGlobalPositionOnGrid(toolkit.rootStage.transform, toolkit.rootStage);
            wall = Physics2D.OverlapCircle(toolkit.globalSpawnCoords, 12.0f, wallLayer);

            if (wall != null) // spawn adjacent to an existing wall
            {
                Vector2 offset = Directions.Instance.DirectionDictionary[Random.Range(1, 8)];
                toolkit.globalSpawnCoords = (Vector2)wall.transform.position + offset;
            }

            wall = Physics2D.OverlapPoint(toolkit.globalSpawnCoords, stageLayer); // check if point is valid

            if (wall == null || wall.gameObject != toolkit.rootStage.gameObject) // if the spawn point is off the map, give up
            {
                return;
            }

            wall = Physics2D.OverlapPoint(toolkit.globalSpawnCoords, entityLayers); // is something already there

            if (wall != null) // if something is already present, give up
            {
                return;
            }

            GameObject instance = Instantiate(wallPrefab, stageList[toolkit.root].transform);

            if (wallLevel == 2)
            {
                instance.GetComponent<Wall>().Upgrade(); // if wall level is 2, upgrade the wall
            }

            instance.transform.position = toolkit.globalSpawnCoords;
            walls++; // increase wall count
        }
        public GameObject SpawnHeart()
        {
            SpawnToolkit toolkit = InitializeNewToolkit();
            toolkit.root = GenerateRandomRootStage(); // choose a root stage to spawn in
            toolkit.rootStage = stageList[toolkit.root];
            toolkit.globalSpawnCoords = FindPointInStage(toolkit.rootStage);
            toolkit.globalSpawnCoords = AlignPosition(toolkit.globalSpawnCoords);

            // spawn the heart 
            GameObject heart = Instantiate(heartPrefab, toolkit.rootStage.transform);
            heart.transform.position = toolkit.globalSpawnCoords;
            return heart;
        }
        public void SpawnFlyingStar(Vector2 origin, Transform target)
        {
            StarFly sf = Instantiate(flyingStarPrefab, origin, Quaternion.identity).GetComponent<StarFly>();
            sf.FlyTo(target);
        }
        /// <summary>
        /// Spawns <b>two</b> portals.
        /// </summary>
        public void SpawnPortal()
        {
            if (stages < 2) return;

            bool spawnReady = false;
            int target1 = GenerateRandomRootStage();
            int target2;

            Stage target1Transform = null;
            Stage target2Transform = null;

            do
            {
                target2 = GenerateRandomRootStage();

                if (target2 == target1) continue; 

                target1Transform = stageList[target1].GetComponent<Stage>();
                target2Transform = stageList[target2].GetComponent<Stage>();

                if (Vector2.Distance(target1Transform.transform.position, target2Transform.transform.position) < 8f) continue;

                spawnReady = true;

            } while (!spawnReady);

            float hue = Random.Range(0f, 1f);
            float sat = Random.Range(0f, 1f);
            Color newColor = Color.HSVToRGB(hue, sat, 1f);

            Vector2 target1local;
            Vector2 target2local;
            spawnReady = false;

            do
            {
                target1local = GlobalToLocalPosition(NormalizePosition(FindPointInStage(target1Transform)), target1Transform.transform.position);
                target2local = GlobalToLocalPosition(NormalizePosition(FindPointInStage(target2Transform)), target2Transform.transform.position);

                if (Vector2.Distance(LocalToGlobalPosition(target1local, target1Transform.transform.position), LocalToGlobalPosition(target2local, target2Transform.transform.position)) >= 8f) spawnReady = true;

            } while (!spawnReady);

            Portal portal1 = Instantiate(portalPrefab, target1Transform.transform, false).GetComponent<Portal>();
            Portal portal2 = Instantiate(portalPrefab, target2Transform.transform, false).GetComponent<Portal>();
            portal1.transform.localPosition = target1local;
            portal2.transform.localPosition = target2local;
            portal1.SetDestination(portal2);
            portal2.SetDestination(portal1);
            portal1.PortalColor = newColor;
            portal2.PortalColor = newColor;
        }
        /// <summary>
        /// Takes a global position and returns it aligned with a grid.
        /// </summary>
        private Vector2 AlignPosition(Vector2 globalPosition)
        {
            return NormalizePosition(globalPosition);
        }
        private Vector2 GenerateLocalPositionOnGrid(Stage rootStage)
        {
            float xCoordinate = Random.Range(-rootStage.Extents.x, rootStage.Extents.x);
            float yCoordinate = Random.Range(-rootStage.Extents.y, rootStage.Extents.y);
            return NormalizePosition(new(xCoordinate, yCoordinate));
        }
        private Vector2 GenerateGlobalPositionOnGrid(Transform parent, Stage root)
        {
            Vector2 v = GenerateLocalPositionOnGrid(root);
            Vector2 globalPosition = LocalToGlobalPosition(v, (Vector2)parent.position);
            return globalPosition;
        }
        private int GenerateRandomRootStage()
        {
            return Random.Range(0, stageList.Count);
        }
        private SpawnToolkit InitializeNewToolkit()
        {
            return new()
            {
                root = 0,
                rootStage = null,
                localSpawnCoords = Vector2.zero,
                globalSpawnCoords = Vector2.zero,
                spawnAttempts = 0,
                spawnReady = false,
                stageCheck = null
            };
        }
        /// <summary>
        /// Returns a random point inside polygon defined by stage. This method should only be used if an item MUST be spawned (e.g. star, hearts).
        /// </summary>
        private Vector2 FindPointInStage(Stage root)
        {
            float xBounds = root.Extents.x;
            float yBounds = root.Extents.y - 0.5f;
            float yOrigin = Random.Range(-yBounds, yBounds) + root.gameObject.transform.position.y + root.Center.y;
            float xOrigin = root.gameObject.transform.position.x - xBounds - 1; // the raycast should start just outside the stage
            bool turn = false; // false = looking for raycastTestLayer; true = looking for inviswall layer
            List<float> collisions = new(); // x coordinates--y is kept constant
            root.gameObject.layer = LayerMask.NameToLayer("RaycastTest"); // temporarily change stage layer
            RaycastHit2D ray;

            do
            {
                ray = Physics2D.Raycast(new Vector2(xOrigin, yOrigin), Vector2.right, 16, turn ? raycastExitLayers : raycastEnterLayer);

                if (ray.collider == null) break;

                xOrigin = ray.point.x + 0.05f;
                collisions.Add(ray.point.x);
                turn = !turn;
            } while (ray.collider != null);

            root.gameObject.layer = LayerMask.NameToLayer("Background"); // return stage layer
            int pairs = Mathf.FloorToInt(collisions.Count / 2f);
            int pair = Random.Range(0, pairs);
            Vector2 randomizedPosition = new(Random.Range(collisions[2 * pair], collisions[(2 * pair) + 1]), yOrigin);
            return randomizedPosition;
        }
        // these two methods exist because I'm stupid and can't remember how to convert
        private Vector2 GlobalToLocalPosition(Vector2 globalPosition, Vector2 origin)
        {
            return globalPosition - origin;
        }
        private Vector2 NormalizePosition(Vector2 position)
        {
            Vector2 p = new(Mathf.Floor(position.x) + 0.5f, Mathf.Floor(position.y) + 0.5f);
            return p;
        }
        private Vector2 LocalToGlobalPosition(Vector2 localPosition, Vector2 origin)
        {
            return origin + localPosition;
        }
    }
}