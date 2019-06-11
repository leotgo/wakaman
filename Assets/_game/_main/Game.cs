using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman
{
    // Game:
    // - Handles game states and scene transition 
    public class Game : MonoBehaviour
    {
        // Singleton accessor
        private static Game instance = null;

        private Tilemap tilemap;
        private MatchInfo matchInfo;

        // Game definitions
        [SerializeField] private GameDefs gameDefs;

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            instance = this;
            DontDestroyOnLoad(this);
            tilemap = FindObjectOfType<Tilemap>();
            matchInfo = new MatchInfo();
            GameEvents.onCollect += OnCollectItem;
            GameEvents.onCollectFruit += OnCollectFruit;
        }

        // -------------------------- //
        // Event Callbacks
        // -------------------------- //

        public void OnCollectItem(CollectibleType type)
        {
            switch (type)
            {
                case CollectibleType.Pellet:
                    matchInfo.Score += gameDefs.GetPelletScore(matchInfo.Stage);
                    break;
                case CollectibleType.PowerPellet:
                    matchInfo.Score += gameDefs.GetPowerPelletScore(matchInfo.Stage);
                    break;
                case CollectibleType.Fruit:
                    matchInfo.Score += gameDefs.GetFruitScore(matchInfo.Stage);
                    break;
            }
        }

        public void OnCollectFruit(FruitType type)
        {
            matchInfo.AddNewFruit(type);
        }

        // -------------------------- //
        // Helpers
        // -------------------------- //

        public static Tilemap GetTilemap()
        {
            Debug.Assert(instance != null);
            Debug.Assert(instance.tilemap != null);
            return instance.tilemap;
        }

        public static int GetScore(CollectibleType type, int stage)
        {
            switch(type)
            {
                case CollectibleType.Pellet:
                    return instance.gameDefs.GetPelletScore(stage);
                case CollectibleType.PowerPellet:
                    return instance.gameDefs.GetPowerPelletScore(stage);
                case CollectibleType.Fruit:
                    return instance.gameDefs.GetFruitScore(stage);
                default:
                    return 0;
            }
        }
    }
}