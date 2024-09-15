using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Flamenccio.Powerup;
using Flamenccio.HUD;
using Flamenccio.Effects.Visual;
using Flamenccio.Effects;
using UnityEngine.InputSystem;
using Flamenccio.DataHandling;
using System.Collections.Generic;
using System;
using Flamenccio.Utility;

namespace Flamenccio.Core
{
    /// <summary>
    /// Keeps track of the current game state and updates other classes accordingly.
    /// </summary>
    public class GameState : MonoBehaviour
    {
        public static bool Paused { get; private set; }

        // parameters
        private int progress = 0;
        private int difficulty = 0;
        private int waveSpawnAmount = 1;

        // constants
        private const float MAX_TIME_INCREASE = 0.5f;
        private const float BASE_TIME = 10.0f;
        private const float INCREASE_WALL_FREQUENCY = 0.25f;
        private const float MAX_WALL_FREQUENCY = 1.0f;
        private const int MIN_LEVEL_WALL_UPGRADE = 8; // the minimum level required for level 2 walls to start spawning
        private const float CHANCE_WALL_UPGRADE = 0.5f;
        private const int MIN_LEVEL_ENEMY_SPAWN = 1;
        private const int MIN_LEVEL_PORTAL_SPAWN = 6;

        // timers
        private float maxTime = BASE_TIME;
        private float mainTimer;
        private float wallFrequency = 3.0f;
        private float wallTimer = 0.0f;

        // other necessary classes
        private Spawner spawnControl;
        private GameObject heart;
        [SerializeField] private GoalArrowControl goalArrow;
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
            spawnControl.SpawnStar();

            // subscribe to events
            GameEventManager.OnStarCollect += (x) => CollectStar(Convert.ToInt32(x.Value));
            GameEventManager.OnMiniStarCollect += (x) => CollectMiniStar(Convert.ToInt32(x.Value));
            GameEventManager.OnPlayerHit += (x) => RemoveLife(Convert.ToInt32(x.Value));
            GameEventManager.OnHeartCollect += (x) => ReplenishLife(Convert.ToInt32(x.Value));
            GameEventManager.OnItemBoxCollect += (_) => CollectItemBox();
            GameEventManager.EquipWeapon += (_) =>
            {
                InputManager.Instance.ChangeActionMap(InputManager.ControlActionMap.Game);
                SetPauseState(false);
            };
        }

        private void Awake()
        {
            mainTimer = maxTime;
            spawnControl = gameObject.GetComponent<Spawner>();
            Paused = false;
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
            FloatingTextManager.Instance.DisplayAmmoText(addPoints);
            FloatingTextManager.Instance.DisplayText($"+{addPoints}", PlayerMotion.Instance.PlayerPosition, Color.yellow, 1.0f, 25.0f, FloatingTextControl.TextAnimation.Fade, FloatingTextControl.TextAnimation.Rise, true);

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
            ReplenishTimer();
            GameObject star = spawnControl.SpawnStar();
            goalArrow.PointAt(star);
            AddPoints(total); // add points to current points
        }

        public void CollectMiniStar(int value)
        {
            ReplenishTimer(0.5f);
            AddKillPoint(value);
            EffectManager.Instance.SpawnCollectedStarShard(PlayerMotion.Instance.PlayerPosition, PlayerMotion.Instance.PlayerTransform);
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

            if (difficulty >= MIN_LEVEL_PORTAL_SPAWN)
            {
                spawnControl.SpawnPortal();
            }

            if (wallFrequency > MAX_WALL_FREQUENCY) // increase wall frequency
            {
                wallFrequency -= INCREASE_WALL_FREQUENCY;
            }

            waveSpawnAmount = EnemyWaveLevel();
            maxTime += MAX_TIME_INCREASE; // increase the maximum time

            if (progress >= DifficultyCurve(difficulty + 1)) // level up again, if necessary
            {
                LevelUp();
            }

            spawnControl.SpawnStage(); // spawn another stage

            GameEventManager.OnLevelUp(GameEventManager.CreateGameEvent(difficulty, PlayerMotion.Instance.transform));
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
            CameraEffects.Instance.ScreenShake(CameraEffects.ScreenShakeIntensity.Weak, transform.position);
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

            FloatingTextManager.Instance.DisplayHealthText(life);
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
                int wallLevel = (difficulty >= MIN_LEVEL_WALL_UPGRADE && UnityEngine.Random.Range(0f, 1f) >= CHANCE_WALL_UPGRADE) ? 2 : 1;

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

            int r = UnityEngine.Random.Range(0, 3); // spawns an additional 0 to 2 more enemies randomly
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
            GameEventManager.ClearAllEvents();
            UIEventManager.ClearAllEvents();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Toggles pause function
        /// </summary>
        private void TogglePause()
        {
            SetPauseState(!Paused);
        }

        private void SetPauseState(bool pause)
        {
            Paused = pause;
            Time.timeScale = Paused ? 0.0f : 1.0f; // TODO restore time scale before pause (instead of just 1)
        }

        private void CollectItemBox()
        {
            SetPauseState(true);
            List<GameObject> randomWeapons = new();
            WeaponLootTable weaponLoot = new();
            
            for (int i = 0; i < 3; i++)
            {
                randomWeapons.Add(weaponLoot.GetRandomWeapon(0f, randomWeapons));
            }

            InputManager.Instance.ChangeActionMap(InputManager.ControlActionMap.Menu);
            UIEventManager.DisplayWeapons(UIEventManager.CreateUIMessage(randomWeapons, randomWeapons.GetType()));
        }
    }
}