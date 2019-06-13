using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wakaman.UI
{
    public class UIGameOverText : MonoBehaviour
    {
        [SerializeField] private Text[] texts;

        private void Start()
        {
            GameEvents.onMatchStart += () => { ShowTexts(false); };
            GameEvents.onRoundStart += () => { ShowTexts(false); };
            GameEvents.onClearScreen += () => { ShowTexts(false); };
            GameEvents.onGameOver += () => { ShowTexts(true); };
        }

        private void ShowTexts(bool show)
        {
            for (int i = 0; i < texts.Length; i++)
                texts[i].enabled = show;
        }

    }
}
