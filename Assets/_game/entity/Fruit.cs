using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.Entities
{
    public class Fruit : MonoBehaviour, IInteractable
    {
        [Header("Gameplay")]
        [SerializeField] private float dotsConsumedRatioSpawn = 0.5f;

        [Header("Score Renderer refs")]
        [SerializeField] private SpriteRenderer fruitScoreLeft;
        [SerializeField] private SpriteRenderer fruitScoreMiddle;
        [SerializeField] private SpriteRenderer fruitScoreRight;

        [Header("Fruit sprite refs")]
        [SerializeField] private Sprite cherrySprite;
        [SerializeField] private Sprite strawberrySprite;
        [SerializeField] private Sprite orangeSprite;
        [SerializeField] private Sprite appleSprite;
        [SerializeField] private Sprite melonSprite;
        [SerializeField] private Sprite galaxianSprite;
        [SerializeField] private Sprite bellSprite;
        [SerializeField] private Sprite keySprite;

        [Header("Score sprite refs")]
        [SerializeField] private Sprite score100;
        [SerializeField] private Sprite score300;
        [SerializeField] private Sprite score500;
        [SerializeField] private Sprite score700;
        [SerializeField] private Sprite score1000Middle;
        [SerializeField] private Sprite score1000Right;
        [SerializeField] private Sprite score2000Left;
        [SerializeField] private Sprite score2000Middle;
        [SerializeField] private Sprite score2000Right;
        [SerializeField] private Sprite score3000Left;
        [SerializeField] private Sprite score3000Middle;
        [SerializeField] private Sprite score3000Right;
        [SerializeField] private Sprite score5000Left;
        [SerializeField] private Sprite score5000Middle;
        [SerializeField] private Sprite score5000Right;

        private FruitType fruitType = FruitType.None;
        private SpriteRenderer spr;
        private bool isInNewRound;

        private void Start()
        {
            spr = GetComponent<SpriteRenderer>();
            GameEvents.onCollect += OnCollect;
            GameEvents.onRoundStart += OnRoundStart;
            gameObject.SetActive(false);
        }

        private void OnRoundStart()
        {
            isInNewRound = true;
        }

        private void OnCollect(CollectibleType type)
        {
            if (isInNewRound && Game.ConsumedPelletsRatio > dotsConsumedRatioSpawn)
                Spawn();
        }

        public void OnInteract(Player p)
        {
            gameObject.SetActive(false);
            GameEvents.Collect(CollectibleType.Fruit);
            GameEvents.CollectFruit(fruitType);
            UpdateFruitScoreSprite();
            fruitScoreMiddle.gameObject.SetActive(true);
        }

        private void Spawn()
        {
            isInNewRound = false;
            fruitType = Game.GetNextFruitType();
            UpdateFruitSprite();
            gameObject.SetActive(true);
        }

        private void UpdateFruitSprite()
        {
            switch(fruitType)
            {
                case FruitType.None:
                    spr.sprite = null;
                    break;
                case FruitType.Cherry:
                    spr.sprite = cherrySprite;
                    break;
                case FruitType.Strawberry:
                    spr.sprite = strawberrySprite;
                    break;
                case FruitType.Orange:
                    spr.sprite = orangeSprite;
                    break;
                case FruitType.Apple:
                    spr.sprite = appleSprite;
                    break;
                case FruitType.Melon:
                    spr.sprite = melonSprite;
                    break;
                case FruitType.Galaxian:
                    spr.sprite = galaxianSprite;
                    break;
                case FruitType.Bell:
                    spr.sprite = bellSprite;
                    break;
                case FruitType.Key:
                    spr.sprite = keySprite;
                    break;
            }
        }

        private void UpdateFruitScoreSprite()
        {
            switch (fruitType)
            {
                case FruitType.None:
                    fruitScoreLeft.sprite = null;
                    fruitScoreMiddle.sprite = null;
                    fruitScoreRight.sprite = null;
                    break;
                case FruitType.Cherry:
                    fruitScoreLeft.sprite = null;
                    fruitScoreMiddle.sprite = score100;
                    fruitScoreRight.sprite = null;
                    break;
                case FruitType.Strawberry:
                    fruitScoreLeft.sprite = null;
                    fruitScoreMiddle.sprite = score300;
                    fruitScoreRight.sprite = null;
                    break;
                case FruitType.Orange:
                    fruitScoreLeft.sprite = null;
                    fruitScoreMiddle.sprite = score500;
                    fruitScoreRight.sprite = null;
                    break;
                case FruitType.Apple:
                    fruitScoreLeft.sprite = null;
                    fruitScoreMiddle.sprite = score700;
                    fruitScoreRight.sprite = null;
                    break;
                case FruitType.Melon:
                    fruitScoreLeft.sprite = null;
                    fruitScoreMiddle.sprite = score1000Middle;
                    fruitScoreRight.sprite = score1000Right;
                    break;
                case FruitType.Galaxian:
                    fruitScoreLeft.sprite = score2000Left;
                    fruitScoreMiddle.sprite = score2000Middle;
                    fruitScoreRight.sprite = score2000Right;
                    break;
                case FruitType.Bell:
                    fruitScoreLeft.sprite = score3000Left;
                    fruitScoreMiddle.sprite = score3000Middle;
                    fruitScoreRight.sprite = score3000Right;
                    break;
                case FruitType.Key:
                    fruitScoreLeft.sprite = score5000Left;
                    fruitScoreMiddle.sprite = score5000Middle;
                    fruitScoreRight.sprite = score5000Right;
                    break;
            }
        }
    }
}
