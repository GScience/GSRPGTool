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
            If(() => SaveManager.database.ContainsKey("Guard." + name + ".opendoor.finished"),
                () =>
                {
                    SetTriggerEnable(trigger, false);
                    AddMessage("接下来请开始属于你的冒险吧！");
                },
                () =>
                {
                    If(() => SaveManager.database.ContainsKey("Guard." + name + ".opendoor.usedKey"), () => { }, 
                    () =>
                    {
                        FaceToPlayer();
                        If(() => SaveManager.database.ContainsKey("Guard." + name + ".hello.finished"),
                            () =>
                            {
                                AddMessage("你又回来了吗？");
                            },
                            () =>
                            {
                                AddMessage("晚上好，欢迎来玩我的游戏。嗯，这个游戏还不是很成熟不知道能不能正常的来玩。");
                                ChangeFaceTo(GetComponent<Actor>(), Actor.Face.Up);
                                Wait(1);
                                FaceToPlayer();
                                AddMessage("你是想到门的那一边嘛？需要使用钥匙的，还好我带了钥匙。。。。。。\n" +
                                                                        "等待，我的钥匙呢！看起来是来的时候掉到草丛里了");

                                SetDatabaseValue("Guard." + name + ".hello.finished", 1);
                            });
                        PickupItem.AddHasItemCheckScripts(this, "钥匙", () =>
                        {
                            FaceToPlayer();
                            AddMessage("看来你找到我的钥匙了");
                            ChangeFaceTo(GetComponent<Actor>(), Actor.Face.Up);
                            Wait(1);
                            FaceToPlayer();
                            AddMessage("好的，钥匙已经插到门上了，但是这扇门好像还有一个特殊的装置！");
                            SetDatabaseValue("Guard." + name + ".opendoor.usedKey", 1);
                            ChangeFaceTo(GetComponent<Actor>(), Actor.Face.Down);

                        }, () =>
                        {
                            AddMessage("你帮我找一下钥匙把，应该在前边的草丛里！仔细搜查一遍应该可以找到的");
                            ChangeFaceTo(GetComponent<Actor>(), Actor.Face.Down);
                        });
                    });
                });

            If(() => SaveManager.database.ContainsKey("Guard." + name + ".opendoor.usedKey"), ()
                =>
            {
                MoveActor(GetComponent<Actor>(), Actor.Face.Left);
                ChangeFaceTo(GetComponent<Actor>(), Actor.Face.Right);
                AddMessage("你来看看这个机关");

                While(() => GameMapManager.gameMapManager.player.GridTransform.position != new Vector2Int(-1, 1),
                    () =>
                    {
                        If(() => GameMapManager.gameMapManager.player.GridTransform.position.y < 1, () =>
                            MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Up));
                        If(() => GameMapManager.gameMapManager.player.GridTransform.position.y > 1, () =>
                            MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Down));
                        If(() => GameMapManager.gameMapManager.player.GridTransform.position.x < -1, () =>
                            MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Right));
                        If(() => GameMapManager.gameMapManager.player.GridTransform.position.x > -1, () =>
                            MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Left));
                    });
                ChangeFaceTo(GameMapManager.gameMapManager.player, Actor.Face.Up);
                ShowSubwindow<Doordisk>("Doordiskwindow", () =>
                {
                    MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Right);
                    ChangeFaceTo(GameMapManager.gameMapManager.player, Actor.Face.Left);
                    MoveActor(GetComponent<Actor>(), Actor.Face.Right);
                    AddMessage("好了，门已经成功的打开了");
                    MoveActor(GetComponent<Actor>(), Actor.Face.Left);
                    ChangeFaceTo(GetComponent<Actor>(), Actor.Face.Right);
                    SetDatabaseValue("Guard." + name + ".opendoor.finished", 1);
                    SetDatabaseValue("Guard." + name + ".opendoor.usedKey", null);
                    ShowChapterInfo("序章 测试", "只是用来测试");
                }, () =>
                {
                    MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Right);
                    ChangeFaceTo(GameMapManager.gameMapManager.player, Actor.Face.Left);
                    MoveActor(GetComponent<Actor>(), Actor.Face.Right);
                    FaceToPlayer();
                    AddMessage("被难住了吗？待会儿再来吧");
                });
            }, () => { });
        }
    }
}