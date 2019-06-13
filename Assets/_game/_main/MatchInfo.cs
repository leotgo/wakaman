using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.AI;

namespace Wakaman
{
    public class MatchInfo
    {
        // Match info
        private int stage;
        private int lives;
        private int score;
        private int highscore;
        private List<FruitType> fruits;
        private int consumedPellets;

        public float roundTime;
        public float timeSinceSpawn;

        // Accessors
        public int Stage {
            get => stage;
        }
        public int Score {
            get => score;
            set {
                int oldScore = score;
                score = value;
                GameEvents.ScoreChange(score);
                if (score > highscore)
                    Highscore = score;

                if (oldScore / 10000 < value / 10000)
                {
                    if (Lives < 4)
                    {
                        Lives++;
                        GameEvents.ExtraLife();
                    }
                    else
                        Score += 5000;
                }
            }
        }
        public int Highscore {
            get => highscore;
            set {
                highscore = value;
                GameEvents.HighScoreChange(value);
            }
        }
        public int Lives {
            get => lives;
            set {
                lives = value;
                GameEvents.LivesChange(lives);
            }
        }
        public List<FruitType> Fruits {
            get => fruits;
        }
        public float ElapsedTime {
            get => roundTime;
        }

        public int ConsumedPellets {
            get { return consumedPellets; }
        }

        public int RemainingPellets {
            get { return Mathf.Max(0, MapInfo.TotalPellets - consumedPellets); }
        }

        // -------------------------- //
        // Constructors
        // -------------------------- //

        public MatchInfo()
        {
            ResetMatchInfo();
            ResetRoundInfo();
        }

        // -------------------------- //
        // Match management
        // -------------------------- //

        public void ResetMatchInfo()
        {
            if (PlayerPrefs.HasKey(Game.PLAYERPREFS_HIGHSCORE_KEY))
            {
                Highscore = PlayerPrefs.GetInt(Game.PLAYERPREFS_HIGHSCORE_KEY);
            }
            else
            {
                Highscore = 0;
                PlayerPrefs.SetInt(Game.PLAYERPREFS_HIGHSCORE_KEY, 0);
            }
            stage = 0;
            Score = 0;
            Lives = 2;
            fruits = new List<FruitType>();
        }

        public void ResetRoundInfo()
        {
            consumedPellets = 0;
            roundTime = 0f;
            timeSinceSpawn = 0f;
        }

        public void AddNewFruit(FruitType fruitType)
        {
            if (!fruits.Contains(fruitType))
            {
                fruits.Add(fruitType);
                GameEvents.FruitsChange(fruits);
            }
        }

        public void AddPellet()
        {
            if(consumedPellets < MapInfo.TotalPellets)
                consumedPellets++;
        }

        public void NextStage()
        {
            stage++;
            GameEvents.StageChange(stage);
        }
    }
}

