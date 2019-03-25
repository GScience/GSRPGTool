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
            AddShowMessageScripts(this, message, autoTurnToPlayer, autoTurnBack);
        }

        public static int AddShowMessageScripts(GameScriptBase script, string message, 
            bool autoTurnBack = true, bool autoTurnToPlayer = true)
        {
            var actor = script.gameObject.GetComponent<Actor>();

            if (actor)
            {
                var actorFaceTo = actor.faceTo;
                var id = -1;

                if (autoTurnToPlayer)
                    id = script.FaceToPlayer();
                var id2 = script.AddMessage(message);
                if (id == -1)
                    id = id2;
                if (autoTurnBack)
                    script.ChangeFace(actor, actorFaceTo);

                return id;
            }

            return script.AddMessage(message);
        }
    }
}
