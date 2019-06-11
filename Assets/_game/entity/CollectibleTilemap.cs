using UnityEngine;
using UnityEngine.Tilemaps;

namespace Wakaman.Entities
{
    public class CollectibleTilemap : MonoBehaviour, IInteractable
    {
        private Tilemap tilemap;
        private TilemapCollider2D coll;

        private void Start()
        {
            tilemap = GetComponent<Tilemap>();
            coll = GetComponent<TilemapCollider2D>();
        }

        public void OnInteract(Player player)
        {
            var cellPos = tilemap.WorldToCell(player.transform.position);
            var tile = tilemap.GetTile(cellPos);
            if (tile is CollectibleTile)
            {
                var type = (tile as CollectibleTile).Type;
                GameEvents.Collect(type);
                tilemap.SetTile(cellPos, null);
            }
        }
    }
}
