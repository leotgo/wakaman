using UnityEngine;
using UnityEngine.UI;

namespace Wakaman.UI
{
    public class UIScoreboard : MonoBehaviour
    {
        // Component Refs
        private Text scoreText;

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            scoreText = GetComponent<Text>();
            Game.onScoreChange += UpdateScoreText;
        }

        // -------------------------- //
        // UI Callbacks
        // -------------------------- //

        public void UpdateScoreText(int newScore)
        {
            scoreText.text = newScore.ToString();
        }
    }
}
