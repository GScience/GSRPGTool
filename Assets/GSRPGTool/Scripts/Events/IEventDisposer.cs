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
        void OnStart(Event gameEvent);

        /// <summary>
        /// 刷新的时候调用
        /// </summary>
        /// <returns>事件是否需要继续刷新</returns>
        bool Update(Event gameEvent);

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器GUI
        /// </summary>
        void OnGUI(Event gameEvent);
#endif
    }
}
