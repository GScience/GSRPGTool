using RPGTool.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RPGTool.Extension.ChapterDisplay
{
    /// <summary>
    /// 章节窗体
    /// </summary>
    public class ChapterWindow : Subwindow
    {
        /// <summary>
        /// 章节名称
        /// </summary>
        public string chapterName;

        /// <summary>
        /// 章节信息
        /// </summary>
        public string chapterInfo;

        private Text _chapterNameText;
        private Text _chapterInfoText;

        private float _lifetime = 5;

        protected override void Awake()
        {
            base.Awake();
            var texts = GetComponentsInChildren<Text>();

            foreach (var text in texts)
                switch (text.name)
                {
                    case "ChapterNameText":
                        _chapterNameText = text;
                        break;
                    case "ChapterInfoText":
                        _chapterInfoText = text;
                        break;
                    default:
                        break;
                }
        }
        private void Start()
        {
            _chapterNameText.text = chapterName;
            _chapterInfoText.text = chapterInfo;
        }
        protected override void Update()
        {
            base.Update();
            if (_lifetime < 0)
                Close();
            _lifetime -= Time.deltaTime;
        }
    }
}
