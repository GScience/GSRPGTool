using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool.GameScripts;
using RPGTool.GameScripts.Triggers;
using UnityEngine;

namespace RPGTool.GameScripts
{
    public class ShowMessage : GameScriptBase
    {
        public string message = "";

        [Tooltip("是否自动转向玩家")]
        public bool autoTurnToPlayer = true;
        [Tooltip("完成对话是否自动转回来")]
        public bool autoTurnBack = true;

        public override void Do(TriggerBase trigger)
        {
            var actor = gameObject.GetComponent<Actor>();
            var actorFaceTo = actor.faceTo;
            if (autoTurnToPlayer)
                FaceToPlayer();
            AddMessage(message);
            if (autoTurnBack)
                ChangeFaceTo(actor, actorFaceTo);
        }
    }
}
