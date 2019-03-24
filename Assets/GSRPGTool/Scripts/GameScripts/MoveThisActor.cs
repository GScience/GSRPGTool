using System.IO;
using UnityEngine;

namespace RPGTool.GameScripts
{
    [RequireComponent(typeof(Actor))]
    public class MoveThisActor : GameScriptBase
    {
        public override void Do()
        {
            var actor = GetComponent<Actor>();
            MoveActor(actor, Actor.Face.Down);
            MoveActor(actor, Actor.Face.Down);
            /*AddMessage("欢迎来玩我的游戏", false);
            AddMessage("这里是游戏角色说出来的第一句话\n" +
                       "Hello World~", true);*/
            MoveActor(actor, Actor.Face.Down);
            MoveActor(actor, Actor.Face.Left);
            MoveActor(actor, Actor.Face.Left);
            MoveActor(actor, Actor.Face.Left);
            MoveActor(actor, Actor.Face.Up);
            //AddMessage("希望这个游戏不要咕咕咕哦~", true);
            MoveActor(actor, Actor.Face.Up);
            MoveActor(actor, Actor.Face.Up);
            MoveActor(actor, Actor.Face.Right);
            MoveActor(actor, Actor.Face.Right);
            MoveActor(actor, Actor.Face.Right);

            //SetPlayerPos("Scene 2", new Vector2Int(15, -10));
            JumpTo(0);
        }

        public override void OnLoad(BinaryReader stream)
        {
            base.OnLoad(stream);
            GetComponent<GridTransform>().ResetMovement();
        }
    }
}