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
        public Sprite[] sprites;

        public static readonly Vector2Int AutoTileGrid = new Vector2Int(2, 3);
        public static readonly Vector2Int FullTileGrid = new Vector2Int(7, 7);

        public float animationSpeed = 1.0f;

        //纹理
        private Sprite[] _fullTileSprite;
        private Texture2D[] _fullTileTexture;
        private readonly Dictionary<TileConnection, Sprite> _tmpTileIndex = new Dictionary<TileConnection, Sprite>();
        private Vector2Int _tileSize;

#if UNITY_EDITOR
        private bool _refreshingAroundFlag;
#endif
        private static Sprite GenAutoTileSprite(Sprite sprite, Vector2Int tileSize)
        {
            //获得单个图块的大小
            var autoTileTexture = new Texture2D(tileSize.x * FullTileGrid.x, tileSize.y * FullTileGrid.y)
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
                var connectionInfo = ((TileConnection)i).ToString().Replace("Corner", ".").Split('.');

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

                var tilePos = new Vector2Int(i % 7 * tileSize.x, i / 7 * tileSize.y);
                if (tileData[0][0] == 4 && tileData[0][1] == 8)
                {
                    tileData[0][0] += 4;
                    tileData[0][1] -= 4;
                }
                else if (tileData[1][0] == 7 || tileData[1][1] == 11)
                {
                    tileData[1][0] += 4;
                    tileData[1][1] -= 4;
                }
                if (tileData[0][0] == 1 && tileData[1][0] == 2)
                {
                    tileData[0][0] += 1;
                    tileData[1][0] -= 1;
                }
                else if (tileData[0][1] == 13 || tileData[1][1] == 14)
                {
                    tileData[0][1] += 1;
                    tileData[1][1] -= 1;
                }
                for (var x = 0; x < 2; ++x)
                    for (var y = 0; y < 2; ++y)
                    {
                        if (tileData[x][y] == 5 || tileData[x][y] == 6 || tileData[x][y] == 9 || tileData[x][y] == 10)
                        {
                            if (x == 1 && (tileData[0][y] == 4 || tileData[0][y] == 8))
                                tileData[x][y] = tileData[0][y] + 1;
                            else if (y == 1 && (tileData[x][0] == 1 || tileData[x][0] == 2))
                                tileData[x][y] = tileData[x][0] + 4;
                            else if (x == 0 && (tileData[1][y] == 7 || tileData[1][y] == 11))
                                tileData[x][y] = tileData[1][y] - 1;
                            else if (y == 0 && (tileData[x][1] == 13 || tileData[x][1] == 14))
                                tileData[x][y] = tileData[x][1] - 4;


                            else if (x == 0 && y == 0 && tileData[0][1] == 22)
                                tileData[x][y] = 10;
                            else if (x == 1 && y == 1 && tileData[0][1] == 22)
                                tileData[x][y] = 5;
                            else if (x == 1 && y == 0 && tileData[0][1] == 22)
                                tileData[x][y] = 9;

                            else if (x == 0 && y == 1 && tileData[1][1] == 23)
                                tileData[x][y] = 6;
                            else if (x == 1 && y == 0 && tileData[1][1] == 23)
                                tileData[x][y] = 9;
                            else if (x == 0 && y == 0 && tileData[1][1] == 13)
                                tileData[x][y] = 10;

                            else if (x == 0 && y == 0 && tileData[1][0] == 19)
                                tileData[x][y] = 10;
                            else if (x == 1 && y == 1 && tileData[1][0] == 19)
                                tileData[x][y] = 5;
                            else if (x == 0 && y == 1 && tileData[1][0] == 19)
                                tileData[x][y] = 6;

                            else if (x == 0 && y == 1 && tileData[0][0] == 18)
                                tileData[x][y] = 6;
                            else if (x == 1 && y == 0 && tileData[0][0] == 18)
                                tileData[x][y] = 9;
                            else if (x == 1 && y == 1 && tileData[0][0] == 18)
                                tileData[x][y] = 5;
                        }
                        DrawSpriteOnTexture(autoTileTexture, GetSubSprite(sprite, tileData[x][y], AutoTileGrid * 2),
                            tilePos + new Vector2Int((int) (tileSize.x * x * 0.5f), (int) (tileSize.y * y * 0.5f)));
                    }
            }
            autoTileTexture.Apply();

            return Sprite.Create(autoTileTexture, new Rect(0, 0, autoTileTexture.width, autoTileTexture.height),
                new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            //更改滤波
            if (sprites == null || sprites.Length == 0)
                return;

            if (sprites.Any(sprite => sprite == null))
                return;

            _tmpTileIndex.Clear();
#endif

            _fullTileSprite = new Sprite[sprites.Length];
            _fullTileTexture = new Texture2D[sprites.Length];
            _tileSize = new Vector2Int((int)(sprites[0].rect.width / 2), (int)(sprites[0].rect.height / 3));

            for (var i = 0; i < sprites.Length; ++i)
            {
                _fullTileSprite[i] = GenAutoTileSprite(sprites[i], _tileSize);
                _fullTileTexture[i] = _fullTileSprite[i].texture;
            }
        }

        private static Sprite GetSubSprite(Sprite sprite, int index, Vector2Int gridCount)
        {
            var rect = sprite.rect;

            var x = index % gridCount.x;
            var y = index / gridCount.x;

            rect.x += rect.width * x / gridCount.x;
            rect.y += rect.height * y / gridCount.y;
            rect.height = sprite.rect.height / gridCount.y;
            rect.width = sprite.rect.width / gridCount.x;
            return Sprite.Create(sprite.texture, rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
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
#if UNITY_EDITOR
            if (_fullTileSprite == null || _fullTileSprite.Length == 0 || _fullTileSprite[0] == null)
                OnEnable();
#endif
            //判断情况
            tileData.sprite = GetTileSprite(tilemap, position, _fullTileSprite[_fullTileSprite.Length - 1], sprites[sprites.Length - 1]);
            
            //刷新周围的tile
#if UNITY_EDITOR
            if (_refreshingAroundFlag || Application.isPlaying)
                return;

            _refreshingAroundFlag = true;
            for (var x = -2; x <= 2; ++x)
            for (var y = -2; y <= 2; ++y)
            {
                if (x == 0 && y == 0) continue;
                var pos = position + new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) == this)
                    RefreshTile(pos, tilemap);
            }

            _refreshingAroundFlag = false;
#endif
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            if (_fullTileSprite == null || _fullTileSprite.Length <= 1)
                return false;

            var animationSprite = new Sprite[_fullTileSprite.Length];
            for (var i = 0; i < _fullTileSprite.Length;++i)
                animationSprite[i] = GetTileSprite(tilemap, position, _fullTileSprite[i], sprites[i]);

            tileAnimationData.animationSpeed = animationSpeed;
            tileAnimationData.animatedSprites = animationSprite;

            return true;
        }

        private Sprite GetTileSprite(ITilemap tilemap, Vector3Int position, Sprite fullTileSprite, Sprite autoTileSprite)
        {
            //return fullTileSprite;
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

            if (_tmpTileIndex.TryGetValue(tileConnection, out var tileSprite))
                return tileSprite;

            tileSprite = tileConnection == TileConnection.None ? 
                GetSubSprite(autoTileSprite, 4, AutoTileGrid) : 
                GetSubSprite(fullTileSprite, (int)tileConnection, FullTileGrid);
            _tmpTileIndex[tileConnection] = tileSprite;
            return tileSprite;
        }
    }
}
