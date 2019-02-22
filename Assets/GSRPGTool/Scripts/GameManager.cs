using RPGTool.Physical;
using UnityEngine;

namespace RPGTool
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        ///     全局GameManager
        /// </summary>
        public static GameManager globalGameManager;

        /// <summary>
        ///     玩家
        /// </summary>
        public TileRigidbody player;

        /// <summary>
        ///     玩家坐标
        /// </summary>
        public Vector2Int playerPosition { get; private set; }

        private void Awake()
        {
            if (globalGameManager != null)
                Destroy(this);
            globalGameManager = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            UpdatePlayerTransform();
        }

        /// <summary>
        ///     刷新玩家的移动信息
        /// </summary>
        private void UpdatePlayerTransform()
        {
            if (player == null)
                return;

            playerPosition = player.GridTransform.position;

            //移动刷新
            if (Input.GetKey(KeyCode.W))
                player.SetMoveDirection(Actor.Face.Up);
            else if (Input.GetKey(KeyCode.S))
                player.SetMoveDirection(Actor.Face.Down);
            else if (Input.GetKey(KeyCode.A))
                player.SetMoveDirection(Actor.Face.Left);
            else if (Input.GetKey(KeyCode.D))
                player.SetMoveDirection(Actor.Face.Right);
            else
                player.SetMoveDirection();
        }
    }
}