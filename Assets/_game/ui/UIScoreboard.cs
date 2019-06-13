using UnityEngine;
using UnityEngine.UI;

namespace Wakaman.UI
{
    public class UIScoreboard : MonoBehaviour
    {
        // Component Refs
        [SerializeField] private Text scoreText;
        [SerializeField] private Text highScoreText;

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            GameEvents.onClearScreen += OnClearScreen;
            GameEvents.onRoundStart += OnRoundStart;
            GameEvents.onScoreChange += UpdateScoreText;
            GameEvents.onHighScoreChange += UpdateHighScoreText;
            GameEvents.onRoundStart += () => {
                UpdateScoreText(Game.Score);
                UpdateHighScoreText(Game.HighScore);
            };
        }

        // -------------------------- //
        // UI Callbacks
        // -------------------------- //

        public void OnClearScreen()
        {
            scoreText.enabled = false;
            highScoreText.enabled = false;
        }

        public void OnRoundStart()
        {
            scoreText.enabled = true;
            highScoreText.enabled = true;
        }

        public void UpdateScoreText(int newScore)
        {
            if(scoreText)
                scoreText.text = newScore.ToString();
        }

        public void UpdateHighScoreText(int newHighScore)
        {
            if (highScoreText)
                highScoreText.text = newHighScore.ToString();
        }

        public void UpdateHighscoreText(int newHighScore)
        {
            if(highScoreText)
                highScoreText.text = newHighScore.ToString();
        }
    }
}
