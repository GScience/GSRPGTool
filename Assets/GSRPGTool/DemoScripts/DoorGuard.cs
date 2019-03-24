using System.IO;
using RPGTool.GameScripts;
using RPGTool.GameScripts.Triggers;
using RPGTool.Save;
using UnityEngine;

namespace RPGTool.DemoScripts
{
    [RequireComponent(typeof(Actor))]
    public class DoorGuard : GameScriptBase
    {
        public override void Do(TriggerBase trigger)
        {
            BlockInteraction(true);
            Check(() => SaveManager.database.ContainsKey("Guard." + name + ".opendoor.finished"), opened =>
            {
                if (opened)
                {
                    SetTriggerEnable(trigger, false);
                    ShowMessage.AddShowMessageScripts(this, "接下来请开始属于你的冒险吧！", false, false);
                }
                else
                {
                    Check(() => SaveManager.database.ContainsKey("Guard." + name + ".hello.finished"), b =>
                    {
                        if (b)
                        {
                            ShowMessage.AddShowMessageScripts(this, "你又回来了吗？", false, false);
                            return;
                        }

                        ShowMessage.AddShowMessageScripts(this, "晚上好，欢迎来玩我的游戏。嗯，这个游戏还不是很成熟不知道能不能正常的来玩。", false);
                        ChangeFace(GetComponent<Actor>(), Actor.Face.Up);
                        Wait(1);
                        ShowMessage.AddShowMessageScripts(this, "你是想到门的那一边嘛？需要使用钥匙的，还好我带了钥匙。。。。。。\n" +
                                                                "等待，我的钥匙呢！看起来是来的时候掉到草丛里了", false);

                        SetDatabaseValue("Guard." + name + ".hello.finished", 1);
                    });
                    PickupItem.AddItemCheckScripts(this, "钥匙", i =>
                    {
                        if (i == 0)
                        {
                            ShowMessage.AddShowMessageScripts(this, "你帮我找一下钥匙把，应该在前边的草丛里！", false, false);
                        }
                        else
                        {
                            ShowMessage.AddShowMessageScripts(this, "看来你找到我的钥匙了");
                            ChangeFace(GetComponent<Actor>(), Actor.Face.Up);
                            Wait(1);
                            FaceToPlayer();
                            ShowMessage.AddShowMessageScripts(this, "好的，门已经打开了！");
                            MoveActor(GetComponent<Actor>(), Actor.Face.Left);
                            ChangeFace(GetComponent<Actor>(), Actor.Face.Right);
                            SetDatabaseValue("Guard." + name + ".opendoor.finished", 1);
                        }
                    });
                }
            });
            BlockInteraction(false);
        }
    }
}