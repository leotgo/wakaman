using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Wakaman/Tiles/Pellet")]
public class CollectibleTile : TileBase
{
    public enum CollectibleType
    {
        Pellet,
        PowerPellet,
        Fruit
    }
    [SerializeField] private CollectibleType type;
    public CollectibleType Type {
        get => type;
    }
    public Sprite sprite;
    private Vector3Int location;
    private ITilemap tilemap;

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.colliderType = Tile.ColliderType.Grid;
    }

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
    {
        this.tilemap = tilemap;
        this.location = location;
        return true;
    }
}
