using System;
using System.Collections;
using UnityEngine;

namespace RPGTool
{
/*
 * 吸附网格移动
 */
    [ExecuteInEditMode]
    public class GridTransform : MonoBehaviour
    {
        /// <summary>
        ///     所在表格
        /// </summary>
        private Grid _grid;

        /// <summary>
        ///     移动偏移量
        /// </summary>
        private Vector2 _movementOffset = Vector2.zero;

        /// <summary>
        ///     格子内的偏移量
        /// </summary>
        [Tooltip("角色位置偏移")] public Vector2 offset = new Vector2(0.5f, 0.5f);

        /// <summary>
        ///     角色的格子位置
        ///     调用<see cref="Move" />或者<see cref="MoveTo" />的时候修改此变量不会产生效果
        /// </summary>
        [Tooltip("角色网格坐标")] public Vector2Int position;

        /// <summary>
        ///     移动协程
        /// </summary>
        public Coroutine MovingCoroutine { get; private set; }

        //是否正在移动
        public bool IsMoving { get; private set; }

        public Vector2 GridSize { get; private set; } = new Vector2(1, 1);

        public SpriteRenderer SpriteRenderer { get; private set; }

        public Vector2 MovingFloatPos => new Vector2(transform.position.x - offset.x, transform.position.y - offset.y);

        private void Awake()
        {
            _grid = GetComponentInParent<Grid>();
            SpriteRenderer = GetComponent<SpriteRenderer>();

            if (_grid)
                GridSize = _grid.cellSize;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                if (GetComponents<GridTransform>().Length != 1)
                    throw new ArgumentException("There should be at most one GridTransform");
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            //如果在编辑器则根据transform设置物体坐标则同步到position
            if (!Application.isPlaying)
                position = new Vector2Int((int) (transform.position.x - offset.x),
                    (int) (transform.position.y - offset.y));
#endif
            //重置移动状态
            if (MovingCoroutine == null)
                _movementOffset = Vector2.zero;

            //移动
            transform.position = new Vector3(position.x + offset.x + _movementOffset.x,
                                     position.y + offset.y + _movementOffset.y, 0) * GridSize;

            //设置深度
            if (SpriteRenderer != null)
                SpriteRenderer.sortingOrder = -position.y;
        }

        /// <summary>
        ///     移动到指定偏移位置
        ///     此移动由GridTransform完全控制
        /// </summary>
        /// <param name="to">移动的目的地</param>
        /// <param name="resistance"> 移动的阻力</param>
        public void Move(Vector2Int offset, float resistance)
        {
            MoveTo(position + offset, resistance);
        }

        /// <summary>
        ///     移动到指定位置
        ///     此移动由GridTransform完全控制
        /// </summary>
        /// <param name="to">移动的目的地</param>
        /// <param name="resistance"> 移动的阻力</param>
        public void MoveTo(Vector2Int to, float resistance)
        {
            if (IsMoving)
                throw new ArgumentException("A moving task is doing by this transform");

            if (MovingCoroutine != null)
                StopCoroutine(MovingCoroutine);
            MovingCoroutine = StartCoroutine(CoroutineMoveTo(to, resistance));
        }

        /// <summary>由GridTransform完全控制移动状态</summary>
        /// <param name="to">移动的目的地</param>
        /// <param name="resistance"> 移动的阻力</param>
        private IEnumerator CoroutineMoveTo(Vector2Int to, float resistance)
        {
            IsMoving = true;
            var lockedPos = position;
            var distance = new Vector2(to.x - lockedPos.x, to.y - lockedPos.y);

            _movementOffset = Math.Abs(distance.x) > Math.Abs(distance.y)
                ? new Vector2(_movementOffset.x, 0)
                : new Vector2(0, _movementOffset.y);

            while (true)
            {
                //移动目标
                var movingSpeed = 1 / resistance;

                if (float.IsInfinity(movingSpeed))
                {
                    lockedPos = to;
                }
                else
                {
                    _movementOffset += distance * movingSpeed * Time.deltaTime;

                    while (Mathf.Abs(_movementOffset.x) >= 1)
                    {
                        lockedPos += new Vector2Int(_movementOffset.x > 0 ? 1 : -1, 0);
                        _movementOffset -= new Vector2(_movementOffset.x > 0 ? 1 : -1, 0);
                    }

                    while (Mathf.Abs(_movementOffset.y) >= 1)
                    {
                        lockedPos += new Vector2Int(0, _movementOffset.y > 0 ? 1 : -1);
                        _movementOffset -= new Vector2(0, _movementOffset.y > 0 ? 1 : -1);
                    }
                }

                //锁定位置
                position = lockedPos;

                //计算当前加上移动偏移的位置
                var currentPos = _movementOffset + lockedPos;
                var deltaPos = currentPos - to;

                //如果移动到了目的地则停止
                if (deltaPos.x * distance.x >= 0 && deltaPos.y * distance.y >= 0)
                {
                    IsMoving = false;
                    yield return 0;
                    break;
                }

                yield return 0;
            }

            MovingCoroutine = null;
        }

        public void ResetMovement()
        {
            if (MovingCoroutine != null)
            {
                StopCoroutine(MovingCoroutine);
                MovingCoroutine = null;
            }

            _movementOffset = Vector2.zero;
            IsMoving = false;
        }
    }
}