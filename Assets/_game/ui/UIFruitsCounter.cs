using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wakaman.UI
{
    public class UIFruitsCounter : MonoBehaviour
    {
        [SerializeField] private Image[] fruitsSprites;
        [SerializeField] private Sprite[] sprites;

        private void Start()
        {
            GameEvents.onClearScreen += OnClearScreen;
            GameEvents.onFruitsChange += OnFruitsChange;
            GameEvents.onRoundStart += () => { OnFruitsChange(Game.Fruits); };
        }

        private void OnClearScreen()
        {
            for (int i = 0; i < fruitsSprites.Length; i++)
                fruitsSprites[i].enabled = false;
        }

        private void OnFruitsChange(List<FruitType> fruits)
        {
            for (int i = fruitsSprites.Length-1; i >= 0; i--)
            {
                bool enabled = i < fruits.Count;
                fruitsSprites[i].enabled = enabled;
                if(enabled)
                    fruitsSprites[i].sprite = GetFruitSprite(fruits[fruits.Count - i - 1]);
            }
        }

        private Sprite GetFruitSprite(FruitType type)
        {
            int index = (int)type - 1;
            if (index >= 0 && index < sprites.Length)
                return sprites[index];
            else
            {
                Debug.LogErrorFormat("Fruit sprite not defined for FruitType {0}", type);
                return null;
            }
        }
    }
}