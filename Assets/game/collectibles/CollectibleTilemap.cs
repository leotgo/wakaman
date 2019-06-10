using UnityEngine;
using UnityEngine.Tilemaps;

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
            tilemap.SetTile(cellPos, null);
        }
    }
}
