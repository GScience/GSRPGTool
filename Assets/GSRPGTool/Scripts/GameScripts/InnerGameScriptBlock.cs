using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool.GameScripts.Triggers;
using RPGTool.Save;
using RPGTool.System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool.GameScripts
{
    public abstract partial class GameScriptBase
    {
        public int While(Func<bool> check, Action doWhat)
        {
            var pos = _actionList.Count;
            var whileStart = (uint)_actionList.Count;

            //插入判断头
            _actionList.Add(null);
            var bodyStartPos = (uint)_actionList.Count;
            //插入执行体
            doWhat();
            //执行体结尾
            _actionList.Add(new ScriptAction("WhileEnd")
            {
                onStart = () => SetPos(whileStart)
            });
            //整个while块结束
            var whileEndPos = (uint)_actionList.Count;

            //while头
            _actionList[pos] = new ScriptAction("While")
            {
                onStart = () =>
                {
                    if (!check())
                        SetPos(whileEndPos);
                }
            };
            return pos;
        }

        /// <summary>
        ///     条件判断
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <param name="check">执行什么判断</param>
        /// <param name="isTrue">真</param>
        /// <param name="isFalse">假</param>
        /// <returns>当前事件Id</returns>
        public int If(Func<bool> check, Action isTrue, Action isFalse = null)
        {
            //判断占位
            var pos = _actionList.Count;
            var checkPos = (uint)_actionList.Count;
            _actionList.Add(null);

            //真
            var trueBeginPos = (uint)_actionList.Count;
            isTrue?.Invoke();
            var trueEndPos = (uint)_actionList.Count;
            _actionList.Add(null);

            //假
            var falseBeginPos = (uint)_actionList.Count;
            isFalse?.Invoke();
            var falseEndPos = (uint)_actionList.Count;
            _actionList.Add(null);

            var outPos = (uint)_actionList.Count;

            //真跳出
            _actionList[(int)trueEndPos] = new ScriptAction("IfTrueEnd")
            {
                onStart = () => SetPos(outPos)
            };

            //假跳出
            _actionList[(int)falseEndPos] = new ScriptAction("IfFalseEnd")
            {
                onStart = () => SetPos(outPos)
            };

            //最初if判断
            _actionList[(int)checkPos] = new ScriptAction("If")
            {
                onStart = () => SetPos(check() ? trueBeginPos : falseBeginPos)
            };

            return pos;
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
        ///     向指定方向移动角色一格
        ///     <para>只支持以阻塞形式调用</para>
        /// </summary>
        /// <param name="actor">移动的角色</param>
        /// <param name="moveToFunc">移动的方向</param>
        /// <returns>当前事件Id</returns>
        public int MoveActorByExpression(Actor actor, Func<Actor.Face?> moveToFunc)
        {
            Actor.Face? moveTo = null;
            var startPos = actor.GridTransform.position;
            _actionList.Add(new ScriptAction("MoveActor")
            {
                onStart = () =>
                {
                    moveTo = moveToFunc();
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
        public int ChangeFaceTo(Actor actor, Actor.Face face)
        {
            _actionList.Add(new ScriptAction("ChangeFaceTo")
            {
                onStart = () => { actor.faceTo = face; }
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     改变面向方向
        /// </summary>
        /// <param name="actor">角色</param>
        /// <param name="faceFunc">面向</param>
        /// <returns>当前事件Id</returns>
        public int ChangeFaceToByExpression(Actor actor, Func<Actor.Face> faceFunc)
        {
            _actionList.Add(new ScriptAction("ChangeFaceTo")
            {
                onStart = () =>
                {
                    var face = faceFunc();
                    actor.faceTo = face;
                }
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
            _actionList.Add(new ScriptAction("SetTriggerEnable")
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
        ///     设置触发是否可用
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="enableFunc">是否可用</param>
        /// <returns>当前事件Id</returns>
        public int SetTriggerEnableByExpression(TriggerBase trigger, Func<bool> enableFunc)
        {
            _actionList.Add(new ScriptAction("SetTriggerEnable")
            {
                onStart = () =>
                {
                    var enable = enableFunc();
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
        ///     等待指定时间
        /// </summary>
        /// <param name="time">等待的时间（单位为秒）</param>
        /// <returns>当前事件Id</returns>
        public int WaitByExpression(Func<float> timeFunc)
        {
            float time = 0;
            _actionList.Add(new ScriptAction("Wait")
            {
                onStart = () => { time = timeFunc(); },
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
            _actionList.Add(new ScriptAction("SetDatabaseValue")
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
        ///     设置数据库变量
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="valueExpression">值表达式（在执行到的时候进行运算）</param>
        /// <returns>当前事件Id</returns>
        public int SetDatabaseValueByExpression(string key, Func<int?> valueExpression)
        {
            _actionList.Add(new ScriptAction("SetDatabaseValue")
            {
                onStart = () =>
                {
                    var value = valueExpression();
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
                onStart = () => SetPos(where)
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
                onStart = () => { pos = GameMapManager.gameMapManager.MainDialog.AddMessage(msg); },
                onUpdate = () => !block || pos + msg.Length < GameMapManager.gameMapManager.MainDialog.ShownMsgPos
            });
            return _actionList.Count - 1;
        }

        /// <summary>
        ///     显示子窗体
        /// </summary>
        /// <param name="isTrue">子窗口返回真</param>
        /// <param name="isFalse">子窗口返回假</param>
        /// <param name="subwindowPrefabName">子窗口预制体</param>
        /// <returns>当前事件Id</returns>
        public int ShowSubwindow<T>(string subwindowPrefabName, Action isTrue = null, Action isFalse = null, Action<T> onOpen = null) where T : Subwindow
        {
            var pos = _actionList.Count;
            var subwindowPrefab = Resources.Load<GameObject>(subwindowPrefabName);
            GameObject subwindowObj = null;
            _actionList.Add(new ScriptAction("ShowSubwindow")
            {
                onStart = () =>
                {
                    subwindowObj = Instantiate(subwindowPrefab);
                    onOpen?.Invoke(subwindowObj.GetComponent<T>());
                },
                onUpdate = () => subwindowObj.GetComponent<Subwindow>().Closed
            });
            If(() => subwindowObj.GetComponent<Subwindow>().Result, isTrue, isFalse);

            return pos;
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
                        GameMapManager.gameMapManager.Fader.FadeIn();
                        GameMapManager.gameMapManager.player.GridTransform.position = pos;
                        GameMapManager.gameMapManager.player.faceTo = faceTo;
                    };

                    GameMapManager.gameMapManager.Fader.FadeOut();
                },
                onUpdate = () =>
                {
                    if (GameMapManager.gameMapManager.Fader.IsFinished)
                    {
                        //移动到下一个
                        if (string.IsNullOrEmpty(sceneName))
                        {
                            GameMapManager.gameMapManager.Fader.FadeIn();
                            return true;
                        }
                        ++_runPos;
                        GameMapManager.gameMapManager.SwitchToScene(sceneName);
                    }

                    return false;
                }
            });
            return _actionList.Count - 1;
        }
    }
}
