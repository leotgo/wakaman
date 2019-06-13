using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wakaman.Utilities;

namespace Wakaman.UI
{
    public class UIGhostScore : MonoBehaviour
    {
        // Component Refs
        private SpriteRenderer spr;

        [SerializeField] private float showTime = Game.EAT_GHOST_STUTTER_TIME;

        // Sprites
        [SerializeField] private Sprite sprScore200;
        [SerializeField] private Sprite sprScore400;
        [SerializeField] private Sprite sprScore800;
        [SerializeField] private Sprite sprScore1600;

        private void Start()
        {
            spr = GetComponent<SpriteRenderer>();
            spr.enabled = false;
            spr.sprite = null;
        }

        public void Show(bool show)
        {
            int score = Game.GetGhostEatenScore();
            switch(score)
            {
                case 200:
                    spr.sprite = sprScore200;
                    break;
                case 400:
                    spr.sprite = sprScore400;
                    break;
                case 800:
                    spr.sprite = sprScore800;
                    break;
                case 1600:
                    spr.sprite = sprScore1600;
                    break;
                default:
                    spr.sprite = null;
                    break;
            }
            spr.enabled = show;
        }
    }
}
