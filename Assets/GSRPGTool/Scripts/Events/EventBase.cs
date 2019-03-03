using System;

namespace RPGTool.Actions
{
    [Serializable]
    public abstract class EventBase
    {
        /// <summary>
        /// 是否阻塞事件系统
        /// </summary>
        public bool block;

        public abstract void OnStart();
        public abstract void OnFinished();
        public abstract bool IsFinished();

#if UNITY_EDITOR
        /// <summary>
        /// 绘制事件编辑UI
        /// </summary>
        public abstract void OnGUI();
#endif
    }
}