using UnityEngine;
using UnityEngine.UI;

namespace Wakaman.UI
{
    public class UILivesCounter : MonoBehaviour
    {
        [SerializeField] private Image[] livesSprites;

        private void Start()
        {
            GameEvents.onClearScreen += OnClearScreen;
            GameEvents.onLivesChange += OnLivesChange;
            GameEvents.onRoundStart += () => { OnLivesChange(Game.Lives); };
        }

        private void OnClearScreen()
        {
            for (int i = 0; i < livesSprites.Length; i++)
                livesSprites[i].enabled = false;
        }

        private void OnLivesChange(int lives)
        {
            for (int i = 0; i < livesSprites.Length; i++)
                livesSprites[i].enabled = i < lives;
        }
    }
}
