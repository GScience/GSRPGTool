using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;

#endif

namespace RPGTool.Tiles
{
    public class AutoTile : TileBase
    {
        public enum TileConnection
        {
            None,
            Up,
            Down,
            Left,
            Right,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight,
            UpDown,
            LeftRight,
            UpLeftRight,
            DownLeftRight,
            UpDownLeft,
            UpDownRight,
            UpDownLeftRight,

            UpLeftCornerUpLeft,
            UpRightCornerUpRight,
            DownLeftCornerDownLeft,
            DownRightCornerDownRight,
            UpLeftRightCornerUpLeft,
            DownLeftRightCornerDownLeft,
            UpDownLeftCornerUpLeft,
            UpDownRightCornerUpRight,
            UpLeftRightCornerUpRight,
            DownLeftRightCornerDownRight,
            UpDownLeftCornerDownLeft,
            UpDownRightCornerDownRight,
            UpLeftRightCornerUpLeftUpRight,
            DownLeftRightCornerDownLeftDownRight,
            UpDownLeftCornerUpLeftDownLeft,
            UpDownRightCornerUpRightDownRight,
            CornerUpLeft,
            CornerUpRight,
            CornerDownLeft,
            CornerDownRight,
            CornerUpLeftUpRight,
            CornerUpLeftDownLeft,
            CornerUpLeftDownRight,
            CornerUpRightDownLeft,
            CornerUpRightDownRight,
            CornerDownLeftDownRight,
            CornerUpRightDownLeftDownRight,
            CornerUpLeftDownLeftDownRight,
            CornerUpLeftUpRightDownRight,
            CornerUpLeftUpRightDownLeft,
            CornerUpLeftUpRightDownLeftDownRight,
            Count
        }

        public static readonly Vector2Int AutoTileGrid = new Vector2Int(2, 3);
        public static readonly Vector2Int FullTileGrid = new Vector2Int(7, 7);

#if UNITY_EDITOR
        private bool _refreshingAroundFlag;
#endif
        public float animationSpeed = 1.0f;

        [HideInInspector] public Rect autoTileSpriteRect;

        [HideInInspector] public Texture2D autoTileTexture;

        //纹理
        [SerializeField] public AutoTileAnimationInfo[] tileSprites;

        public InfoTile.TileType tileType = InfoTile.TileType.Ground;

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

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
#if UNITY_EDITOR
            var gameObject = tilemap.GetComponent<Tilemap>().gameObject;

            if (!Application.isPlaying || EditorSceneManager.IsPreviewSceneObject(gameObject))
                return true;
#endif
            //刷新地图的可行走度
            if (SceneInfo.sceneInfo != null)
            {
                var currentTileInfo =
                    SceneInfo.sceneInfo.infoTilemap.GetTileInfo(new Vector2Int(position.x, position.y));

                if (currentTileInfo.tileType == InfoTile.TileType.Ground ||
                    currentTileInfo.tileType == InfoTile.TileType.Void)
                    SceneInfo.sceneInfo.infoTilemap.SetTileInfo(new Vector2Int(position.x, position.y), tileType,
                        currentTileInfo.hasActor);
            }

            return true;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            //判断情况
            var autoTileAnimationInfo = GetTileSprites(tilemap, position);
            tileData.sprite = autoTileAnimationInfo.animationSprite[autoTileAnimationInfo.animationSprite.Length - 1];
#if UNITY_EDITOR
            //刷新周围的tile
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

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap,
            ref TileAnimationData tileAnimationData)
        {
            tileAnimationData.animationSpeed = animationSpeed;
            tileAnimationData.animatedSprites = GetTileSprites(tilemap, position).animationSprite;
            tileAnimationData.animationStartTime = 0;

            return true;
        }

        private AutoTileAnimationInfo GetTileSprites(ITilemap tilemap, Vector3Int position)
        {
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

            return tileSprites[(int) tileConnection];
        }

        [Serializable]
        public class AutoTileAnimationInfo
        {
            public Sprite[] animationSprite;
        }
    }
}