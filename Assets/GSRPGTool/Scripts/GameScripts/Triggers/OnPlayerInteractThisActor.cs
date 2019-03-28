using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGTool.GameScripts.Triggers
{
    [RequireComponent(typeof(Actor))]
    public class OnPlayerInteractThisActor : TriggerBase
    {
        protected override bool Check()
        {
            if (!GameMapManager.GetInput(PlayerInput.Interaction))
                return false;

            var player = GameMapManager.gameMapManager.player;
            return Actor.FaceToVector(player.faceTo) ==
                   GetComponent<Actor>().GridTransform.position - player.GridTransform.position;
        }
    }
}
