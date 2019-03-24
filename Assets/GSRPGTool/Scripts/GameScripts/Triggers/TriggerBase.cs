using System.IO;
using RPGTool.Save;
using UnityEngine;

namespace RPGTool.GameScripts.Triggers
{
    [RequireComponent(typeof(GameScriptBase))]
    public class TriggerBase : SavableBehaviour
    {
        [Tooltip("需要触发的脚本")] public GameScriptBase gameScript;

        [Tooltip("是否只触发一次")] public bool onlyOnce = true;

        private void LateUpdate()
        {
            if (Check())
            {
                gameScript.RunScript();
                enabled = !onlyOnce;
            }
        }

        /// <summary>
        ///     判断是否满足触发条件
        /// </summary>
        /// <returns>返回true则表示满足条件</returns>
        protected virtual bool Check()
        {
            return false;
        }

        public override void OnSave(BinaryWriter stream)
        {
            DataSaver.Save(enabled, stream);
        }

        public override void OnLoad(BinaryReader stream)
        {
            enabled = DataLoader.Load<bool>(stream);
        }
    }
}