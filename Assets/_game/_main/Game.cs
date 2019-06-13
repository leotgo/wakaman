using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Wakaman.AI;
using Wakaman.Entities;

namespace Wakaman
{
    // Game:
    // - Handles game states and scene transition 
    // - Static access point for general game variables
    // - Receives and dispatches main game control messages
    public class Game : MonoBehaviour
    {
        // Singleton accessor
        private static Game instance = null;

        // Consts
        public const float DEATH_STUTTER_TIME = 1f;
        public const float EAT_GHOST_STUTTER_TIME = 0.5f;
        public const float MATCH_START_TIME = 1.5f;
        public const float ROUND_START_TIME = 3f;
        public const float RESPAWN_FREEZE_TIME = 2f;
        public const float FINISH_LEVEL_FLASH_TIME = 3f;
        public const float GAME_OVER_TEXT_TIME = 4f;
        public const string PLAYERPREFS_HIGHSCORE_KEY = "g_HighScore";
        public static readonly int[] GhostScore = new int[4]{ 200, 400, 800, 1600 };

        // Game definitions
        [SerializeField] private GameDefs gameDefs;

        // Runtime
        private MatchInfo matchInfo;
        private float powerPelletTime;
        private int powerPelletEatenGhosts;
        private bool isInPowerPelletMode;
        public bool isGameOver;

        // Accessors
        public static int Stage {
            get => instance.matchInfo.Stage;
        }
        public static int Score {
            get => instance.matchInfo.Score;
        }
        public static int HighScore {
            get => instance.matchInfo.Highscore;
        }
        public static int Lives {
            get => instance.matchInfo.Lives;
        }
        public static List<FruitType> Fruits {
            get => instance.matchInfo.Fruits;
        }
        public static float RoundTime {
            get { return instance.matchInfo.ElapsedTime; }
        }
        public static float TimeSinceSpawn {
            get { return instance.matchInfo.timeSinceSpawn; }
        }
        public static int ConsumedPellets {
            get { return instance.matchInfo.ConsumedPellets; }
        }
        public static int RemainingPellets {
            get { return instance.matchInfo.RemainingPellets; }
        }
        public static float ConsumedPelletsRatio {
            get { return (float)ConsumedPellets / (float)MapInfo.TotalPellets; }
        }
        public static float RemainingPelletsRatio {
            get { return (float)RemainingPellets / (float)MapInfo.TotalPellets; }
        }
        public static bool IsInPowerPelletMode {
            get => instance.isInPowerPelletMode;
        }
        public static bool IsGameOver {
            get => instance.isGameOver;
            set {
                instance.isGameOver = value;
            }
        }
        public static int PowerPelletsEatenGhosts {
            get => instance.powerPelletEatenGhosts;
        }

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            GameEvents.ClearAllEvents();
            instance = this;
            matchInfo = new MatchInfo();
            isGameOver = false;

            GameEvents.onCollect += OnCollectItem;
            GameEvents.onCollectFruit += OnCollectFruit;
            GameEvents.onDeath += OnDeath;
            GameEvents.onStartPowerPellet += OnStartPowerPellet;
            GameEvents.onEatGhost += OnEatGhost;

            StartNewMatch();
        }

        private void Update()
        {
            matchInfo.roundTime += Time.deltaTime;
            if (Player.instance.IsReady)
                matchInfo.timeSinceSpawn += Time.deltaTime;
            else
                matchInfo.timeSinceSpawn = 0f;

            if (isInPowerPelletMode)
            {
                powerPelletTime += Time.deltaTime;
                if (powerPelletTime > GetGhostScaredTime())
                {
                    powerPelletTime = 0f;
                    isInPowerPelletMode = false;
                    GameEvents.EndPowerPellet();
                }
            }

            // Cheat codes used for debugging
            // Add 1000 score cheatcode
            if (Input.GetKeyDown(KeyCode.F11))
            {
                matchInfo.Score += 1000;
            }
            // Win level cheatcode
            if (Input.GetKeyDown(KeyCode.F12))
            {
                int remainingCache = RemainingPellets;
                for (int i = 0; i < remainingCache; i++)
                    OnCollectItem(CollectibleType.Pellet);
            }
        }

        // -------------------------- //
        // Game Management
        // -------------------------- //

        private void StartNewMatch()
        {
            StartCoroutine(NewMatchRoutine());
        }

