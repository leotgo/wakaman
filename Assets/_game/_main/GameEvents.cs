using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman
{
    public class GameEvents
    {
        // Game Events
        public static event OnCollect onCollect;
        public static event OnScoreChange onScoreChange;
        public static event OnLivesChange onLivesChange;
        public static event OnFruitsChange onFruitsChange;
        public static event OnDeath onDeath;

        // Event delegates
        public delegate void OnCollect(CollectibleType type);
        public delegate void OnScoreChange(int newScore);
        public delegate void OnLivesChange(int newValue);
        public delegate void OnFruitsChange(List<FruitType> fruits);
        public delegate void OnDeath();

        // -------------------------- //
        // Event Invokers
        // -------------------------- //

        public static void Collect(CollectibleType collectibleType)
        {
            onCollect?.Invoke(collectibleType);
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
