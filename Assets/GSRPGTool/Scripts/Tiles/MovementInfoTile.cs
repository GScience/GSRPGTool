using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGTool.Tiles
{
    public class MovementInfoTile : TileBase
    {
        public static Sprite movementInfoTileSprite;

        public bool canMove = false;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.flags = TileFlags.LockAll;
            tileData.color = canMove ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

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