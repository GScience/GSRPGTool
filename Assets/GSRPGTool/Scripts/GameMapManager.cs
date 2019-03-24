using RPGTool.System;
using UnityEngine;

namespace RPGTool
{
    [ExecuteInEditMode]
    public class GameMapManager : MonoBehaviour
    {
        private static GameMapManager _gameMapManager;
        private bool _ignorePlayerInteract;

        /// <summary>
        ///     隐藏用canvas
        /// </summary>
        public Fader fader;

        /// <summary>
        ///     地图状态
        /// </summary>
        public InfoTilemap infoTilemap;

        /// <summary>
        ///     对话框
        /// </summary>
        public Dialog mainDialog;

        /// <summary>
        ///     玩家
        /// </summary>
        public Actor player;

        /// <summary>
        ///     全局变量
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

        /// <summary>
        ///     屏蔽玩家交互
        /// </summary>
        public bool IgnorePlayerInteract
        {
            get => mainDialog.PrintingPaused || mainDialog.Message.Length != 0 || //对话框自动屏蔽
                   _ignorePlayerInteract; //手动屏蔽
            set => _ignorePlayerInteract = value;
        }

        /// <summary>
        ///     玩家坐标
        /// </summary>
        public Vector2Int PlayerPosition { get; private set; }

        private void Awake()
        {
            gameMapManager = this;
        }

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

            PlayerPosition = player.GridTransform.position;

            //移动刷新
            if (!player.GridTransform.IsMoving)
            {
                if (Input.GetKey(KeyCode.W))
                    player.expectNextMoveDirection = Actor.Face.Up;
                else if (Input.GetKey(KeyCode.S))
                    player.expectNextMoveDirection = Actor.Face.Down;
                else if (Input.GetKey(KeyCode.A))
                    player.expectNextMoveDirection = Actor.Face.Left;
                else if (Input.GetKey(KeyCode.D))
                    player.expectNextMoveDirection = Actor.Face.Right;
            }
        }
    }
}