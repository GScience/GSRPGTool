using RPGTool.GameScripts.Triggers;
using UnityEngine;

namespace RPGTool.GameScripts
{
    [RequireComponent(typeof(Actor))]
    public class OpenAndMovePlayer : GameScriptBase
    {
        private Actor _doorActor;

        /// <summary>
        ///     移动到的场景名
        /// </summary>
        public string moveToScene;

        /// <summary>
        ///     坐标
        /// </summary>
        public Vector2Int pos;

        private void Awake()
        {
            blockPlayerInteraction = true;
            _doorActor = GetComponent<Actor>();
        }

        public override void Do(TriggerBase trigger)
        {
            MoveActor(GameMapManager.gameMapManager.player, null);
            ChangeFaceTo(GameMapManager.gameMapManager.player, Actor.Face.Up);
            ChangeFaceTo(_doorActor, Actor.Face.Down);
            Wait(0.1f);
            ChangeFaceTo(_doorActor, Actor.Face.Left);
            Wait(0.1f);
            ChangeFaceTo(_doorActor, Actor.Face.Right);
            Wait(0.1f);
            ChangeFaceTo(_doorActor, Actor.Face.Up);
            Wait(0.1f);
            SetActor(GameMapManager.gameMapManager.player, true);
            MoveActor(GameMapManager.gameMapManager.player, Actor.Face.Up);
            SetActor(GameMapManager.gameMapManager.player, false);
            ChangeFaceTo(_doorActor, Actor.Face.Up);
            Wait(0.1f);
            ChangeFaceTo(_doorActor, Actor.Face.Right);
            Wait(0.1f);
            ChangeFaceTo(_doorActor, Actor.Face.Left);
            Wait(0.1f);
            ChangeFaceTo(_doorActor, Actor.Face.Down);
            Wait(0.1f);
            SetPlayerPos(moveToScene, pos, Actor.Face.Down);
        }
    }
}