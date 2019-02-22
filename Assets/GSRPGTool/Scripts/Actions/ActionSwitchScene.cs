using System;
using UnityEngine;

namespace RPGTool.Actions
{
    [Serializable]
    public class ActionSwitchScene : IQueueAction
    {
        [SerializeField] public Vector2Int position;

        [SerializeField] public string to;

        public void OnStart()
        {
        }

        public void OnFinished()
        {
        }

        public bool IsFinished()
        {
            return true;
        }
    }
}