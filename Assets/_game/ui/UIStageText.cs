using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wakaman.UI
{
    public class UIStageText : MonoBehaviour
    {
        [SerializeField] private Text stageText;

        private void Start()
        {
            GameEvents.onClearScreen += OnClearScreen;
            GameEvents.onRoundStart += OnRoundStart;
            GameEvents.onStageChange += UpdateStageText;
            GameEvents.onRoundStart += () => { UpdateStageText(Game.Stage); };
        }

        public void OnClearScreen()
        {
            stageText.enabled = false;
        }

        public void OnRoundStart()
        {
            stageText.enabled = true;
        }

        public void UpdateStageText(int newStage)
        {
            if (stageText)
                stageText.text = (newStage + 1).ToString();
        }
    }
}
