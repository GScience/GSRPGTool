using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGTool.Tiles
{
    [CreateAssetMenu(menuName = "Tiles/AutoTile")]
    public class AutoTile : TileBase
    {
        enum TileConnection
        {
            None,
            Up,Down,Left,Right,
            UpLeft,UpRight,DownLeft,DownRight,
            UpDown, LeftRight,
            UpLeftRight,DownLeftRight, UpDownLeft, UpDownRight,
            UpDownLeftRight,

            UpLeftCornerUpLeft, UpRightCornerUpRight, DownLeftCornerDownLeft, DownRightCornerDownRight,
            UpLeftRightCornerUpLeft, DownLeftRightCornerDownLeft, UpDownLeftCornerUpLeft, UpDownRightCornerUpRight,
            UpLeftRightCornerUpRight, DownLeftRightCornerDownRight, UpDownLeftCornerDownLeft, UpDownRightCornerDownRight,
            UpLeftRightCornerUpLeftUpRight, DownLeftRightCornerDownLeftDownRight, UpDownLeftCornerUpLeftDownLeft, UpDownRightCornerUpRightDownRight,
            CornerUpLeft, CornerUpRight, CornerDownLeft, CornerDownRight,
            CornerUpLeftUpRight, CornerUpLeftDownLeft, CornerUpLeftDownRight,
            CornerUpRightDownLeft, CornerUpRightDownRight,
            CornerDownLeftDownRight,
            CornerUpRightDownLeftDownRight, CornerUpLeftDownLeftDownRight, CornerUpLeftUpRightDownRight, CornerUpLeftUpRightDownLeft,
            CornerUpLeftUpRightDownLeftDownRight
        }


        private static TileConnection GetTileConnection(
            bool up, bool upLeft, bool left, bool downLeft, 
            bool down, bool downRight, bool right, bool upRight)
        {
            //如果都没有返回空
            if (!up && !left && !down && !right)
                return TileConnection.None;

            var tileConnectionName = "";

            if (up)
                tileConnectionName += "Up";
            if (down)
                tileConnectionName += "Down";
            if (left)
                tileConnectionName += "Left";
            if (right)
                tileConnectionName += "Right";

            var cornerInfo = "";

            if (!upLeft && up && left)
                cornerInfo += "UpLeft";
            if (!upRight && up && right)
                cornerInfo += "UpRight";
            if (!downLeft && down && left)
                cornerInfo += "DownLeft";
            if (!downRight && down && right)
                cornerInfo += "DownRight";

            if (cornerInfo != "")
            {
                //如果全有的话则省略上下左右
                if (tileConnectionName == "UpDownLeftRight")
                    tileConnectionName = "";

                tileConnectionName += "Corner" + cornerInfo;
            }
            if (!Enum.TryParse(tileConnectionName, out TileConnection connection))
                throw new KeyNotFoundException("Failed to found " + tileConnectionName);
            return connection;
        }
        [SerializeField]
        public Sprite sprite;

        //纹理
        private Texture2D _autoTileTexture;

        private Vector2Int _tileSize;

#if UNITY_EDITOR
        private bool _refreshingAroundFlag = false;
#endif
        void OnEnable()
        {
#if UNITY_EDITOR
            //更改滤波
            if (sprite == null)
                return;
#endif
            //获得单个图块的大小
            _tileSize = new Vector2Int((int) (sprite.rect.width / 2), (int) (sprite.rect.height / 3));
            _autoTileTexture = new Texture2D(_tileSize.x * 7, _tileSize.y * 7)
            {
                filterMode = FilterMode.Point
            };

            //开始创建图块
            for (var i = 0; i <= (int)TileConnection.CornerUpLeftUpRightDownLeftDownRight; ++i)
            {
                var tileData = new[]
                {
                    new[] {5, 9},
                    new[] {6, 10}
                };
                var connectionInfo = ((TileConnection) i).ToString().Replace("Corner", ".").Split('.');

                if (connectionInfo[0] == "None")
                {
                    tileData[0][0] = 16;
                    tileData[1][0] = 17;
                    tileData[0][1] = 20;
                    tileData[1][1] = 21;
                }
                else
                {
                    if (connectionInfo[0] == "")
                        connectionInfo[0] = "UpDownLeftRight";
                    if (!connectionInfo[0].Contains("Up"))
                    {
                        tileData[0][0] += 4;
                        tileData[1][0] += 4;
                        tileData[0][1] += 4;
                        tileData[1][1] += 4;
                    }

                    if (!connectionInfo[0].Contains("Down"))
                    {
                        tileData[0][0] -= 4;
                        tileData[1][0] -= 4;
                        tileData[0][1] -= 4;
                        tileData[1][1] -= 4;

                        if (!connectionInfo[0].Contains("Up"))
                        {
                            tileData[0][0] -= 4;
                            tileData[1][0] -= 4;
                            tileData[0][1] += 4;
                            tileData[1][1] += 4;
                        }
                    }

                    if (!connectionInfo[0].Contains("Left"))
                    {
                        tileData[0][0] -= 1;
                        tileData[0][1] -= 1;
                        tileData[1][0] -= 1;
                        tileData[1][1] -= 1;
                    }

                    if (!connectionInfo[0].Contains("Right"))
                    {
                        tileData[0][0] += 1;
                        tileData[0][1] += 1;
                        tileData[1][0] += 1;
                        tileData[1][1] += 1;
                        if (!connectionInfo[0].Contains("Left"))
                        {
                            tileData[0][0] -= 1;
                            tileData[0][1] -= 1;
                            tileData[1][0] += 1;
                            tileData[1][1] += 1;
                        }
                    }

                    if (connectionInfo.Length == 2)
                    {
                        if (connectionInfo[1].Contains("UpLeft"))
                            tileData[0][1] = 22;
                        if (connectionInfo[1].Contains("UpRight"))
                            tileData[1][1] = 23;
                        if (connectionInfo[1].Contains("DownLeft"))
                            tileData[0][0] = 18;
                        if (connectionInfo[1].Contains("DownRight"))
                            tileData[1][0] = 19;
                    }
                }

                var tilePos = new Vector2Int(i % 7 * _tileSize.x, i / 7 * _tileSize.y);
                for (var x = 0; x < 2; ++x)
                for (var y = 0; y < 2; ++y)
                    DrawSpriteOnTexture(_autoTileTexture, GetSubAutoTile(tileData[x][y]),
                        tilePos + new Vector2Int((int) (_tileSize.x * x * 0.5f), (int) (_tileSize.y * y * 0.5f)));
            }
            _autoTileTexture.Apply();
        }

        Sprite GetSubAutoTile(int index)
        {
            var rect = sprite.rect;

            var x = index % 4;
            var y = index / 4;

            rect.x += rect.width * x / 4.0f;
            rect.y += rect.height * y / 6.0f;
            rect.height = _tileSize.y / 2.0f;
            rect.width = _tileSize.x / 2.0f;
            return Sprite.Create(sprite.texture, rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
        }

        Sprite GetTileSprite(TileConnection tileConnection)
        {
            if (_autoTileTexture == null)
                OnEnable();

            var rect = new Rect();

            var index = (int) tileConnection;

            var x = index % 7;
            var y = index / 7;

            rect.x += _autoTileTexture.width * x / 7.0f;
            rect.y += _autoTileTexture.height * y / 7.0f;
            rect.height = _tileSize.y;
            rect.width = _tileSize.x;
            /*return Sprite.Create(_autoTileTexture, new Rect(0, 0, _autoTileTexture.width, _autoTileTexture.height),
                new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);*/
            return Sprite.Create(_autoTileTexture, rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
        }

        static void DrawSpriteOnTexture(Texture2D texture, Sprite sprite, Vector2Int pos)
        {
            texture.SetPixels(pos.x, pos.y,
                (int) sprite.rect.width, (int) sprite.rect.height, sprite.texture.GetPixels(
                    (int) sprite.rect.x, (int) sprite.rect.y,
                    (int) sprite.rect.width, (int) sprite.rect.height));
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            //判断情况
            var tileConnection = GetTileConnection(
                tilemap.GetTile(position + Vector3Int.up) == this,
                tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) == this,
                tilemap.GetTile(position + Vector3Int.left) == this,
                tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) == this,
                tilemap.GetTile(position + Vector3Int.down) == this,
                tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) == this,
                tilemap.GetTile(position + Vector3Int.right) == this,
                tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) == this
            );

            var newSprite = GetTileSprite(tileConnection);
            tileData.sprite = newSprite;

            //刷新周围的tile
#if UNITY_EDITOR
            if (_refreshingAroundFlag || Application.isPlaying)
                return;

            _refreshingAroundFlag = true;
            for (var x = -2; x <= 2; ++x)
            for (var y = -2; y <= 2; ++y)
            {
                var pos = position + new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) == this)
                    RefreshTile(pos, tilemap);
            }

            _refreshingAroundFlag = false;
#endif
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }
    }
}
