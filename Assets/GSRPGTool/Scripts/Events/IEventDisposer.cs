using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.Events
{
    public interface IEventDisposer
    {
        /// <summary>
        /// 在事件开始的时候调用
        /// </summary>
        void OnStart();

        /// <summary>
        /// 刷新的时候调用
        /// </summary>
        /// <returns>事件是否结束</returns>
        bool Update();

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器GUI
        /// </summary>
        void OnGUI();
#endif
    }
}
