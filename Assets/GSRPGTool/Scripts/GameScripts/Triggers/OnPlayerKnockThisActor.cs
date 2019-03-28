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
                    return GameMapManager.GetInput(PlayerInput.Up);
                case Actor.Face.Down:
                    return GameMapManager.GetInput(PlayerInput.Down);
                case Actor.Face.Left:
                    return GameMapManager.GetInput(PlayerInput.Left);
                case Actor.Face.Right:
                    return GameMapManager.GetInput(PlayerInput.Right);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}