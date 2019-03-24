using System;
using System.Collections.Generic;
using System.IO;
using RPGTool.Save;
using RPGTool.Tiles;
using UnityEngine;

namespace RPGTool
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer), typeof(GridTransform))]
    public class Actor : SavableBehaviour
    {
        /// <summary>
        ///     角色面向
        /// </summary>
        public enum Face
        {
            Up = 3,
            Down = 0,
            Left = 1,
            Right = 2
        }

        /// <summary>
        ///     分割后的角色图像
        /// </summary>
        private readonly Sprite[] _actorSprites = new Sprite[12];

        /// <summary>
        ///     当前的角色移动方向
        /// </summary>
        private Face? _currentMoveDirection;

        private float _deltaTimeFromChangeIndex = 3;

        /// <summary>
        ///     角色图像索引
        /// </summary>
        private int _movingStage;
#if UNITY_EDITOR
        private Face _nowSpriteFace = Face.Up;
#endif

        /// <summary>
        ///     角色的图像
        /// </summary>
        public Texture2D actorTexture;

        /// <summary>
        ///     是否可以穿过
        /// </summary>
        public bool canPass;

        /// <summary>
        ///     接下来一格预期的角色移动方向
        /// </summary>
        public Face? expectNextMoveDirection;

        /// <summary>
        ///     角色的面向
        /// </summary>
        public Face faceTo = Face.Down;

        /// <summary>
        ///     是否是运动学的(忽略一切物理碰撞)
        /// </summary>
        public bool isKinematic;

        /// <summary>
        ///     角色素材中心
        /// </summary>
        public Vector2 pivot = new Vector2(0.5f, 0.25f);

        public int pixelPerTexture = 32;

        /// <summary>
        ///     角色速度
        /// </summary>
        public float speed = 2;

        /// <summary>
        ///     动画速度
        /// </summary>
        public float AnimationSpeed => speed;

        public GridTransform GridTransform { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }

        public List<Vector2Int> JointPositions { get; private set; } = new List<Vector2Int>();

        public override void OnSave(BinaryWriter stream)
        {
            DataSaver.Save(GridTransform.position, stream);
            DataSaver.Save(faceTo, stream);
            DataSaver.Save(isKinematic, stream);
            DataSaver.Save(canPass, stream);
        }

        public override void OnLoad(BinaryReader stream)
        {
            GridTransform = GetComponent<GridTransform>();
            GridTransform.ResetMovement();
            GridTransform.position = DataLoader.Load<Vector2Int>(stream);
            faceTo = DataLoader.Load<Face>(stream);
            isKinematic = DataLoader.Load<bool>(stream);
            canPass = DataLoader.Load<bool>(stream);
        }

        public static Vector2Int FaceToVector(Face face)
        {
            switch (face)
            {
                case Face.Up:
                    return Vector2Int.up;
                case Face.Down:
                    return Vector2Int.down;
                case Face.Left:
                    return Vector2Int.left;
                case Face.Right:
                    return Vector2Int.right;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
            SpriteRenderer = GetComponent<SpriteRenderer>();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            if (actorTexture == null)
                return;

            //分割图像
            for (var i = 0; i < 12; ++i)
                _actorSprites[i] = GetSprite(i);

            //设置纹理显示模式
            actorTexture.filterMode = FilterMode.Point;
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
        }

        private Sprite GetSprite(int index)
        {
            return Sprite.Create(actorTexture,
                new Rect(
                    index % 3 / 3.0f * actorTexture.width, (3 - index / 3) / 4.0f * actorTexture.height,
                    actorTexture.width / 3.0f, actorTexture.height / 4.0f),
                pivot,
                pixelPerTexture);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                UpdateMovement();
                if (!canPass)
                    UpdateCollider();
#if UNITY_EDITOR
            }
#endif
            if (actorTexture != null)
                UpdateAnima();
        }

        /// <summary>
        ///     刷新移动判断
        /// </summary>
        private void UpdateMovement()
        {
            if (GridTransform.IsMoving || expectNextMoveDirection == null)
                return;

            faceTo = expectNextMoveDirection.Value;
            _currentMoveDirection = expectNextMoveDirection;

            var directionVector = FaceToVector(faceTo);
            if (!isKinematic &&
                !CanMoveIn(
                    GameMapManager.gameMapManager.infoTilemap.GetTileInfo(GridTransform.position + directionVector)))
                return;
            GridTransform.Move(directionVector, 1 / speed);
            expectNextMoveDirection = null;
        }

        /// <summary>
        ///     刷新动画
        /// </summary>
        private void UpdateAnima()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (_nowSpriteFace != faceTo)
                {
                    GetComponent<SpriteRenderer>().sprite = GetSprite(1 + (int) faceTo * 3);
                    _nowSpriteFace = faceTo;
                }
            }
            else
