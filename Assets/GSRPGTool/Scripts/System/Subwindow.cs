using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGTool.System
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class Subwindow : MonoBehaviour
    {
        enum State
        {
            Loading,
            Loaded,
            Closing,
            Closed
        }

        private State _state;

        public CanvasGroup fader;
        public Canvas canvas;
        [HideInInspector] public bool Result { get; protected set; }

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            fader = GetComponent<CanvasGroup>();

            _state = State.Loading;
            fader.alpha = 0;
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <returns>是否已经关闭窗体</returns>
        public void Close()
        {
            if (_state == State.Closed)
                return;
            _state = State.Closing;
        }

        public bool Closed => _state == State.Closed;

        protected virtual void Update()
        {
            switch (_state)
            {
                case State.Loading:
                    fader.alpha += Time.deltaTime * 5.0f;
                    if (fader.alpha >= 1)
                    {
                        fader.alpha = 1;
                        _state = State.Loaded;
                    }

                    break;
                case State.Loaded:
                    break;
                case State.Closing:
                    fader.alpha -= Time.deltaTime * 5.0f;
                    if (fader.alpha <= 0)
                    {
                        fader.alpha = 0;
                        _state = State.Closed;
                    }

                    break;
                case State.Closed:
                    Destroy(gameObject, 0.1f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
