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

        // -------------------------- //
        // Constructors
        // -------------------------- //

        public MatchInfo()
        {
            stage = 0;
            Score = 0;
            Lives = 2;
            fruits = new List<FruitType>();
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
