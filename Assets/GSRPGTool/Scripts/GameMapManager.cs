using System;
using System.Collections.Generic;
using RPGTool.Save;
using RPGTool.System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool
{
    public enum PlayerInput
    {
        Up, Down, Left, Right,
        Interaction,
        End
    }
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
        public Fader Fader
        {
            get
            {
                if (_mainFader == null
#if UNITY_EDITOR 
                    && Application.isPlaying
#endif
                    )
                {
                    var mainFader = Instantiate(Resources.Load<GameObject>("FaderCanvas"), transform);
                    mainFader.name = "FaderCanvas";
                    _mainFader = mainFader.GetComponentInChildren<Fader>();
                }

                return _mainFader;
            }
        }

        private Fader _mainFader;

        /// <summary>
        ///     地图状态
        /// </summary>
        public InfoTilemap InfoTilemap
        {
            get
            {
                if (_infoTilemap == null
#if UNITY_EDITOR
                    && Application.isPlaying
#endif
                    )
                {
                    var infoTimemapObj = new GameObject("InfoTilemap", typeof(InfoTilemap));
                    infoTimemapObj.transform.parent = transform;
                    _infoTilemap = infoTimemapObj.GetComponent<InfoTilemap>();
                }
                return _infoTilemap;
            }
        }

        private InfoTilemap _infoTilemap;

        /// <summary>
        ///     对话框
        /// </summary>
        public Dialog MainDialog
        {
            get
            {
                if (_mainDialog == null
#if UNITY_EDITOR 
                    && Application.isPlaying
#endif
                    )
                {
                    var mainDialog = Instantiate(Resources.Load<GameObject>("DialogCanvas"), transform);
                    mainDialog.name = "DialogCanvas";
                    _mainDialog = mainDialog.GetComponentInChildren<Dialog>();
                }

                return _mainDialog;
            }
        }

        private Dialog _mainDialog;
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
            get => MainDialog.PrintingPaused || MainDialog.Message.Length != 0 || //对话框自动屏蔽
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
            if (player == null)
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

        /// <summary>
        /// 切换到场景
        /// </summary>
        /// <param name="sceneName">场景名，如果为空则切换到开始</param>
        public void SwitchToScene(string sceneName)
        {
#if UNITY_EDITOR
            if (SaveManager.saveManager)
#endif
                SaveManager.saveManager.SaveCurrentScene();

            if (string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(0);
            else
                SceneManager.LoadScene(sceneName);
        }

        public static bool GetInput(PlayerInput input)
        {
            return gameMapManager.PlayerInputs[(int) input];
        }
    }
}