using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman
{
    public class GameEvents
    {
        // Game Events
        public static event OnMatchStart onMatchStart;
        public static event OnCollect onCollect;
        public static event OnCollectFruit onCollectFruit;
        public static event OnEatGhost onEatGhost;
        public static event OnScoreChange onScoreChange;
        public static event OnLivesChange onLivesChange;
        public static event OnFruitsChange onFruitsChange;
        public static event OnDeath onDeath;

        // Event delegates
        public delegate void OnMatchStart();
        public delegate void OnCollect(CollectibleType type);
        public delegate void OnCollectFruit(FruitType type);
        public delegate void OnEatGhost();
        public delegate void OnScoreChange(int newScore);
        public delegate void OnLivesChange(int newValue);
        public delegate void OnFruitsChange(List<FruitType> fruits);
        public delegate void OnDeath();

        // -------------------------- //
        // Event Invokers
        // -------------------------- //

        public static void MatchStart()
        {
            onMatchStart?.Invoke();
        }

        public static void Collect(CollectibleType collectibleType)
        {
            onCollect?.Invoke(collectibleType);
        }

        public static void CollectFruit(FruitType type)
        {
            onCollectFruit?.Invoke(type);
        }

        public static void EatGhost()
        {
            onEatGhost?.Invoke();
        }

        public static void ScoreChange(int newScore)
        {
            onScoreChange?.Invoke(newScore);
        }

        public static void LivesChange(int newValue)
        {
            onLivesChange?.Invoke(newValue);
        }

        public static void FruitsChange(List<FruitType> fruits)
        {
            onFruitsChange?.Invoke(fruits);
        }

        public static void Death()
        {
            onDeath?.Invoke();
        }
    }
}
