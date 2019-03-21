using RPGTool.System;
using UnityEngine;

namespace RPGTool
{
    [ExecuteInEditMode]
    public class GameMapManager : MonoBehaviour
    {
        /// <summary>
        /// 全局变量
        /// </summary>
        public static GameMapManager gameMapManager
        {
            get
            {
                if (_gameMapManager == null)
                    _gameMapManager = FindObjectOfType<GameMapManager>();
                return _gameMapManager;
            }
            private set => _gameMapManager = value;
        }

        private static GameMapManager _gameMapManager;

        /// <summary>
        ///     玩家
        /// </summary>
        public Actor player;

        /// <summary>
        /// 地图状态
        /// </summary>
        public InfoTilemap infoTilemap;

        /// <summary>
        /// 对话框
        /// </summary>
        public Dialog mainDialog;

        /// <summary>
        /// 屏蔽玩家交互
        /// </summary>
        public bool IgnorePlayerInteract
        {
            get
            {
                return mainDialog.PrintingPaused || mainDialog.Message.Length != 0;
            }
        }

        private void Awake()
        {
            gameMapManager = this;
        }

        /// <summary>
        ///     玩家坐标
        /// </summary>
        public Vector2Int playerPosition { get; private set; }

        private void Update()
        {
            if (!IgnorePlayerInteract)
                UpdatePlayerInteract();
        }

        /// <summary>
        ///     刷新玩家的移动信息
        /// </summary>
        private void UpdatePlayerInteract()
        {
            if (player == null
#if UNITY_EDITOR
                || !Application.isPlaying
#endif
            )
                return;

            playerPosition = player.GridTransform.position;

            //移动刷新
            if (Input.GetKey(KeyCode.W))
                player.expectNextMoveDirection = Actor.Face.Up;
            else if (Input.GetKey(KeyCode.S))
                player.expectNextMoveDirection = Actor.Face.Down;
            else if (Input.GetKey(KeyCode.A))
                player.expectNextMoveDirection = Actor.Face.Left;
            else if (Input.GetKey(KeyCode.D))
                player.expectNextMoveDirection = Actor.Face.Right;
            else
                player.expectNextMoveDirection = null;
        }
    }
}