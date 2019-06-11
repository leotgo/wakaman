using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.Entities
{
    public class Fruit : MonoBehaviour, IInteractable
    {
        [SerializeField] private FruitType fruitType = FruitType.Cherry;

        public void OnInteract(Player p)
        {
            gameObject.SetActive(false);
            GameEvents.Collect(CollectibleType.Fruit);
            GameEvents.CollectFruit(fruitType);
        }
    }
}
