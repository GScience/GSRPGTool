using System;
using System.Collections.Generic;
using RPGTool.System;
using UnityEngine;

namespace RPGTool
{
    public enum PlayerInput
    {
        Up, Down, Left, Right,
        Interaction,
        End
    }
    [ExecuteInEditMode]
    public class GameMapManager : MonoBehaviour
    {
        private static GameMapManager _gameMapManager;
        private bool _ignorePlayerInteract;

        /// <summary>
        /// 玩家输入信息
        /// </summary>
        public List<bool> PlayerInputs { get; private set; } = new List<bool>((int) PlayerInput.End);

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

            for (var i = 0; i < (int) PlayerInput.End; ++i)
                PlayerInputs.Add(false);
        }

        private void Update()
        {
            UpdatePlayerInputs();
            UpdatePlayerInteract();
        }

        private void UpdatePlayerInputs()
        {
            for (var i = 0; i < PlayerInputs.Count; ++i)
                PlayerInputs[i] = false;

            if (IgnorePlayerInteract)
                return;

            PlayerInputs[(int)PlayerInput.Up] = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            PlayerInputs[(int)PlayerInput.Down] = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            PlayerInputs[(int)PlayerInput.Left] = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            PlayerInputs[(int)PlayerInput.Right] = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            PlayerInputs[(int)PlayerInput.Interaction] = Input.GetKeyDown(KeyCode.Space);
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
                if (PlayerInputs[(int)PlayerInput.Up])
                    player.expectNextMoveDirection = Actor.Face.Up;
                else if (PlayerInputs[(int)PlayerInput.Down])
                    player.expectNextMoveDirection = Actor.Face.Down;
                else if (PlayerInputs[(int)PlayerInput.Left])
                    player.expectNextMoveDirection = Actor.Face.Left;
                else if (PlayerInputs[(int)PlayerInput.Right])
                    player.expectNextMoveDirection = Actor.Face.Right;
            }
        }

        public static bool GetInput(PlayerInput input)
        {
            return gameMapManager.PlayerInputs[(int) input];
        }
    }
}