using System;
using System.Collections.Generic;

namespace Wakaman
{
    public class GameEvents
    {
        // Game Events
        public static event Action onMatchStart;
        public static event Action onRoundStart;
        public static event Action onFinishLevel;
        public static event Action onGameOver;
        public static event OnCollect onCollect;
        public static event OnCollectFruit onCollectFruit;
        public static event OnEatGhost onEatGhost;
        public static event OnScoreChange onScoreChange;
        public static event OnHighScoreChange onHighScoreChange;
        public static event OnLivesChange onLivesChange;
        public static event OnStageChange onStageChange;
        public static event OnFruitsChange onFruitsChange;
        public static event Action onDeath;
        public static event Action onClearScreen;
        public static event Action onExtraLife;
        public static event Action onStartPowerPellet;
        public static event Action onEndPowerPellet;
        public static event Action onGhostEndRetreat;

        // Event delegates
        public delegate void OnCollect(CollectibleType type);
        public delegate void OnCollectFruit(FruitType type);
        public delegate void OnEatGhost();
        public delegate void OnScoreChange(int newScore);
        public delegate void OnHighScoreChange(int newHighScore);
        public delegate void OnLivesChange(int newValue);
        public delegate void OnStageChange(int newStage);
        public delegate void OnFruitsChange(List<FruitType> fruits);

        // -------------------------- //
        // Event Invokers
        // -------------------------- //

        public static void MatchStart()
        {
            onMatchStart?.Invoke();
        }

        public static void RoundStart()
        {
            onRoundStart?.Invoke();
        }

        public static void FinishLevel()
        {
            onFinishLevel?.Invoke();
        }

        public static void GameOver()
        {
            onGameOver?.Invoke();
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

        public static void HighScoreChange(int newHighScore)
        {
            onHighScoreChange?.Invoke(newHighScore);
        }

        public static void LivesChange(int newValue)
        {
            onLivesChange?.Invoke(newValue);
        }

        public static void StageChange(int newValue)
        {
            onStageChange?.Invoke(newValue);
        }

        public static void FruitsChange(List<FruitType> fruits)
        {
            onFruitsChange?.Invoke(fruits);
        }

        public static void Death()
        {
            onDeath?.Invoke();
        }
        public static void ClearScreen()
        {
            onClearScreen?.Invoke();
        }

        public static void ExtraLife()
        {
            onExtraLife?.Invoke();
        }

        public static void StartPowerPellet()
        {
            onStartPowerPellet?.Invoke();
        }

        public static void EndPowerPellet()
        {
            onEndPowerPellet?.Invoke();
        }

        public static void GhostEndRetreat()
        {
            onGhostEndRetreat?.Invoke();
        }

        // -------------------------- //
        // Helpers
        // -------------------------- //

        public static void ClearAllEvents()
        {
            if (onMatchStart != null)
                foreach (Delegate d in onMatchStart.GetInvocationList())
                    onMatchStart -= (Action)d;

            if (onRoundStart != null)
                foreach (Delegate d in onRoundStart.GetInvocationList())
                    onRoundStart -= (Action)d;

            if (onFinishLevel != null)
                foreach (Delegate d in onFinishLevel.GetInvocationList())
                    onFinishLevel -= (Action)d;

            if (onGameOver != null)
                foreach (Delegate d in onGameOver.GetInvocationList())
                    onGameOver -= (Action)d;

            if (onCollect != null)
                foreach (Delegate d in onCollect.GetInvocationList())
                    onCollect -= (OnCollect)d;

            if (onCollectFruit != null)
                foreach (Delegate d in onCollectFruit.GetInvocationList())
                    onCollectFruit -= (OnCollectFruit)d;

            if (onEatGhost != null)
                foreach (Delegate d in onEatGhost.GetInvocationList())
                    onEatGhost -= (OnEatGhost)d;

            if (onScoreChange != null)
                foreach (Delegate d in onScoreChange.GetInvocationList())
                    onScoreChange -= (OnScoreChange)d;

            if (onHighScoreChange != null)
                foreach (Delegate d in onHighScoreChange.GetInvocationList())
                    onHighScoreChange -= (OnHighScoreChange)d;

            if (onLivesChange != null)
                foreach (Delegate d in onLivesChange.GetInvocationList())
                    onLivesChange -= (OnLivesChange)d;

            if (onStageChange != null)
                foreach (Delegate d in onStageChange.GetInvocationList())
                    onStageChange -= (OnStageChange)d;

            if (onFruitsChange != null)
                foreach (Delegate d in onFruitsChange.GetInvocationList())
                    onFruitsChange -= (OnFruitsChange)d;

            if (onDeath != null)
                foreach (Delegate d in onDeath.GetInvocationList())
                    onDeath -= (Action)d;

            if (onClearScreen != null)
                foreach (Delegate d in onClearScreen.GetInvocationList())
                    onClearScreen -= (Action)d;

            if (onExtraLife != null)
                foreach (Delegate d in onExtraLife.GetInvocationList())
                    onExtraLife -= (Action)d;

            if (onStartPowerPellet != null)
                foreach (Delegate d in onStartPowerPellet.GetInvocationList())
                    onStartPowerPellet -= (Action)d;

            if (onEndPowerPellet != null)
                foreach (Delegate d in onEndPowerPellet.GetInvocationList())
                    onEndPowerPellet -= (Action)d;

            if (onGhostEndRetreat != null)
                foreach (Delegate d in onGhostEndRetreat.GetInvocationList())
                    onGhostEndRetreat -= (Action)d;
        }
    }
}
