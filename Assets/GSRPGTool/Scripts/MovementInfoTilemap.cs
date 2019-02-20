using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGTool
{
    [RequireComponent(typeof(Tilemap))]
    public class MovementInfoTilemap : MonoBehaviour
    {
        public Tilemap Tilemap { get; private set; }

        void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
        }

        public bool CanMove(Vector2Int position)
        {
            var tile = Tilemap.GetTile(new Vector3Int(position.x, position.y, 0)) as MovementInfoTile;
            return tile == null || tile.canMove;
        }

        public void SetMovable(Vector2Int position, bool canMove)
        {
            var newTileInfo = ScriptableObject.CreateInstance<MovementInfoTile>();
            newTileInfo.canMove = canMove;
            Tilemap.SetTile(new Vector3Int(position.x, position.y, 0), newTileInfo);
        }
    }
}
