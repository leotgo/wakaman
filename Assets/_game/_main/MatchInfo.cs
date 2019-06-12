using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman
{
    public class MatchInfo
    {
        // Match info
        private int stage;
        private int lives;
        private int score;
        private List<FruitType> fruits;

        public float elapsedTime;

        private int totalPellets;
        private int remainingPellets;

        // Accessors
        public int Stage {
            get => stage;
        }
        public int Score {
            get => score;
            set {
                score = value;
                GameEvents.ScoreChange(score);
            }
        }
        public int Lives {
            get => lives;
            set {
                lives = value;
                GameEvents.LivesChange(lives);
            }
        }
        public float ElapsedTime {
            get => elapsedTime;
        }
        public int TotalPellets {
            get { return totalPellets; }
        }
        public int RemainingPellets {
            get { return remainingPellets; }
        }
        public int ConsumedPellets {
            get { return totalPellets - remainingPellets; }
        }

        // -------------------------- //
        // Constructors
        // -------------------------- //

        public MatchInfo()
        {
            stage = 0;
            Score = 0;
            Lives = 2;
            fruits = new List<FruitType>();
            GameEvents.FruitsChange(fruits);
        }

        // -------------------------- //
        // Match management
        // -------------------------- //

        public void AddNewFruit(FruitType fruitType)
        {
            if (!fruits.Contains(fruitType))
            {
                fruits.Add(fruitType);
                GameEvents.FruitsChange(fruits);
            }
        }
    }
}
