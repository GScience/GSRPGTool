using System;
using System.Collections.Generic;
using System.IO;
using RPGTool.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool.GameScrpits
{
    public abstract class GameScriptBase : MonoBehaviour, ISavable
    {
        /// <summary>
        ///     是否正在运行
        /// </summary>
        private bool _isRunning;

        /// <summary>
        ///     当前执行到的位置
        /// </summary>
        private uint _runPos;

        /// <summary>
        ///     游戏脚本执行队列
        /// </summary>
        private readonly List<ScriptAction> _actionList = new List<ScriptAction>();

        public override int GetHashCode()
        {
            init();

            var hashCode = 0;
            foreach (var action in _actionList)
                hashCode += action.name.GetHashCode();

            hashCode *= _actionList.Count;
            return hashCode;
        }
        public virtual void OnSave(BinaryWriter stream)
        {
            DataSaver.Save(GetHashCode(), stream);
            DataSaver.Save(_runPos, stream);
            DataSaver.Save(_isRunning, stream);
        }

        private void init()
        {
            if (_actionList.Count == 0)
                Do();
        }

        public virtual void OnLoad(BinaryReader stream)
        {
            var hashCode = DataLoader.Load<int>(stream);
            _runPos = DataLoader.Load<uint>(stream);
            _isRunning = DataLoader.Load<bool>(stream);

            if (hashCode != GetHashCode())
            {
                _runPos = 0;
                Debug.LogWarning("The sprite has been changed while running it. Reset pos to begin. ");
            }

            if (_isRunning)
                RunScript();
        }

        /// <summary>
        ///     判断分支执行
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <typeparam name="T">自动推断判断的类型</typeparam>
        /// <param name="checkFunc">获取判断值</param>
        /// <param name="doWhat">执行什么判断</param>
        /// <returns>当前事件Id</returns>
        public int Check<T>(Func<T> checkFunc, Action<T> doWhat)
        {
            _actionList.Add(new ScriptAction("Check")
            {
                OnStart = () => { doWhat(checkFunc()); }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     向指定方向移动角色一格
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <param name="actor">移动的角色</param>
        /// <param name="moveTo">移动的方向</param>
        /// <returns>当前事件Id</returns>
        public int MoveActor(Actor actor, Actor.Face moveTo, float speed)
        {
            var startPos = actor.GridTransform.position;
            _actionList.Add(new ScriptAction("MoveActor")
            {
                OnStart = () =>
                {
                    startPos = actor.GridTransform.position;
                    actor.speed = speed;
                    actor.expectNextMoveDirection = moveTo;
                },
                OnUpdate = () =>
                {
                    var offset = actor.GridTransform.position - startPos;

                    return offset == Actor.FaceToVector(moveTo);
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     跳转到指定位置
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <param name="where">指定位置</param>
        /// <returns>当前事件Id</returns>
        public int JumpTo(uint where)
        {
            _actionList.Add(new ScriptAction("JumpTo")
            {
                OnStart = () =>
                {
                    _runPos = where;
                    _actionList[(int) _runPos].OnStart();
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        /// 显示消息
        ///     <para>支持阻塞与非阻塞</para>
        /// </summary>
        /// <param name="msg">向显示的东西</param>
        /// <param name="block">是否阻挡事件的执行</param>
        /// <returns>当前事件Id</returns>
        public int AddMessage(string msg, bool block)
        {
            int pos = 0;
            _actionList.Add(new ScriptAction("AddMessage")
            {
                OnStart = () =>
                {
                    pos = GameMapManager.gameMapManager.mainDialog.AddMessage(msg);
                },
                OnUpdate = ()=>
                {
                    return !block || pos + msg.Length < GameMapManager.gameMapManager.mainDialog.ShownMsgPos;
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     设置玩家位置
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <param name="sceneName">场景名，为null或者为空则代表不改变场景</param>
        /// /// <param name="pos">新的位置</param>
        /// <returns>当前事件Id</returns>
        public int SetPlayerPos(string sceneName, Vector2Int pos)
        {
            _actionList.Add(new ScriptAction("SetPlayerPos")
            {
                OnStart = () =>
                {
                    SceneManager.LoadScene(sceneName);
                    SceneManager.sceneLoaded += (Scene arg0, LoadSceneMode arg1)=>
                    {
                        GameMapManager.gameMapManager.player.GridTransform.position = pos;
                    };
                }
            });
            return _actionList.Count - 1;
        }

        private void onSwitchScene(Scene arg0, LoadSceneMode arg1)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            if (_runPos >= _actionList.Count)
                return;
            var currentAction = _actionList[(int) _runPos];
            if (currentAction.OnUpdate == null || currentAction.OnUpdate())
                if (++_runPos < _actionList.Count)
                    _actionList[(int) _runPos].OnStart();
        }

        /// <summary>
        ///     开始运行脚本
        /// </summary>
        public void RunScript()
        {
            init();
            _actionList[(int) _runPos].OnStart();
            _isRunning = true;
        }

        public abstract void Do();

        /// <summary>
        ///     脚本Action
        /// </summary>
        private class ScriptAction
        {
            public delegate void StartDelegate();

            public delegate bool UpdateDelegate();

            /// <summary>
            ///     在开始时调用
            /// </summary>
            public StartDelegate OnStart;

            /// <summary>
            ///     在刷新时调用
            ///     如果返回true则代表刷新结束
            /// </summary>
            public UpdateDelegate OnUpdate;

            public readonly string name;

            public ScriptAction(string actionName)
            {
                name = actionName;
            }
        }
    }
}