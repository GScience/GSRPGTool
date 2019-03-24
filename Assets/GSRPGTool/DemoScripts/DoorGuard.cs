using System.IO;
using Assets.GSRPGTool.Scripts.GameScripts;
using RPGTool.GameScripts;
using RPGTool.Save;
using UnityEngine;

namespace RPGTool.DemoScripts
{
    [RequireComponent(typeof(Actor))]
    public class DoorGuard : GameScriptBase
    {
        public override void Do()
        {
            BlockInteraction(true);
            Check(() => SaveManager.database.ContainsKey("Guard." + name + ".hello.finished"), b =>
            {
                if (b)
                    return;
                ShowMessage.AddShowMessageScripts(this, "晚上好，欢迎来玩我的游戏。嗯，这个游戏还不是很成熟不知道能不能正常的来玩。");
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
                    ShowMessage.AddShowMessageScripts(this, "你帮我找一下钥匙把，应该在前边的草丛里！");
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
                }
                BlockInteraction(false);
            });
        }
    }
}