using UnityEngine;

namespace RPGTool
{
    [ExecuteInEditMode]
    public class GameMapManager : MonoBehaviour
    {
        /// <summary>
        /// 全局变量
        /// </summary>
        public static GameMapManager gameMapManager;

        /// <summary>
        ///     玩家
        /// </summary>
        public Actor player;

        /// <summary>
        /// 地图状态
        /// </summary>
        public InfoTilemap infoTilemap;

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
            gameMapManager = this;

            UpdatePlayerTransform();
        }

        /// <summary>
        ///     刷新玩家的移动信息
        /// </summary>
        private void UpdatePlayerTransform()
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