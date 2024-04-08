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

public class Spawner : MonoBehaviour
{
    // this class handles all spawning activity (enemies, walls, stages)
    private int stages = 0;
    private int walls = 0;
    private const int MAX_WALLS = 10000;
    private const int STAGE_LENGTH = 8;
    private const int MIN_WALL_UPGRADE_LEVEL = 9;

    // prefabs
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject heartPrefab;
    // other necessary classes/objects
    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private Directions directionsClass = new Directions();
    [SerializeField] private GameObject stageContainer;
    [SerializeField] private List<Stage> stageList = new List<Stage>();
    private EnemyList enemyList;

    // layer masks
    [SerializeField] private LayerMask stageLayer; // layer of all stages
    [SerializeField] private LayerMask entityLayers; // layer of all game objects that need their own unique space
    [SerializeField] private LayerMask wallLayer; // layer of all walls
    [SerializeField] private LayerMask itemLayer; // layer of all items

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
    private void Awake()
    {
        // add the starting stage
        stageList.Add(FindObjectOfType<Stage>());
        enemyList = gameObject.GetComponent<EnemyList>();
    }
    public void DecreaseWallCount()
    {
        walls--;
    }
    public GameObject SpawnEnemy(int difficulty)
    {
        // if the level is too low to spawn anything, return nothing
        if (difficulty < enemyList.MinimumEnemySpawningLevel) return null;

        // spawn an enemy at a random location (make sure it's not too close to the player)
        // the enemy should be scaled with difficulty
        bool spawnReady = false;

        Vector2 localSpawnCoord = Vector2.zero;

        Vector2 globalSpawnCoord = Vector2.zero;

        int attempts = 0;

        int root = 0;

        Collider2D target = null;
        Collider2D[] targets = null;

        while (!spawnReady && attempts < 5)
        {
            attempts++;

            root = GenerateRandomRootStage(); // pick random root stage

            localSpawnCoord = GenerateLocalPosition(root); // pick random local location
            
            // update the global position
            globalSpawnCoord = localSpawnCoord + (Vector2)stageList[root].transform.position;
            globalSpawnCoord = OffsetPosition(globalSpawnCoord);

            target = Physics2D.OverlapPoint(globalSpawnCoord, wallLayer); // check if a wall is already at that location
            if (target != null) continue;

            target = Physics2D.OverlapCircle(globalSpawnCoord, ENEMY_SPAWN_RADIUS, LayerMask.GetMask("Player")); // check if the player is within the spawn radius (enemySpawnRadius)
            if (target != null) continue;

            target = Physics2D.OverlapPoint(globalSpawnCoord, stageLayer); // check if the point is viable (within the stage)
            if (target == null) continue;

            spawnReady = true;
        }
        if (!spawnReady) return null;

        GameObject enemy = enemyList.GetRandomEnemy(difficulty);
        if (enemy == null) return null;

        targets = Physics2D.OverlapCircleAll(globalSpawnCoord, 2.0f, wallLayer); // destroy walls around the spawn location
        foreach (Collider2D wallCollider in targets)
        {
            wallCollider.GetComponent<Wall>().Die();
            DecreaseWallCount();
        }

        Instantiate(enemy, globalSpawnCoord, Quaternion.Euler(0f, 0f, 0f));

        return null;
    }

    /// <summary>
    /// spawn a star on stage
    /// </summary>
    /// <returns>returns the spanwed star object</returns>
    public GameObject SpawnStar()
    {
        // spawn a star at a random location
        SpawnToolkit tk = InitializeNewToolkit();
        Collider2D target;

        while (!tk.spawnReady)
        {

            // choose a root stage to spawn in
            tk.root = GenerateRandomRootStage();
            tk.rootStage = stageList[tk.root];

            tk.localSpawnCoords = GenerateLocalPosition(tk.root);
            tk.localSpawnCoords = OffsetPosition(tk.localSpawnCoords);

            tk.globalSpawnCoords = tk.localSpawnCoords + (Vector2)tk.rootStage.transform.position; // calculate a global position based off of the local position

            // check if the point is viable (within the stage)
            target = Physics2D.OverlapPoint(tk.globalSpawnCoords, stageLayer);
            if (target == null) continue;

            // check if the point is not occupied by other items
            target = Physics2D.OverlapPoint(tk.globalSpawnCoords, itemLayer);
            if (target != null) continue;

            tk.spawnReady = true;
        }
        if (!tk.spawnReady) return null;

        // spawn the star
        GameObject star = Instantiate(starPrefab, tk.rootStage.transform);
        star.transform.localPosition = tk.localSpawnCoords;
        return star;
    }

