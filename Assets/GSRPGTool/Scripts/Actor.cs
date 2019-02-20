using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSRPGTool;
using GSRPGTool.Physical;
using UnityEngine;

namespace RPGTool
{
    [ExecuteInEditMode,RequireComponent(typeof(SpriteRenderer))]
    public class Actor : MonoBehaviour
    {
        /// <summary>
        /// 动画速度
        /// </summary>
        public float animationSpeed = 1;

        /// <summary>
        /// 角色面向
        /// </summary>
        public enum Face
        {
            Up = 3, Down = 0, Left = 1, Right = 2
        }

        /// <summary>
        /// 角色的图像
        /// </summary>
        public Texture2D actorTexture;

        public int pixelPerTexture = 32;

        public Vector2 pivot = new Vector2(0.5f,0.25f);

        /// <summary>
        /// 角色的面向
        /// </summary>
        public Face faceTo = Face.Down;

        /// <summary>
        /// 角色图像索引
        /// </summary>
        private int _movingStage;

        private float _deltaTimeFromChangeIndex = 3;

        /// <summary>
        /// 分割后的角色图像
        /// </summary>
        private readonly Sprite[] _actorSprites = new Sprite[12];

        public GridTransform GridTransform { get; private set;}
        public SpriteRenderer SpriteRenderer { get; private set; }
        public TileCollider TileCollider { get; private set; }

        void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            //分割图像
            for (var i = 0; i < 12; ++i)
                _actorSprites[i] = GetSprite(i);

            //设置纹理显示模式
            actorTexture.filterMode = FilterMode.Point;
            GridTransform = GetComponent<GridTransform>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            TileCollider = GetComponent<TileCollider>();
        }

        Sprite GetSprite(int index)
        {
            return Sprite.Create(actorTexture,
                new Rect(
                    index % 3 / 3.0f * actorTexture.width, (3 - index / 3) / 4.0f * actorTexture.height,
                    actorTexture.width / 3.0f, actorTexture.height / 4.0f),
                pivot,
                pixelPerTexture);
        }
#if UNITY_EDITOR
        private Face _nowSpriteFace = Face.Up;
#endif
        void Update()
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
                SwitchNextSprite(!GridTransform || !GridTransform.IsMoving);
        }

        /// <summary>
        /// 获取下一步的Sprite
        /// </summary>
        /// <param name="isStop">是否停止</param>
        /// <returns></returns>
        public void SwitchNextSprite(bool isStop)
        {
            _deltaTimeFromChangeIndex += Time.deltaTime;
            var timePerFrame = 1.0f / animationSpeed / 3.0f;

            if (isStop)
            {
                SpriteRenderer.sprite = _actorSprites[1 + (int) faceTo * 3];
                _deltaTimeFromChangeIndex = 0;
                _movingStage = 0;
            }
            else if (_deltaTimeFromChangeIndex >= timePerFrame)
            {
                _deltaTimeFromChangeIndex %= timePerFrame;

                ++_movingStage;
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
                SpriteRenderer.sprite = _actorSprites[imageIndex + (int)faceTo * 3];
            }
        }
    }
}
