using RPGTool.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGTool
{
    [RequireComponent(typeof(Tilemap))]
    [ExecuteInEditMode]
    public class InfoTilemap : MonoBehaviour
    {
        public Tilemap Tilemap { get; private set; }

        private void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
        }

        public void SetTileInfo(Vector2Int position, InfoTile.TileType tileType, bool hasActor)
        {
            Tilemap.SetTile(new Vector3Int(position.x, position.y, 0), InfoTile.GetInfoTile(tileType, hasActor));
        }

        public void SetTileInfo(Vector2Int position, bool hasActor)
        {
            var oldTile = GetTileInfo(position);

            Tilemap.SetTile(new Vector3Int(position.x, position.y, 0),
                InfoTile.GetInfoTile(oldTile != null ? oldTile.tileType : InfoTile.TileType.Ground, hasActor)
            );
        }

        public InfoTile GetTileInfo(Vector2Int position)
        {
            return Tilemap.GetTile<InfoTile>(new Vector3Int(position.x, position.y, 0));
        }
    }
}