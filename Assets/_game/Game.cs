using UnityEngine;
using UnityEngine.Tilemaps;
using Wakaman.Entities;

namespace Wakaman
{
    // Game:
    // - Handles game state, scene transition and match info
    // - Initialized once at bootup scene (scene index 0)
    public class Game : MonoBehaviour
    {
        // Singleton accessor
        private static Game instance = null;

        // Match info
        private Tilemap tilemap;
        private int level;
        private int score;

        // Game Events
        public static event OnCollect onCollect;
        public static event OnScoreChange onScoreChange;
        public static event OnDeath onDeath;

        // Event delegates
        public delegate void OnCollect(CollectibleType type);
        public delegate void OnScoreChange(int newScore);
        public delegate void OnDeath();

        // -------------------------- //
        // Monobehaviour
        // -------------------------- //

        private void Start()
        {
            instance = this;
            DontDestroyOnLoad(this);
            tilemap = FindObjectOfType<Tilemap>();
            score = 0;
            level = 1;
        }

        // -------------------------- //
        // Events
        // -------------------------- //

        public static void OnCollectItem(CollectibleType type)
        {
            switch (type)
            {
                case CollectibleType.Pellet:
                    instance.score += GameDefs.SCORE_PELLET;
                    break;
                case CollectibleType.PowerPellet:
                    instance.score += GameDefs.SCORE_POWERPELLET;
                    break;
                case CollectibleType.Fruit:
                    instance.score += GameDefs.SCORE_FRUIT * instance.level;
                    break;
            }
            onCollect?.Invoke(type);
            onScoreChange?.Invoke(instance.score);
        }

        public static void Death()
        {
            onDeath?.Invoke();
        }

        // -------------------------- //
        // Helper methods
        // -------------------------- //

        public static Tilemap GetTilemap()
        {
            Debug.Assert(instance != null);
            Debug.Assert(instance.tilemap != null);
            return instance.tilemap;
        }
    }
}