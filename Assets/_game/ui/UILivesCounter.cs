using UnityEngine;
using UnityEngine.UI;

namespace Wakaman
{
    public class UILivesCounter : MonoBehaviour
    {
        [SerializeField] private Image[] livesSprites;

        private void Start()
        {
            GameEvents.onLivesChange += OnLivesChange;
        }

        private void OnLivesChange(int lives)
        {
            for (int i = 0; i < livesSprites.Length; i++)
                    if(i < livesSprites.Length)
                        livesSprites[i].enabled = i < lives;
        }
    }
}
