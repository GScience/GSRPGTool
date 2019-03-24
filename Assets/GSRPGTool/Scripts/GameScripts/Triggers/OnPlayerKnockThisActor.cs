using System;
using UnityEngine;

namespace RPGTool.GameScripts.Triggers
{
    public class OnPlayerKnockThisActor : OnPlayerMoveTo
    {
        [Tooltip("向哪里撞")] public Actor.Face knockTo;

        protected override bool Check()
        {
            if (!base.Check() || 
                GameMapManager.gameMapManager.player.GridTransform.MovingCoroutine != null)
                return false;

            switch (knockTo)
            {
                case Actor.Face.Up:
                    return Input.GetKey(KeyCode.W);
                case Actor.Face.Down:
                    return Input.GetKey(KeyCode.S);
                case Actor.Face.Left:
                    return Input.GetKey(KeyCode.A);
                case Actor.Face.Right:
                    return Input.GetKey(KeyCode.D);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}