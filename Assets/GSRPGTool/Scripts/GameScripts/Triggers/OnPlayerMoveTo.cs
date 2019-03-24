using UnityEngine;

namespace RPGTool.GameScripts.Triggers
{
    public class OnPlayerMoveTo : TriggerBase
    {
        [Tooltip("是否是相对坐标")] public bool isRelaventPos = false;

        [Tooltip("坐标")] public Vector2Int pos;

        private void Awake()
        {
            if (isRelaventPos)
                pos += GetComponent<GridTransform>().position;
        }

        protected override bool Check()
        {
            return GameMapManager.gameMapManager.PlayerPosition == pos;
        }
    }
}