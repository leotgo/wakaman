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
            GameEvents.onScoreChange += UpdateScoreText;
        }

        // -------------------------- //
        // UI Callbacks
        // -------------------------- //

        public void UpdateScoreText(int newScore)
        {
            if(scoreText)
                scoreText.text = newScore.ToString();
        }

        public void UpdateHighscoreText(int newHighScore)
        {
            if(highScoreText)
                highScoreText.text = newHighScore.ToString();
        }
    }
}
