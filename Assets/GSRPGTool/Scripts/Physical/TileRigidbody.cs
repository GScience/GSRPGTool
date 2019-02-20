using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSRPGTool.Physical;
using UnityEngine;

namespace RPGTool.Physical
{
    [RequireComponent(typeof(TileCollider), typeof(GridTransform))]
    public class TileRigidbody : MonoBehaviour
    {
        /// <summary>
        /// 刚体速度
        /// </summary>
        public float speed = 2;

        public GridTransform GridTransform { get; private set; }
        public TileCollider TileCollider { get; private set; }
        public Actor Actor { get; private set; }

        private Actor.Face? _movement;

        void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
            TileCollider = GetComponent<TileCollider>();
            Actor = GetComponent<Actor>();
        }

        void Update()
        {
            if (Actor != null)
                Actor.animationSpeed = speed;
        }

        /// <summary>
        /// 向指定方向移动一次
        /// 转向也算一次
        /// </summary>
        /// <param name="direction">方向，默认为null</param>
        public void SetMoveDirection(Actor.Face? direction = null)
        {
            _movement = direction;
        }

        void LateUpdate()
        {
            if (GridTransform.MovingCoroutine == null && _movement != null)
                UpdateMovement(_movement.Value);
        }

        private void UpdateMovement(Actor.Face direction)
        {
            if (Actor == null || Actor.faceTo == direction)
                switch (_movement)
                {
                    case Actor.Face.Up:
                        if (SceneInfo.sceneInfo.movementInfoTilemap.CanMove(GridTransform.position + Vector2Int.up))
                            GridTransform.Move(Vector2Int.up, 1 / speed);
                        break;
                    case Actor.Face.Down:
                        if (SceneInfo.sceneInfo.movementInfoTilemap.CanMove(GridTransform.position + Vector2Int.down))
                            GridTransform.Move(Vector2Int.down, 1 / speed);
                        break;
                    case Actor.Face.Left:
                        if (SceneInfo.sceneInfo.movementInfoTilemap.CanMove(GridTransform.position + Vector2Int.left))
                            GridTransform.Move(Vector2Int.left, 1 / speed);
                        break;
                    case Actor.Face.Right:
                        if (SceneInfo.sceneInfo.movementInfoTilemap.CanMove(GridTransform.position + Vector2Int.right))
                            GridTransform.Move(Vector2Int.right, 1 / speed);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            else
            if (Actor != null)
                Actor.faceTo = direction;
        }
    }
}
