using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman.Entities
{
    [CreateAssetMenu(menuName = "Wakaman/Tiles/Pellet")]
    public class CollectibleTile : TileBase
    {
        [SerializeField] private CollectibleType type = CollectibleType.Pellet;
        public CollectibleType Type { get => type; }

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
}