using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Flamenccio.Powerup;
using Flamenccio.HUD;
using Flamenccio.Effects.Visual;
using Flamenccio.Effects;


namespace Flamenccio.Core
{
    public class GameState : MonoBehaviour
    {
        // this class coordinates all other management classes

        public static GameState Instance { get; private set; }

        // parameters
        private int progress = 0;
        private int difficulty = 0;
        private int waveSpawnAmount = 1;

        // constants
        private const float MAX_TIME_INCREASE = 0.5f;
        private const float BASE_TIME = 10.0f;
        private const float INCREASE_WALL_FREQUENCY = 0.25f;
        private const float MAX_WALL_FREQUENCY = 1.0f;
        private const int MIN_LEVEL_WALL_UPGRADE = 12; // the minimum level required for level 2 walls to start spawning
        private const float CHANCE_WALL_UPGRADE = 0.5f;
        private const int MIN_LEVEL_ENEMY_SPAWN = 0;

        // timers
        private float maxTime = BASE_TIME;
        private float mainTimer = 0.0f;
        private float wallFrequency = 3.0f;
        private float wallTimer = 0.0f;

        // other necessary classes
        private Spawner spawnControl;
        private GameObject heart;
        [SerializeField] private GoalArrowControl goalArrow;
        [SerializeField] private HUDControl hudControl;
        [SerializeField] private CameraControl cameraControl;
        [SerializeField] private PlayerAttributes playerAtt;

        // debug stuff
        [SerializeField] private bool infiniteTime;

        public float Timer
        {
            get
            {
                int temp = Mathf.RoundToInt(mainTimer * 100);
                return temp / 100f;
            }
        }
        public int Level { get => difficulty; }
        public int Progress { get => progress; }

        private void Start()
        {
            Time.timeScale = 1.0f;
        }
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

            mainTimer = maxTime;
            spawnControl = gameObject.GetComponent<Spawner>();
            spawnControl.SpawnStar();
        }
        private void Update()
        {
            MainTimer();
            WallTimer();
        }
        public void AddPoints(int addPoints)
        {
            playerAtt.AddAmmo(addPoints);
            progress += addPoints;
            hudControl.DisplayScoreFlyText(addPoints);

            if (progress >= DifficultyCurve(difficulty + 1)) // level up
            {
                LevelUp();
            }
        }
        public void CollectStar(int value)
        {
            int total = Mathf.FloorToInt(playerAtt.KillPoints * playerAtt.KillPointBonus) + value; // calculate total points gained
            SpawnEnemies(); // spawn more enemies with an additional amount based on the amount of kill points obtained
            ResetKills();
            GameObject star = spawnControl.SpawnStar();
            goalArrow.PointAt(star);
            AddPoints(total); // add points to current points
        }
        public void CollectMiniStar(int value)
        {
            AddKillPoint(value);
            spawnControl.SpawnFlyingStar(PlayerMotion.Instance.PlayerPosition, PlayerMotion.Instance.transform, hudControl.transform);
        }
        public void AddKillPoint(int pt)
        {

            playerAtt.AddKillPoints(pt);
        }
        private void LevelUp()
        {
            if (heart != null) Destroy(heart);

            difficulty++; // increase difficulty
            heart = spawnControl.SpawnHeart();
            spawnControl.SpawnStage(); // spawn another stage

            if (wallFrequency > MAX_WALL_FREQUENCY) // increase wall frequency
            {
                wallFrequency -= INCREASE_WALL_FREQUENCY;
            }

            waveSpawnAmount = EnemyWaveLevel();
            maxTime += MAX_TIME_INCREASE; // increase the maximum time
            hudControl.DisplayLevelUpText(difficulty); // display on HUD

            if (progress >= DifficultyCurve(difficulty + 1)) // level up again, if necessary
            {
                LevelUp();
            }

            cameraControl.IncreaseCameraSize();
        }
        /// <summary>
        /// Remove points from player
        /// </summary>
        /// <param name="points">Amount to remove</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemovePoints(int points)
        {
            return playerAtt.UseAmmo(points);
        }
        public void ReplenishTimer()
        {
            mainTimer = maxTime;
        }
        public void ReplenishTimer(float t)
        {
            mainTimer += t;
        }
        /// <summary>
        /// Remove life from player
        /// </summary>
        /// <param name="life">Amount of life to remove</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveLife(int life)
        {
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, transform.position); // TODO maybe scale intensity with damage taken
            cameraControl.HurtZoom();
            hudControl.DisplayHurtLines();
            playerAtt.ChangeLife(-life);

            if (playerAtt.HP == 0)
            {
                GameOver();
            }

            return true;
        }
        public void ReplenishLife(int life)
        {
            if (!playerAtt.ChangeLife(life))
            {
                AddPoints(3);
                return;
            }

            hudControl.DisplayHealthFlyText(life);
        }

        /// <summary>
        /// Returns the amount of points needed for the next level
        /// </summary>
        /// <param name="nextLevel"></param>
        /// <returns></returns>
        public int DifficultyCurve(int nextLevel)
        {
            return Mathf.CeilToInt(2 * Mathf.Pow(nextLevel, 11f / 5f));
        }
        private void MainTimer()
        {
            if (infiniteTime) return;

            if (mainTimer > 0.0f) mainTimer -= Time.deltaTime;

            if (mainTimer < 0.0f)
            {
                GameOver();
                mainTimer = 0.0f;
            }
        }
        private void WallTimer()
        {
            if (wallTimer >= wallFrequency)
            {
                wallTimer = 0.0f;
                int wallLevel = (difficulty >= MIN_LEVEL_WALL_UPGRADE && Random.Range(0f, 1f) >= CHANCE_WALL_UPGRADE) ? 2 : 1;

                for (int i = 0; i < difficulty + 1; i++)
                {
                    spawnControl.SpawnWall(wallLevel);
                }
            }

            wallTimer += Time.deltaTime;
        }
        /// <summary>
        /// spawn a wave of enemies defined by waveSpawnAmount PLUS an amount based on the amount of killPoints obtained
        /// </summary>
        private void SpawnEnemies()
        {
            if (difficulty < MIN_LEVEL_ENEMY_SPAWN) return; // if level is not high enough, don't do anything

            int kp = playerAtt.KillPoints;

            if (kp > 0) kp = Mathf.FloorToInt(Mathf.Log(kp)); // kill point scaling

            int r = Random.Range(0, 3); // spawns an additional 0 to 2 more enemies randomly
            int enemies = waveSpawnAmount + kp + r; // total amount of enemies to spawn in this wave

            for (int i = 0; i < enemies; i++) // spawn a wave of enemies
            {
                spawnControl.SpawnEnemy(difficulty);
            }
        }
        private int EnemyWaveLevel()
        {
            return Mathf.CeilToInt(Mathf.Sqrt(difficulty) / 2f) + 1;
        }
        public void GameOver()
        {
            StartCoroutine(Reload());
        }
        public void ResetKills()
        {
            playerAtt.UseKillPoints();
        }
        private IEnumerator Reload()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            Time.timeScale = 0.0f;
            yield return new WaitForSecondsRealtime(1.0f);
            Instance = null; // clear instance
            SceneManager.LoadScene(0);
        }
    }
}
