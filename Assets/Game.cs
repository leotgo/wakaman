using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Initialized once at bootup scene (scene index 0)
public class Game : MonoBehaviour
{
    // Singleton
    private static Game instance = null;

    private Tilemap tilemap;

    // Unity Callbacks

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnApplicationLoadScene(int index)
    {
        tilemap = FindObjectOfType<Tilemap>();
    }

    // Utilities

    public static Tilemap GetTilemap()
    {
        Debug.Assert(instance != null);
        Debug.Assert(instance.tilemap != null);
        return instance.tilemap;
    }
}
