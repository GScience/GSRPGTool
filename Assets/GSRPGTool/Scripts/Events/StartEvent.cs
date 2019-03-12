using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGTool.Events
{
    /// <summary>
    /// 开始事件
    /// </summary>
    [Serializable]
    public class StartEvent : IEventDisposer
    {
        public void OnStart(Event gameEvent)
        {
        }

        public bool Update(Event gameEvent)
        {
            return true;
        }
#if UNITY_EDITOR
        public void OnGUI(Event gameEvent)
        {
            EditorGUILayout.LabelField("开始");
        }
#endif
    }
}
