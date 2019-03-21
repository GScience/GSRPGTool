using System.IO;
using UnityEngine;

namespace RPGTool.GameScrpits
{
    [RequireComponent(typeof(Actor))]
    public class MoveThisActor : GameScriptBase
    {
        public override void Do()
        {
            var actor = GetComponent<Actor>();
            MoveActor(actor, Actor.Face.Down, 2);
            MoveActor(actor, Actor.Face.Down, 2);
            AddMessage("欢迎来玩我的游戏", false);
            AddMessage("这里是游戏角色说出来的第一句话\n" +
                "Hello World~", true);
            MoveActor(actor, Actor.Face.Down, 2);
            MoveActor(actor, Actor.Face.Left, 2);
            MoveActor(actor, Actor.Face.Left, 2);
            MoveActor(actor, Actor.Face.Left, 2);
            MoveActor(actor, Actor.Face.Up, 2);
            AddMessage("希望这个游戏不要咕咕咕哦~", true);
            MoveActor(actor, Actor.Face.Up, 2);
            MoveActor(actor, Actor.Face.Up, 2);
            MoveActor(actor, Actor.Face.Right, 2);
            MoveActor(actor, Actor.Face.Right, 2);
            MoveActor(actor, Actor.Face.Right, 2);
            JumpTo(0);
        }


        private void Start()
        {
            RunScript();
        }

        public override void OnLoad(BinaryReader stream)
        {
            base.OnLoad(stream);
            GetComponent<GridTransform>().ResetMovement();
        }
    }
}