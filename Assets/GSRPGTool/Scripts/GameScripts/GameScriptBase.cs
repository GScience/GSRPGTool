using System;
using System.Collections.Generic;
using System.IO;
using RPGTool.GameScripts.Triggers;
using RPGTool.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool.GameScripts
{
    public abstract partial class GameScriptBase : SavableBehaviour
    {
        /// <summary>
        ///     游戏脚本执行队列
        /// </summary>
        private readonly List<ScriptAction> _actionList = new List<ScriptAction>();

        /// <summary>
        ///     是否正在运行
        /// </summary>
        private bool _isRunning;

        /// <summary>
        ///     当前执行到的位置
        /// </summary>
        private uint _runPos;

        public override int GetHashCode()
        {
            if (_actionList.Count == 0)
                Do(null);

            var hashCode = 0;
            foreach (var action in _actionList)
            {
                if (action != null)
                    hashCode += action.name.GetHashCode();
                hashCode += 1;
            }

            hashCode *= _actionList.Count;
            return hashCode;
        }

        public override void OnSave(BinaryWriter stream)
        {
            DataSaver.Save(GetHashCode(), stream);
            DataSaver.Save(_runPos, stream);
            DataSaver.Save(_isRunning, stream);
        }

        public override void OnLoad(BinaryReader stream)
        {
            var hashCode = DataLoader.Load<int>(stream);
            _runPos = DataLoader.Load<uint>(stream);
            var isRunning = DataLoader.Load<bool>(stream);

            if (hashCode != GetHashCode())
            {
                _runPos = 0;
                isRunning = false;
                Debug.LogWarning("The sprite has been changed while running it. Reset pos to begin. ");
            }

            if (isRunning)
                RunScript();
            _isRunning = isRunning;
        }

        private void Update()
        {
            if (_runPos >= _actionList.Count || !_isRunning || _actionList[(int) _runPos] == null)
            {
                _isRunning = false;
                _actionList.Clear();
                _runPos = 0;
                return;
            }

            var currentAction = _actionList[(int) _runPos];
            if (currentAction.onUpdate == null || currentAction.onUpdate())
                if (++_runPos < _actionList.Count && _actionList[(int)_runPos] != null)
                    _actionList[(int) _runPos].onStart();
        }

        /// <summary>
        ///     开始运行脚本
        /// </summary>
        public void RunScript(TriggerBase trigger = null)
        {
            if (_isRunning)
                return;

            _actionList.Clear();
            Do(trigger);
            _actionList[(int) _runPos].onStart();
            _isRunning = true;
        }

        public abstract void Do(TriggerBase trigger);

        /// <summary>
        ///     脚本Action
        /// </summary>
        private class ScriptAction
        {
            public delegate void StartDelegate();

            public delegate bool UpdateDelegate();

            public readonly string name;

            /// <summary>
            ///     在开始时调用
            /// </summary>
            public StartDelegate onStart;

            /// <summary>
            ///     在刷新时调用
            ///     如果返回true则代表刷新结束
            /// </summary>
            public UpdateDelegate onUpdate;

            public ScriptAction(string actionName)
            {
                name = actionName;
            }
        }
    }
}