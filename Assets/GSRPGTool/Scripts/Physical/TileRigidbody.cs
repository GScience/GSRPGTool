﻿using System;
using UnityEngine;

namespace RPGTool.Physical
{
    [RequireComponent(typeof(TileCollider), typeof(GridTransform))]
    public class TileRigidbody : MonoBehaviour
    {
        private Actor.Face? _movement;

        /// <summary>
        ///     刚体速度
        /// </summary>
        public float speed = 2;

        public bool isKinematic = false;

        public GridTransform GridTransform { get; private set; }
        public TileCollider TileCollider { get; private set; }
        public Actor Actor { get; private set; }

        private void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
            TileCollider = GetComponent<TileCollider>();
            Actor = GetComponent<Actor>();
        }

        private void Update()
        {
            if (Actor != null)
                Actor.animationSpeed = speed;
        }

        /// <summary>
        ///     向指定方向移动一次
        /// </summary>
        /// <param name="direction">方向，默认为null</param>
        public void SetMoveDirection(Actor.Face? direction = null)
        {
            _movement = direction;
        }

        public void LateUpdate()
        {
            if (GridTransform.MovingCoroutine == null && _movement != null)
                UpdateMovement(_movement.Value);
        }

        private void UpdateMovement(Actor.Face direction)
        {
            if (Actor == null || Actor.faceTo == direction)
            {
                switch (_movement)
                {
                    case Actor.Face.Up:
                        if (Actor == null || isKinematic ||
                            Actor.CanMoveIn(
                                SceneInfo.sceneInfo.infoTilemap.GetTileInfo(GridTransform.position + Vector2Int.up)))
                            ApplyMovement(Vector2Int.up);
                        break;
                    case Actor.Face.Down:
                        if (Actor == null || isKinematic ||
                            Actor.CanMoveIn(
                                SceneInfo.sceneInfo.infoTilemap.GetTileInfo(GridTransform.position + Vector2Int.down)))
                            ApplyMovement(Vector2Int.down);
                        break;
                    case Actor.Face.Left:
                        if (Actor == null || isKinematic ||
                            Actor.CanMoveIn(
                                SceneInfo.sceneInfo.infoTilemap.GetTileInfo(GridTransform.position + Vector2Int.left)))
                            ApplyMovement(Vector2Int.left);
                        break;
                    case Actor.Face.Right:
                        if (Actor == null || isKinematic ||
                            Actor.CanMoveIn(
                                SceneInfo.sceneInfo.infoTilemap.GetTileInfo(GridTransform.position + Vector2Int.right)))
                            ApplyMovement(Vector2Int.right);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }
            else if (Actor != null)
                Actor.faceTo = direction;
        }

        void ApplyMovement(Vector2Int direction)
        {
            _movement = null;
            GridTransform.Move(direction, 1 / speed);
            TileCollider.AddJointPoint(direction + Actor.GridTransform.position);
        }
    }
}