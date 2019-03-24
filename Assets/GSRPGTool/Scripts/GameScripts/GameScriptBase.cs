using System;
using System.Collections.Generic;
using System.IO;
using RPGTool.GameScripts.Triggers;
using RPGTool.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool.GameScripts
{
    public abstract class GameScriptBase : SavableBehaviour
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
                onStart = () =>
                {
                    //记录当前状态
                    var nowPos = _runPos;
                    var additionStartPos = _actionList.Count + 1;

                    //加入终止
                    _actionList.Add(null);

                    //在结尾加入新的需要执行的内容
                    doWhat(checkFunc());

                    //跳转到结尾
                    _runPos = (uint) additionStartPos;

                    //在结尾插入往回的跳转
                    JumpTo(nowPos + 1);

                    //开始运行
                    _actionList[(int) _runPos].onStart();
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     面向玩家
        /// </summary>
        /// <typeparam name="T">自动推断判断的类型</typeparam>
        /// <param name="checkFunc">获取判断值</param>
        /// <param name="doWhat">执行什么判断</param>
        /// <returns>当前事件Id</returns>
        public int FaceToPlayer()
        {
            var actor = GetComponent<Actor>();

            _actionList.Add(new ScriptAction("FaceToPlayer")
            {
                onStart = () =>
                {
                    switch (GameMapManager.gameMapManager.player.faceTo)
                    {
                        case Actor.Face.Up:
                            actor.faceTo = Actor.Face.Down;
                            break;
                        case Actor.Face.Down:
                            actor.faceTo = Actor.Face.Up;
                            break;
                        case Actor.Face.Left:
                            actor.faceTo = Actor.Face.Right;
                            break;
                        case Actor.Face.Right:
                            actor.faceTo = Actor.Face.Left;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
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
        public int MoveActor(Actor actor, Actor.Face? moveTo)
        {
            var startPos = actor.GridTransform.position;
            _actionList.Add(new ScriptAction("MoveActor")
            {
                onStart = () =>
                {
                    startPos = actor.GridTransform.position;
                    actor.expectNextMoveDirection = moveTo;
                },
                onUpdate = () =>
                {
                    if (moveTo == null)
                        return true;

                    var offset = actor.GridTransform.position - startPos;

                    return offset == Actor.FaceToVector(moveTo.Value);
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     设置角色信息
        /// </summary>
        /// <param name="actor">角色</param>
        /// <param name="isKinematic">是否仅运动学</param>
        /// <param name="speed">速度</param>
        /// <returns>当前事件Id</returns>
        public int SetActor(Actor actor, bool isKinematic, float speed = -1)
        {
            _actionList.Add(new ScriptAction("SetActor")
            {
                onStart = () =>
                {
                    actor.isKinematic = isKinematic;
                    if (speed > 0)
                        actor.speed = speed;
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     改变面向方向
        /// </summary>
        /// <param name="actor">角色</param>
        /// <param name="face">面向</param>
        /// <returns>当前事件Id</returns>
        public int ChangeFace(Actor actor, Actor.Face face)
        {
            _actionList.Add(new ScriptAction("ChangeFace")
            {
                onStart = () => { actor.faceTo = face; }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     设置触发是否可用
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="enable">是否可用</param>
        /// <returns>当前事件Id</returns>
        public int SetTriggerEnable(TriggerBase trigger, bool enable)
        {
            _actionList.Add(new ScriptAction("ChangeFace")
            {
                onStart = () =>
                {
                    if (trigger != null)
                        trigger.enabled = enable;
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     屏蔽交互事件
        /// </summary>
        /// <param name="block">是否屏蔽</param>
        /// <returns>当前事件Id</returns>
        public int BlockInteraction(bool block)
        {
            _actionList.Add(new ScriptAction("BlockInteraction")
            {
                onStart = () =>
                {
                    GameMapManager.gameMapManager.IgnorePlayerInteract = block;
                    GameMapManager.gameMapManager.player.expectNextMoveDirection = null;
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     等待指定时间
        /// </summary>
        /// <param name="time">等待的时间（单位为秒）</param>
        /// <returns>当前事件Id</returns>
        public int Wait(float time)
        {
            _actionList.Add(new ScriptAction("Wait")
            {
                onStart = () => { },
                onUpdate = () => (time -= Time.deltaTime) < 0
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     设置数据库变量
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">值</param>
        /// <returns>当前事件Id</returns>
        public int SetDatabaseValue(string key, int? value)
        {
            _actionList.Add(new ScriptAction("Wait")
            {
                onStart = () =>
                {
                    if (value == null)
                        SaveManager.database.Remove(key);
                    else
                    SaveManager.database[key] = value.Value;
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
                onStart = () =>
                {
                    _runPos = where;
                    if (_actionList[(int)_runPos] != null)
                        _actionList[(int) _runPos].onStart();
                }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     显示消息
        ///     <para>支持阻塞与非阻塞</para>
        /// </summary>
        /// <param name="msg">向显示的东西</param>
        /// <param name="block">是否阻挡事件的执行</param>
        /// <returns>当前事件Id</returns>
        public int AddMessage(string msg, bool block = true)
        {
            var pos = 0;
            _actionList.Add(new ScriptAction("AddMessage")
            {
                onStart = () => { pos = GameMapManager.gameMapManager.mainDialog.AddMessage(msg); },
                onUpdate = () => !block || pos + msg.Length < GameMapManager.gameMapManager.mainDialog.ShownMsgPos
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     设置玩家位置
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <param name="sceneName">场景名，为null或者为空则代表不改变场景</param>
        /// <param name="faceTo">面向位置</param>
        /// <param name="pos">新的位置</param>
        /// <returns>当前事件Id</returns>
        public int SetPlayerPos(string sceneName, Vector2Int pos, Actor.Face faceTo)
        {
            _actionList.Add(new ScriptAction("SetPlayerPos")
            {
                onStart = () =>
                {
                    SceneManager.sceneLoaded += (arg0, arg1) =>
                    {
                        GameMapManager.gameMapManager.fader.FadeIn();
                        GameMapManager.gameMapManager.player.GridTransform.position = pos;
                        GameMapManager.gameMapManager.player.faceTo = faceTo;
                    };

                    GameMapManager.gameMapManager.fader.FadeOut();
                },
                onUpdate = () =>
                {
                    if (GameMapManager.gameMapManager.fader.IsFinished)
                    {
                        //移动到下一个
                        if (string.IsNullOrEmpty(sceneName))
                        {
                            GameMapManager.gameMapManager.fader.FadeIn();
                            return true;
                        }
                        ++_runPos;
#if UNITY_EDITOR
                        if (SaveManager.saveManager)
#endif
                            SaveManager.saveManager.SaveCurrentScene();
                        SceneManager.LoadScene(sceneName);
                    }

                    return false;
                }
            });
            return _actionList.Count - 1;
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