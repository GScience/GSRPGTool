using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGTool.Tiles
{
    public class InfoTile : TileBase
    {
        public enum TileType
        {
            Void,
            Ground,
            Wall,
            Water,
            Lava,
            Count
        }

#if UNITY_EDITOR
        private static readonly Color[] _infoTileColor =
        {
            new Color(0, 0, 0, 1), //void
            new Color(0, 1, 0, 0.1f), //ground
            new Color(1, 0, 0, 0.1f), //wall
            new Color(0, 0, 1, 0.1f), //water
            new Color(1, 0.5f, 0, 0.1f) //lava 
        };
#endif
        public static Sprite movementInfoTileSprite;
        public static readonly InfoTile[] InfoTiles = new InfoTile[(int) TileType.Count * 2];
        public bool hasActor;

        public TileType tileType = TileType.Void;

        private static void InitInfoTile()
        {
            for (var i = 0; i < InfoTiles.Length / 2; ++i)
            {
                InfoTiles[i] = CreateInstance<InfoTile>();
                InfoTiles[i].tileType = (TileType) i;
                InfoTiles[i].hasActor = false;
            }

            for (var i = (int) TileType.Count; i < InfoTiles.Length; ++i)
            {
                InfoTiles[i] = CreateInstance<InfoTile>();
                InfoTiles[i].tileType = (TileType) (i - InfoTiles.Length / 2);
                InfoTiles[i].hasActor = true;
            }
        }

        public static InfoTile GetInfoTile(TileType tileType, bool hasActor)
        {
            if (InfoTiles[0] == null)
                InitInfoTile();
            return InfoTiles[(int) tileType + (hasActor ? (int) TileType.Count : 0)];
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.flags = TileFlags.LockAll;
#if UNITY_EDITOR
            tileData.color = hasActor ? new Color(1, 0f, 0, 0.5f) : _infoTileColor[(int) tileType];
#endif
            if (movementInfoTileSprite == null)
            {
                var spriteTexture = new Texture2D(1, 1);
                spriteTexture.SetPixel(0, 0, Color.white);
                spriteTexture.Apply();
                movementInfoTileSprite =
                    Sprite.Create(spriteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
            }

            tileData.sprite = movementInfoTileSprite;
        }
    }
}