    public void SpawnStage()
    {
        stages++; // increase number of stage count

        SpawnToolkit toolkit = InitializeNewToolkit();

        Directions.Cardinals localSpawnDirection = new Directions.Cardinals(); // initialize new Cardinal struct
        localSpawnDirection.Vector = Vector2.up;

        toolkit.spawnReady = false;
        Stage newStage = null;

        do
        {
            toolkit.root = GenerateRandomRootStage(); // get a random stage number
            toolkit.rootStage = stageList[toolkit.root]; // get the random stage object
            toolkit.rootStage.ScanNearbyStages(); // make sure there are not any stray stages nearby before we begin adding stuff.
            localSpawnDirection.Direction = directionsClass.RandomDirection(); // choose random cardinal to spawn new stage 

            // grab a random stage variant; avoid using the normal stage (type 0)
            StageVariant.variants newStageVariant = (StageVariant.variants)Random.Range(1, System.Enum.GetNames(typeof(StageVariant.variants)).Length); 
            newStage = Instantiate(stagePrefab, stageContainer.transform).GetComponent<Stage>(); // instantiate new thing
            newStage.UpdateVariant(newStageVariant);

            if (toolkit.rootStage.Extend(localSpawnDirection.Direction, newStage))
            {
                toolkit.spawnReady = true;
            }
            else
            {
                Destroy(newStage.gameObject); // i hate how this works but it works
            }
        } while (!toolkit.spawnReady);

        toolkit.globalSpawnCoords = (Vector2)toolkit.rootStage.transform.position + STAGE_LENGTH * 2 * localSpawnDirection.Vector;
        newStage.transform.position = toolkit.globalSpawnCoords; // place the new stage in the right place
        newStage.ScanNearbyStages(); // scan for nearby stages

        stageList.Add(newStage); // add the new stage to the stage list

        cameraControl.IncreaseCameraSize(); // zoom camera out
    }
    public void SpawnWall(int wallLevel)
    {
        if (walls >= MAX_WALLS) return;

        SpawnToolkit toolkit = InitializeNewToolkit();
        toolkit.root = GenerateRandomRootStage();
        toolkit.rootStage = stageList[toolkit.root];

        Collider2D wall;

        // new method: attempt to find a nearby wall to spawn next to. if none is found, just place the wall

        toolkit.localSpawnCoords = GenerateLocalPosition(toolkit.root);
        toolkit.localSpawnCoords = OffsetPosition(toolkit.localSpawnCoords);
        toolkit.globalSpawnCoords = toolkit.localSpawnCoords + (Vector2)toolkit.rootStage.transform.position;

        wall = Physics2D.OverlapCircle(toolkit.globalSpawnCoords, 12.0f, wallLayer);

        if (wall != null)
        {
            Vector2 offset = directionsClass.DirectionDictionary[Random.Range(1, 8)];
            toolkit.globalSpawnCoords = (Vector2)wall.transform.position + offset;
            toolkit.localSpawnCoords = toolkit.globalSpawnCoords - (Vector2)toolkit.rootStage.transform.position;
        }

        // check if point is valid
        wall = Physics2D.OverlapPoint(toolkit.globalSpawnCoords, stageLayer);
        if (wall == null || wall.gameObject != toolkit.rootStage.gameObject) // if the spawn point is off the map, try again
        {
            SpawnWall(wallLevel);
            return;
        }

        // is something already there
        wall = Physics2D.OverlapPoint(toolkit.globalSpawnCoords, entityLayers);
        if (wall != null)
        {
            return;
        }

        GameObject instance = Instantiate(wallPrefab, stageList[toolkit.root].transform);
        if (wallLevel == 2)
        {
            instance.GetComponent<Wall>().Upgrade(); // if wall level is 2, upgrade the wall
        }
        instance.transform.localPosition = toolkit.localSpawnCoords;

        // increase wall count
        walls++;
    }
    public GameObject SpawnHeart()
    {
        SpawnToolkit toolkit = InitializeNewToolkit();

        // because the star MUST spawn, we use a loop until success
        while (!toolkit.spawnReady && toolkit.spawnAttempts < 5)
        {
            toolkit.spawnAttempts++;

            // choose a root stage to spawn in
            toolkit.root = GenerateRandomRootStage();

            // create a randomized local position
            toolkit.localSpawnCoords = GenerateLocalPosition(toolkit.root);
            toolkit.localSpawnCoords = OffsetPosition(toolkit.localSpawnCoords);

            // calculate a global position based off of the local position
            toolkit.globalSpawnCoords = toolkit.localSpawnCoords + (Vector2)stageList[toolkit.root].transform.position;

            // check if the point is viable (within the stage)
            toolkit.stageCheck = Physics2D.OverlapPoint(toolkit.globalSpawnCoords, stageLayer);
            if (toolkit.stageCheck == null) continue;

            // check if the point is not occupied by other items
            toolkit.stageCheck = Physics2D.OverlapPoint(toolkit.globalSpawnCoords, itemLayer);
            if (toolkit.stageCheck != null) continue;

            toolkit.spawnReady = true;
        }
        if (!toolkit.spawnReady) return null;

        // spawn the star
        GameObject star = Instantiate(heartPrefab, stageList[toolkit.root].transform);
        star.transform.localPosition = toolkit.localSpawnCoords;
        return star;
    }
    /// <summary>
    /// Because the squares on the stage grid are offset by 0.5 units (the center of the grid is between the squares), we also need to offset each spawned entity and item by 0.5 units to perfectly fit the square.
    /// </summary>
    /// <param name="offset">Position to offset</param>
    /// <returns>Returns a modified version of the given position.</returns>
    private Vector2 OffsetPosition(Vector2 offset)
    {
        // offset the spawn location so that the wall spawns in the right place (on square)
        if (offset.x > 0) offset.x += 0.5f;
        if (offset.y > 0) offset.y += 0.5f;
        if (offset.x <= 0) offset.x -= 0.5f;
        if (offset.y <= 0) offset.y -= 0.5f;
        return offset;
    }
    private Vector2 GenerateLocalPosition(int rootStage)
    {
        return new Vector2((int)Random.Range(-stageList[rootStage].GetExtentX(), stageList[rootStage].GetExtentX()), (int)Random.Range(-stageList[rootStage].GetExtentY(), stageList[rootStage].GetExtentY()));
    }
    private int GenerateRandomRootStage()
    {
        return Random.Range(0, stageList.Count);
    }
    private SpawnToolkit InitializeNewToolkit()
    {
        SpawnToolkit newToolkit = new SpawnToolkit();
        newToolkit.root = 0;
        newToolkit.rootStage = null;
        newToolkit.localSpawnCoords = Vector2.zero;
        newToolkit.globalSpawnCoords = Vector2.zero;
        newToolkit.spawnAttempts = 0;
        newToolkit.spawnReady = false;
        newToolkit.stageCheck = null;

        return newToolkit;
    }
}