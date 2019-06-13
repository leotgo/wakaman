using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman.Entities
{
    [CreateAssetMenu(menuName = "Wakaman/Tiles/Pellet")]
    public class CollectibleTile : TileBase
    {
        public Sprite[] animSprites;
        public float animSpeed = 1f;
        public float animStart;

        [SerializeField] private CollectibleType type = CollectibleType.Pellet;
        public CollectibleType Type { get => type; }

        private Vector3Int location;
        private ITilemap tilemap;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            if(animSprites != null && animSprites.Length > 0)
                tileData.sprite = animSprites[0];

            tileData.colliderType = Tile.ColliderType.Grid;
        }

        public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
        {
            this.tilemap = tilemap;
            this.location = location;
            return true;
        }

        public override bool GetTileAnimationData(Vector3Int location, ITilemap tileMap, ref TileAnimationData tileAnimationData)
        {
            if (animSprites != null && animSprites.Length > 0)
            {
                tileAnimationData.animatedSprites = animSprites;
                tileAnimationData.animationSpeed = animSpeed;
                tileAnimationData.animationStartTime = animStart;
                return true;
            }
            return false;
        }
    }
}