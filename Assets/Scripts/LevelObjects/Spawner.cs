using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Flamenccio.Utility;
using Flamenccio.LevelObject.Stages;
using Flamenccio.LevelObject.Walls;
using System.Security;

namespace Flamenccio.Core
{
    /// <summary>
    /// Manages spawning of objects, enemies, and stages.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        public static Spawner Instance { get; private set; }

        private LayerMask wallLayer; // layer of all walls
        private LayerMask enemyCheckLayers; // obstructing layers when spawning enemies
        private LayerMask wallCheckLayers; // obstructing layers when spawning walls

        private ItemSpawner.Item starshardItem;

        private LevelManager levelManager;
        private ObjectSpawner objectSpawner;
        private ItemSpawner itemSpawner;
        private EntitySpawner entitySpawner;

        private const float MIN_PORTAL_DISTANCE = 32f;
        private const int MAX_PORTAL_SPAWN_ATTEMPTS = 4;
        private const int MAX_ENEMY_SPAWN_ATTEMPTS = 3;
        private const float WALL_SEARCH_RADIUS = 8.0f;
        private const float ENEMY_SPAWN_RADIUS = 2.0f; // the minimum space required between the player and enemy for it (the enemy) to spawn

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            wallLayer = LayerManager.GetLayerMask(Layer.Wall);
            enemyCheckLayers = LayerManager.GetLayerMask(new List<Layer> { Layer.Player, Layer.Enemy, Layer.Wall });
            wallCheckLayers = LayerManager.GetLayerMask(new List<Layer> { Layer.Player, Layer.Wall });

            levelManager = GetComponent<LevelManager>();
            objectSpawner = GetComponent<ObjectSpawner>();
            itemSpawner = GetComponent<ItemSpawner>();
            entitySpawner = GetComponent<EntitySpawner>();
        }

        private void Start()
        {
            starshardItem = itemSpawner.GetItem("starshard");
        }

        public GameObject SpawnEnemy(int difficulty)
        {
            Stage stage;
            Vector2 position;
            var spawnAttemptsRemaining = MAX_ENEMY_SPAWN_ATTEMPTS;
            bool accepted = false;

            do
            {
                spawnAttemptsRemaining--;
                stage = levelManager.GetRandomStage();
                position = AlignPosition(stage.GetGlobalPointInExents());

                if (stage.PointIsInStage(position, true)
                && !PointIsObstructedByLayers(position, enemyCheckLayers))
                {
                    accepted = true;
                }

            } while (spawnAttemptsRemaining > 0 && !accepted);

            if (!accepted) return null;

            Physics2D.OverlapCircleAll(position, ENEMY_SPAWN_RADIUS, wallLayer)
                .ToList()
                .ForEach(x =>
                {
                    if (x.TryGetComponent<Wall>(out var wall))
                    {
                        wall.Die();
                    }
                });

            return entitySpawner.SpawnRandomEnemy(position, difficulty);
        }

        /// <summary>
        /// spawn a star on stage
        /// </summary>
        /// <returns>returns the spanwed star object</returns>
        public GameObject SpawnStar()
        {
            var rootStage = levelManager.GetRandomStage();
            var localPosition = AlignPosition(rootStage.GetLocalPointInStage());

            return itemSpawner.SpawnItem("star", rootStage.transform, localPosition);
        }

        public void SpawnStage()
        {
            Stage rootStage;
            Directions.CardinalValues extendDirection;
            var stageResources = StageResources.Instance;

            do
            {
                rootStage = levelManager.GetRandomStage();
                rootStage.ScanNearbyStages();
                extendDirection = Directions.RandomCardinal();
            } while (!rootStage.LinkableInDirection(extendDirection));

            var availableVariants = stageResources.GetAvailableVariantsInDirection(extendDirection, rootStage.Variant);
            var variant = availableVariants[Random.Range(1, availableVariants.Count)];
            var instance = stageResources.CreateStage(variant);
            levelManager.AddStage(instance, rootStage, extendDirection);
        }

        public void SpawnWall(int wallLevel)
        {
            var stage = levelManager.GetRandomStage();
            Vector2 point = stage.GetGlobalPointInExents();
            var existingWall = Physics2D.OverlapCircle(point, WALL_SEARCH_RADIUS, wallLayer);
            Vector2 localPosition;

            if (existingWall != null)
            {
                Directions.CardinalValues dir = Directions.RandomCardinal();
                localPosition = (Vector2)existingWall.transform.localPosition + Directions.DirectionsToVector2(dir);
            }
            else
            {
                localPosition = GlobalToLocalPosition(point, stage.transform.position);
            }

            localPosition = AlignPosition(localPosition);

            if (!stage.PointIsInStage(localPosition, false)
                || PointIsObstructedByLayers(point, wallCheckLayers))
            {
                return;
            }

            objectSpawner.SpawnWall(stage.transform, localPosition, wallLevel);
        }

        public GameObject SpawnHeart()
        {
            var stage = levelManager.GetRandomStage();
            var point = AlignPosition(stage.GetLocalPointInStage());
            return itemSpawner.SpawnItem("heart", stage.transform, point);
        }

        /// <summary>
        /// Spawns <b>two</b> portals.
        /// </summary>
        public void SpawnPortal()
        {
            if (levelManager.Stages.Count < 2) return;

            Stage stage1;
            Stage stage2;
            Vector2 localPosition1;
            Vector2 localPosition2;
            bool ready = false;
            int spawnAttempts = MAX_PORTAL_SPAWN_ATTEMPTS;

            do
            {
                spawnAttempts--;
                stage1 = levelManager.GetRandomStage();
                stage2 = levelManager.GetRandomStageExcept(stage1);
                localPosition1 = AlignPosition(stage1.GetLocalPointInStage());
                localPosition2 = AlignPosition(stage2.GetLocalPointInStage());

                if (Vector2.Distance(LocalToGlobalPosition(localPosition1, stage1.transform.position), LocalToGlobalPosition(localPosition2, stage2.transform.position)) >= MIN_PORTAL_DISTANCE) ready = true;
            } while (!ready && spawnAttempts > 0);

            if (!ready) return;

            objectSpawner.SpawnPortal(stage1, stage2, localPosition1, localPosition2);
        }

        /// <summary>
        /// Spawns a star shard at a global position.
        /// </summary>
        /// <param name="globalPosition">Global position.</param>
        public void SpawnStarShard(Vector2 globalPosition)
        {
            itemSpawner.SpawnItem(starshardItem, globalPosition);
        }

        /// <summary>
        /// Spawns multiple star shards at a global position.
        /// </summary>
        /// <param name="globalPosition">Global position.</param>
        /// <param name="amount">Amount of shards to spawn.</param>
        public void SpawnStarShard(Vector2 globalPosition, int amount)
        {
            if (amount < 0) return;

            for (int i = 0; i < amount; i++)
            {
                SpawnStarShard(globalPosition);
            }
        }

        /// <summary>
        /// Takes a global position and returns it aligned with a grid.
        /// </summary>
        private Vector2 AlignPosition(Vector2 globalPosition)
        {
            return NormalizePosition(globalPosition);
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

        private bool PointIsObstructedByLayers(Vector2 globalPosition, LayerMask obstructingLayers)
        {
            return Physics2D.OverlapPoint(globalPosition, obstructingLayers) != null;
        }
    }
}