using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wakaman.Utilities;

namespace Wakaman.UI
{
    public class UIReadyText : MonoBehaviour
    {
        [SerializeField] private Text playerOneText;
        [SerializeField] private Text readyText;

        private void Start()
        {
            GameEvents.onMatchStart += OnMatchStart;
            GameEvents.onRoundStart += OnRoundStart;
            GameEvents.onDeath += OnDeath;
        }

        private void OnMatchStart()
        {
            playerOneText.enabled = true;
            readyText.enabled = true;
            this.Delay(Game.MATCH_START_TIME + Game.ROUND_START_TIME, () =>
            {
                playerOneText.enabled = false;
            });
        }

        private void OnRoundStart()
        {
            readyText.enabled = true;
            this.Delay(Game.RESPAWN_FREEZE_TIME, () =>
            {
                readyText.enabled = false;
            });
        }

        private void OnDeath()
        {
            if (!Game.IsGameOver)
            {
                this.Delay(Game.DEATH_STUTTER_TIME + Game.ROUND_START_TIME, () =>
                {
                    readyText.enabled = true;
                    this.Delay(Game.RESPAWN_FREEZE_TIME, () =>
                    {
                        readyText.enabled = false;
                    });
                });
            }
        }
    }
}
