using UnityEngine;
using UnityEditor;
using System.IO;

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
            MoveActor(actor, Actor.Face.Down, 2);
            MoveActor(actor, Actor.Face.Left, 2);
            MoveActor(actor, Actor.Face.Left, 2);
            MoveActor(actor, Actor.Face.Left, 2);
            MoveActor(actor, Actor.Face.Up, 2);
            MoveActor(actor, Actor.Face.Up, 2);
            MoveActor(actor, Actor.Face.Up, 2);
            MoveActor(actor, Actor.Face.Right, 2);
            MoveActor(actor, Actor.Face.Right, 2);
            MoveActor(actor, Actor.Face.Right, 2);
            JumpTo(0);
        }


        void Start()
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