using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using RPGTool.Physical;
using RPGTool.Save;
using System.IO;

namespace RPGTool.GameScrpits
{
    public abstract class GameScriptBase : MonoBehaviour, ISavable
    {
        /// <summary>
        /// 脚本Action
        /// </summary>
        private class ScriptAction
        {
            public delegate bool UpdateDelegate();
            public delegate void StartDelegate();

            /// <summary>
            /// 在开始时调用
            /// </summary>
            public StartDelegate OnStart;

            /// <summary>
            /// 在刷新时调用
            /// 如果返回true则代表刷新结束
            /// </summary>
            public UpdateDelegate OnUpdate;
        }

        /// <summary>
        /// 当前执行到的位置
        /// </summary>
        private uint _runPos = 0;

        /// <summary>
        /// 游戏脚本执行队列
        /// </summary>
        private List<ScriptAction> actionList = new List<ScriptAction>();

        /// <summary>
        /// 判断分支执行
        /// </summary>
        /// <typeparam name="T">自动推断判断的类型</typeparam>
        /// <param name="checkFunc">获取判断值</param>
        /// <param name="doWhat">执行什么判断</param>
        /// <returns>当前事件Id</returns>
        public int Check<T>(Func<T> checkFunc, Action<T> doWhat)
        {
            actionList.Add(new ScriptAction()
            {
                OnStart = () =>
                {
                    doWhat(checkFunc());
                }
            });
            return actionList.Count - 1;
        }

        /// <summary>
        /// 向指定方向移动角色一格
        /// </summary>
        /// <param name="actor">移动的角色</param>
        /// <param name="moveTo">移动的方向</param>
        /// <returns>当前事件Id</returns>
        public int MoveActor(Actor actor, Actor.Face moveTo, float speed)
        {
            var rigidbody = actor.GetComponent<TileRigidbody>();
            var startPos = actor.GridTransform.position;
            actionList.Add(new ScriptAction()
            {
                OnStart = () =>
                {
                    startPos = actor.GridTransform.position;
                    rigidbody.speed = speed;
                    rigidbody.SetMoveDirection(moveTo);
                },
                OnUpdate = () =>
                {
                    var offset = actor.GridTransform.position - startPos;
                    switch (moveTo)
                    {
                        case Actor.Face.Up:
                            if (offset == Vector2Int.up)
                            {
                                rigidbody.SetMoveDirection(null);
                                return true;
                            }
                            break;
                        case Actor.Face.Down:
                            if (offset == Vector2Int.down)
                            {
                                rigidbody.SetMoveDirection(null);
                                return true;
                            }
                            break;
                        case Actor.Face.Left:
                            if (offset == Vector2Int.left)
                            {
                                rigidbody.SetMoveDirection(null);
                                return true;
                            }
                            break;
                        case Actor.Face.Right:
                            if (offset == Vector2Int.right)
                            {
                                rigidbody.SetMoveDirection(null);
                                return true;
                            }
                            break;
                    }
                    rigidbody.SetMoveDirection(moveTo);
                    return false;
                }
            });
            return actionList.Count - 1;
        }
        /// <summary>
        /// 跳转到指定位置
        /// </summary>
        /// <param name="where">指定位置</param>
        /// <returns>当前事件Id</returns>
        public int JumpTo(uint where)
        {
            actionList.Add(new ScriptAction()
        {
            OnStart = () =>
            {
                _runPos = where;
                actionList[(int)_runPos].OnStart();
            }
            });
            return actionList.Count - 1;
        }

        void Update()
        {
            if (_runPos >= actionList.Count)
                return;
            var currentAction = actionList[(int)_runPos];
            if (currentAction.OnUpdate == null || currentAction.OnUpdate())
                if (++_runPos < actionList.Count)
                    actionList[(int)_runPos].OnStart();
        }

        /// <summary>
        /// 开始运行脚本
        /// </summary>
        public void RunScript()
        {
            actionList.Clear();
            Do();
            actionList[(int)_runPos].OnStart();
        }

        public abstract void Do();

        public virtual void OnSave(BinaryWriter stream)
        {
            DataSaver.Save(_runPos, stream);
        }

        public virtual void OnLoad(BinaryReader stream)
        {
            _runPos = DataLoader.Load<uint>(stream);
            RunScript();
        }
    }
}