#endif
            {
                SwitchNextSprite(!GridTransform || GridTransform.MovingCoroutine == null);
            }
        }

        /// <summary>
        ///     获取下一步的Sprite
        /// </summary>
        /// <param name="isStop">是否停止</param>
        /// <returns></returns>
        public void SwitchNextSprite(bool isStop)
        {
            _deltaTimeFromChangeIndex += Time.deltaTime;
            var timePerFrame = 1.0f / AnimationSpeed / 4f;

            if (isStop)
            {
                SpriteRenderer.sprite = _actorSprites[1 + (int) faceTo * 3];
                _deltaTimeFromChangeIndex = 0;
                _movingStage = 0;
            }
            else
            {
                if (_deltaTimeFromChangeIndex >= timePerFrame)
                {
                    _deltaTimeFromChangeIndex %= timePerFrame;

                    ++_movingStage;
                }

                int imageIndex;
                switch (_movingStage)
                {
                    case 0:
                    case 2:
                        imageIndex = 1;
                        break;
                    case 1:
                        imageIndex = 0;
                        break;
                    case 3:
                        imageIndex = 2;
                        break;
                    default:
                        _movingStage = 0;
                        imageIndex = 1;
                        break;
                }

                SpriteRenderer.sprite = _actorSprites[imageIndex + (int) faceTo * 3];
            }
        }

        /// <summary>
        ///     是否可以走在指定类型的Tile当中
        /// </summary>
        /// <param name="infoTile">Tile信息</param>
        /// <returns></returns>
        public virtual bool CanMoveIn(InfoTile infoTile)
        {
            if (infoTile == null)
                return false;

            if (infoTile.hasActor)
                return false;

            return infoTile.tileType == InfoTile.TileType.Ground;
        }

        /// <summary>
        ///     刷新碰撞箱
        /// </summary>
        private void UpdateCollider()
        {
            if (GridTransform.IsMoving)
            {
                var newJointPosition = new List<Vector2Int>();

                var directionVector = FaceToVector(_currentMoveDirection ?? faceTo);

                newJointPosition.Add(GridTransform.position + directionVector);
                newJointPosition.Add(GridTransform.position);
                UpdateJointPos(newJointPosition);
            }
            else
            {
                UpdateJointPos(new List<Vector2Int> {GridTransform.position});
            }

            //脚下始终不可通过
            GameMapManager.gameMapManager.infoTilemap.SetTileInfo(GridTransform.position, true);
        }

        /// <summary>
        ///     刷新碰撞相交点
        /// </summary>
        /// <param name="newJointPosition">碰撞橡胶垫列表</param>
        private void UpdateJointPos(List<Vector2Int> newJointPosition)
        {
            //新的站位
            foreach (var pos in newJointPosition)
                if (!JointPositions.Contains(pos))
                    GameMapManager.gameMapManager.infoTilemap.SetTileInfo(pos, true);

            //旧的站位
            foreach (var pos in JointPositions)
                if (!newJointPosition.Contains(pos))
                    GameMapManager.gameMapManager.infoTilemap.SetTileInfo(pos, false);

            JointPositions = newJointPosition;
        }
    }
}