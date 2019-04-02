using RPGTool.Extension.ChapterDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.GameScripts
{
    public partial class GameScriptBase
    {
        public int ShowChapterInfo(string chapterName, string chapterInfo)
        {
            int pos = ShowSubwindow<ChapterWindow>("ChapterWindow", null, null, (subwindow) =>
            {
                subwindow.chapterName = chapterName;
                subwindow.chapterInfo = chapterInfo;
            });

            return pos;
        }
    }
}
