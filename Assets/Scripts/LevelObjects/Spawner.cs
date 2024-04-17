using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Runtime.CompilerServices;
using System.Net;
using System.ComponentModel;
using System.Linq;

public class Spawner : MonoBehaviour
{
    // this class handles all spawning activity (enemies, walls, stages)
    private int stages = 0;
    private int walls = 0;
    private const int MAX_WALLS = 10000;
    private const int STAGE_LENGTH = 8;

    // prefabs
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject flyingStarPrefab;
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
    }
    private void Awake()
    {
        Debug.Log("Awake called");
        stageList.Add(FindObjectOfType<Stage>());
        enemyList = gameObject.GetComponent<EnemyList>();
    }
    public void DecreaseWallCount() // HACK kind of useless
    {
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
            tk.localSpawnCoords = GenerateLocalPosition(tk.root); // pick random local location
            tk.globalSpawnCoords = tk.localSpawnCoords + (Vector2)stageList[tk.root].transform.position; // update the global position
            tk.globalSpawnCoords = OffsetPosition(tk.globalSpawnCoords);
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
        Debug.Log($"Number of stages: {stageList.Count}");
        tk.rootStage = stageList[tk.root]; // TODO this breaks when reloading scene
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
            toolkit.root = GenerateRandomRootStage(); // get random stage number
            toolkit.rootStage = stageList[toolkit.root]; // get stage from stage number
            toolkit.rootStage.ScanNearbyStages(); // make sure things are connected
            localSpawnDirection.Direction = Directions.Instance.RandomDirection();
            toolkit.spawnReady = toolkit.rootStage.LinkableInDirection(localSpawnDirection.Direction); // check if the root stage can be extended in direction
        } while (!toolkit.spawnReady);

        List<StageVariant.Variants> blacklisted = new(StageResources.Instance.GetStageVariant(toolkit.rootStage.Variant).Links.First(v => v.LinkDirection == localSpawnDirection.Direction).BlackListedVariants); // copy blacklisted variants of stage link in chosen direction
        List<StageVariant.Variants> variants = new(StageResources.Instance.GetVariantsExtendableInDirection(Directions.Instance.OppositeOf(localSpawnDirection.Direction)).Except(blacklisted)); // basically, find all stage variants that can extend in the opposite direction of localSpawnDirection.Direction and then remove variants blacklisted by the roots variant.
        foreach (StageVariant.Variants variant in variants)
        {
            Debug.Log($"{localSpawnDirection.Direction}: {variant}");
        }
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

        // new method: attempt to find a nearby wall to spawn next to. if none is found, just place the wall
        toolkit.localSpawnCoords = GenerateLocalPositionOnGrid(toolkit.root);
        toolkit.globalSpawnCoords = toolkit.localSpawnCoords + (Vector2)toolkit.rootStage.transform.position;
        wall = Physics2D.OverlapCircle(toolkit.globalSpawnCoords, 12.0f, wallLayer);

        if (wall != null)
        {
            Vector2 offset = Directions.Instance.DirectionDictionary[Random.Range(1, 8)];
            toolkit.globalSpawnCoords = (Vector2)wall.transform.position + offset;
            toolkit.localSpawnCoords = toolkit.globalSpawnCoords - (Vector2)toolkit.rootStage.transform.position;
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

        instance.transform.localPosition = toolkit.localSpawnCoords;
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
    public void SpawnFlyingStar(Vector2 origin, Transform target, Transform canvas)
    {
        StarFly sf = Instantiate(flyingStarPrefab, origin, Quaternion.identity).GetComponent<StarFly>();
        sf.FlyTo(target);
    }
    /// <summary>
    /// Because the squares on the stage grid are offset by 0.5 units (the center of the grid is between the squares), we also need to offset each spawned entity and item by 0.5 units to perfectly fit the square.
    /// </summary>
    /// <param name="offset">Position to offset</param>
    /// <returns>Returns a modified version of the given position.</returns>
    private Vector2 OffsetPosition(Vector2 offset)
    {
        // offset the spawn location so that the wall spawns in the right place (on square)
        if (offset.x > 0) offset.x -= 0.5f;
        if (offset.y > 0) offset.y -= 0.5f;
        if (offset.x <= 0) offset.x += 0.5f;
        if (offset.y <= 0) offset.y += 0.5f;
        return offset;
    }
    private Vector2 AlignPosition(Vector2 position)
    {
        Vector2 newPos = new(Mathf.Ceil(position.x), Mathf.Ceil(position.y));
        return OffsetPosition(newPos);
    }
    private Vector2 GenerateLocalPosition(int rootStage)
    {
        return new Vector2(Mathf.FloorToInt(Random.Range(-stageList[rootStage].Extents.x, stageList[rootStage].Extents.x)), Mathf.FloorToInt(Random.Range(-stageList[rootStage].Extents.y, stageList[rootStage].Extents.y)));
    }
    private Vector2 GenerateLocalPositionOnGrid(int rootStage) // like GenerateLocalPosition, but returns coordinates aligned with grid.
    {
        Vector2 v = GenerateLocalPosition(rootStage);
        return OffsetPosition(v);
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
        } while(ray.collider != null);

        root.gameObject.layer = LayerMask.NameToLayer("Background"); // return stage layer
        int pairs = Mathf.FloorToInt(collisions.Count / 2);
        int pair = Random.Range(0, pairs);
        return new Vector2(Random.Range(collisions[2 * pair], collisions[(2 * pair) + 1]), yOrigin); // TODO this causes a bug, but idk how...
    }
}