using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Initialized once at bootup scene (scene index 0)
public class Game : MonoBehaviour
{
    // Singleton
    private static Game instance = null;

    // Consts
    public const int SCORE_PELLET = 10;
    public const int SCORE_POWERPELLET = 50;

    // Match info
    private Tilemap tilemap;
    public int score;

    // Events
    public delegate void OnCollect(CollectibleTile.CollectibleType type);

    // Unity Callbacks

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);
        tilemap = FindObjectOfType<Tilemap>();

        score = 0;
    }

    // Game

    public static void AddScore(int score)
    {
        instance.score += score;
        Debug.LogFormat("Score: {0}", instance.score);
    }

    // Utilities

    public static Tilemap GetTilemap()
    {
        Debug.Assert(instance != null);
        Debug.Assert(instance.tilemap != null);
        return instance.tilemap;
    }
}
