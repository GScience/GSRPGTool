﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool.Tiles;
using UnityEditor;
using UnityEngine;

namespace RPGTool.Editor
{
    public class WindowCreateTileFromAutoTile : EditorWindow
    {
        [MenuItem("RPGTool/Create tile from auto tile")]
        static void ShowMyWindow()
        {
            var myWindow = GetWindow<WindowCreateTileFromAutoTile>();
            myWindow.Show();//显示创建的自定义窗口
        }

        private Sprite _selectedSprite;
        private int _autoTileType;
        private string _tileNames;

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

        private static Sprite GenFullTileSprite(Sprite sprite, Vector2Int tileSize)
        {
            //获得单个图块的大小
            var autoTileTexture = new Texture2D(tileSize.x * AutoTile.FullTileGrid.x, tileSize.y * AutoTile.FullTileGrid.y)
            {
                filterMode = FilterMode.Point
            };

            //开始创建图块
            for (var i = 0; i < (int)AutoTile.TileConnection.Count; ++i)
            {
                var tileData = new[]
                {
                    new[] {5, 9},
                    new[] {6, 10}
                };
                var connectionInfo = ((AutoTile.TileConnection)i).ToString().Replace("Corner", ".").Split('.');

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

                        DrawSpriteOnTexture(autoTileTexture, GetSubSprite(sprite, tileData[x][y], AutoTile.AutoTileGrid * 2),
                            tilePos + new Vector2Int((int)(tileSize.x * x * 0.5f), (int)(tileSize.y * y * 0.5f)));
                    }
            }

            autoTileTexture.Apply();

            return Sprite.Create(autoTileTexture, new Rect(0, 0, autoTileTexture.width, autoTileTexture.height),
                new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit);
        }

        private static void DrawSpriteOnTexture(Texture2D texture, Sprite sprite, Vector2Int pos)
        {
            texture.SetPixels(pos.x, pos.y,
                (int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.GetPixels(
                    (int)sprite.rect.x, (int)sprite.rect.y,
                    (int)sprite.rect.width, (int)sprite.rect.height));
        }

        void OnGUI()
        {
            _autoTileType = EditorGUILayout.Popup("Auto tile type", _autoTileType, new[] {"11123334", "12345678"});
            _selectedSprite =
                (Sprite) EditorGUILayout.ObjectField("Auto tile", _selectedSprite, typeof(Sprite), false);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile name",GUILayout.MaxWidth(60));
            _tileNames = EditorGUILayout.TextArea(_tileNames,GUILayout.MinHeight(200));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Create"))
            {
                var tileNames = _tileNames;
                if (tileNames.Contains('\n'))
                {
                    if (tileNames.Contains('\r'))
                    {
                        for (var i = 0; i < tileNames.Length; ++i)
                            if (tileNames[i] == '\r')
                                tileNames = tileNames.Remove(i, 1);
                    }
                }
                else
                    tileNames = tileNames.Replace('\r', '\n');

                var nameList = tileNames.Split('\n');
                if (_autoTileType == 0)
                {
                    if (nameList.Length < 16)
                    {
                        EditorUtility.DisplayDialog("Error", "Name list should has at least 16 item for type 11123334",
                            " Ok");
                        return;
                    }

                    for (var x = 0; x < 4; ++x)
                    for (var y = 0; y < 4; ++y)
                    {
                        var posX = x == 0 ? 0 :
                            x == 1 ? _selectedSprite.rect.width * 3 / 8 :
                            x == 2 ? _selectedSprite.rect.width * 1 / 2 :
                            _selectedSprite.rect.width * 7 / 8;

                        var posY = _selectedSprite.rect.height * (3 - y) / 4;

                        var animaFrameCount = x % 2 == 0 ? 3 : 1;

                        var sprites = new Sprite[animaFrameCount];
                        for (var i = 0; i < animaFrameCount; ++i)
                        {
                            var rect = new Rect(posX + i * _selectedSprite.rect.width * 1.0f / 8, posY,
                                _selectedSprite.rect.width * 1.0f / 8,
                                _selectedSprite.rect.height / 4.0f);

                            sprites[i] = Sprite.Create(_selectedSprite.texture, rect, new Vector2(0.5f, 0.5f),
                                _selectedSprite.pixelsPerUnit);
                        }

                        var tileName = "";
                        tileName = y <= 1 ? nameList[x * 2 + y] : nameList[x * 2 + (y - 2) + 8];
                        CreateTile(sprites, Path.GetDirectoryName(AssetDatabase.GetAssetPath(_selectedSprite)),
                            tileName);
                    }
                }
                else if (_autoTileType == 1)
                {
                    if (nameList.Length < 32)
                    {
                        EditorUtility.DisplayDialog("Error", "Name list should has at least 16 item for type 11123334",
                            " Ok");
                        return;
                    }
                    for (var x = 0; x < 8; ++x)
                    for (var y = 0; y < 4; ++y)
                    {
                        var sprites = new Sprite[1];
                        var rect = new Rect(x / 8.0f * _selectedSprite.rect.width, (3 - y) / 4.0f * _selectedSprite.rect.height,
                            _selectedSprite.rect.width * 1.0f / 8,
                            _selectedSprite.rect.height / 4.0f);

                        sprites[0] = Sprite.Create(_selectedSprite.texture, rect, new Vector2(0.5f, 0.5f),
                            _selectedSprite.pixelsPerUnit);
                        CreateTile(sprites, AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(
                                Path.GetDirectoryName(AssetDatabase.GetAssetPath(_selectedSprite)), _selectedSprite.name)),
                            nameList[x + y * 8]);
                    }
                }
            }
        }

        static void CreateTile(Sprite[] autoTileSprites, string dir, string name)
        {
            var tile = CreateInstance<AutoTile>();
            tile.tileSprites = new AutoTile.AutoTileAnimationInfo[(int)AutoTile.TileConnection.Count];

            var tileName = name;

            var outputDir = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(dir, tileName));

            var tileSize = new Vector2Int((int)(autoTileSprites[0].rect.width / 2), (int)(autoTileSprites[0].rect.height / 3));

            for (var i = 0; i < autoTileSprites.Length; ++i)
            {
                var animationiDir = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(outputDir, "" + i));
                var fullTileSprite = GenFullTileSprite(autoTileSprites[i], tileSize);
                AssetDatabase.CreateAsset(fullTileSprite.texture,
                    animationiDir + "\\FullSprite.asset");

                //分割素材
                for (var j = 0; j < (int)AutoTile.TileConnection.Count; ++j)
                {
                    if (tile.tileSprites[j] == null)
                        tile.tileSprites[j] =
                            new AutoTile.AutoTileAnimationInfo { animationSprite = new Sprite[autoTileSprites.Length] };

                    var sprite = GetSubSprite(fullTileSprite, j, AutoTile.FullTileGrid);
                    tile.tileSprites[j].animationSprite[i] = sprite;

                    AssetDatabase.CreateAsset(sprite, animationiDir + "\\FullSprite_" + j + ".asset");
                }
            }
            AssetDatabase.CreateAsset(tile, outputDir + "_Tile.asset");
            AssetDatabase.Refresh();
        }
    }
}