        private IEnumerator NewMatchRoutine()
        {
            yield return new WaitForEndOfFrame();
            instance.matchInfo.ResetMatchInfo();
            instance.matchInfo.ResetRoundInfo();
            Time.timeScale = 0f;
            GameEvents.ClearScreen();
            GameEvents.MatchStart();
            yield return new WaitForSecondsRealtime(MATCH_START_TIME);
            StartNewRound();
        }

        private void StartNewRound()
        {
            instance.matchInfo.ResetRoundInfo();
            StartCoroutine(RoundStartRoutine());
        }

        private IEnumerator RoundStartRoutine()
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(ROUND_START_TIME);
            MapInfo.RestoreMapPellets();
            matchInfo.roundTime = 0f;
            powerPelletEatenGhosts = 0;
            powerPelletTime = 0f;
            isInPowerPelletMode = false;
            GameEvents.RoundStart();
            yield return new WaitForSecondsRealtime(RESPAWN_FREEZE_TIME);
            Time.timeScale = 1f;
        }

        private void FinishLevel()
        {
            StopAllCoroutines();
            if (IsInPowerPelletMode)
            {
                isInPowerPelletMode = false;
                GameEvents.EndPowerPellet();
            }
            GameEvents.FinishLevel();
            matchInfo.NextStage();
            StartNewRound();
        }

        private void GameOver()
        {
            StartCoroutine(GameOverRoutine());

        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSecondsRealtime(DEATH_STUTTER_TIME);
            GameEvents.ClearScreen();
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(ROUND_START_TIME);
            GameEvents.GameOver();
            yield return new WaitForSecondsRealtime(GAME_OVER_TEXT_TIME);
            PlayerPrefs.SetInt(PLAYERPREFS_HIGHSCORE_KEY, matchInfo.Highscore);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }

        private IEnumerator PowerPelletEffectRoutine()
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.5f);
            Time.timeScale = 1f;
            powerPelletTime = 0f;
            isInPowerPelletMode = true;
            GameEvents.StartPowerPellet();
        }

        // -------------------------- //
        // Events
        // -------------------------- //

        public void OnCollectItem(CollectibleType type)
        {
            int oldScore = Score;
            matchInfo.Score += GetCollectibleScore(type);
            if (type == CollectibleType.Pellet || type == CollectibleType.PowerPellet)
            {
                matchInfo.AddPellet();
                if (RemainingPellets == 0)
                    FinishLevel();
            }
            if (type == CollectibleType.PowerPellet)
            {
                StartCoroutine(PowerPelletEffectRoutine());
            }
        }

        public void OnCollectFruit(FruitType type)
        {
            matchInfo.AddNewFruit(type);
        }

        public void OnEatGhost()
        {
            powerPelletEatenGhosts++;
            matchInfo.Score += GetGhostEatenScore();
        }

        public void OnDeath()
        {
            if (Lives == 0)
                GameOver();
            else
            {
                matchInfo.Lives--;
                matchInfo.timeSinceSpawn = 0f;
            }
        }

        private void OnStartPowerPellet()
        {
            powerPelletEatenGhosts = 0;
            powerPelletTime = 0f;
            isInPowerPelletMode = true;
        }

        // -------------------------- //
        // Helpers
        // -------------------------- //

        public static FruitType GetNextFruitType()
        {
            return instance.gameDefs.GetFruitType(instance.matchInfo.Stage);
        }

        public static float GetGhostScaredTime()
        {
            return instance.gameDefs.GetGhostBlueTime(instance.matchInfo.Stage);
        }

        public static bool IsNearLevelEnd()
        {
            return instance.matchInfo.RemainingPellets < 15;
        }

        public static int GetCollectibleScore(CollectibleType type)
        {
            switch (type)
            {
                case CollectibleType.Pellet:
                    return instance.gameDefs.GetPelletScore(instance.matchInfo.Stage);
                case CollectibleType.PowerPellet:
                    return instance.gameDefs.GetPowerPelletScore(instance.matchInfo.Stage);
                case CollectibleType.Fruit:
                    return instance.gameDefs.GetFruitScore(instance.matchInfo.Stage);
                default:
                    return 0;
            }
        }

        public static int GetGhostEatenScore()
        {
            int index = instance.powerPelletEatenGhosts - 1;
            if (GhostScore != null && GhostScore.Length != 0)
            {
                if (index >= GhostScore.Length)
                    index = GhostScore.Length - 1;
                else if (index <= 0)
                    index = 0;
                return GhostScore[index];
            }
            else
                return 0;
        }
    }
}