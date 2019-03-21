using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGTool.System
{
    [RequireComponent(typeof(Image))]
    public class Dialog : MonoBehaviour
    {
        /// <summary>
        /// 显示的消息
        /// </summary>
        public string Message { get; private set; } = "";

        /// <summary>
        /// 是否暂停打印文字
        /// </summary>
        public bool PrintingPaused { get; private set; } = false;

        /// <summary>
        /// 打印延迟
        /// </summary>
        private float _mDeltaTime = 0;

        /// <summary>
        /// 现在显示的消息所在的位置
        /// </summary>
        public int ShownMsgPos { get; private set; } = 0;

        public float printSpeed = 10;

        public Text TextBox { get; private set; }
        public Image DialogBackground { get; private set; }

        /// <summary>
        /// 增加显示的消息
        /// </summary>
        /// <param name="msg">要显示什么</param>
        /// <returns>增加的消息的位置</returns>
        public int AddMessage(string msg)
        {
            int pos = ShownMsgPos + Message.Length;
            Message += msg + "\f";

            return pos;
        }

        private void Awake()
        {
            TextBox = GetComponentInChildren<Text>();
            DialogBackground = GetComponent<Image>();
        }
        void Start()
        {
            DialogBackground.enabled = false;
        }

        void Update()
        {
            //暂停时处理暂停相关
            if (PrintingPaused)
            {
                if (Input.anyKeyDown)
                {
                    TextBox.text = "";
                    PrintingPaused = false;
                    _mDeltaTime = 0;

                    //移除分页符
                    if (Message[0] == '\f')
                    {
                        Message = Message.Remove(0, 1);
                        ++ShownMsgPos;
                    }
                }
                return;
            }

            //等待刷新时间
            _mDeltaTime += Time.deltaTime * (Input.anyKey ? 10 : 1);
            if (_mDeltaTime < 1 / printSpeed)
                return;

            //如果空了隐藏对话框
            if (Message.Length == 0)
            {
                DialogBackground.enabled = false;
                return;
            }
            else
                DialogBackground.enabled = true;


            //如果是换页，移除换页符并暂停
            if (Message[0] == '\f')
            {
                 PrintingPaused = true;
                return;
            }

            //增加一个文字进入
            _mDeltaTime %= 1 / printSpeed;

            var generator = new TextGenerator();
            var newText = TextBox.text + Message[0];
            generator.Populate(newText, TextBox.GetGenerationSettings(TextBox.rectTransform.rect.size));

            if (newText.Length > generator.characterCountVisible)
            {
                PrintingPaused = true;
                _mDeltaTime = 0;
                return;
            }

            //同步到文本框
            ++ShownMsgPos;
            TextBox.text = newText;
            Message = Message.Remove(0, 1);
        }
    }
}