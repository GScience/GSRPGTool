using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace RPGTool.Actions
{
    [Serializable]
    public class ActionSwitchScene : IQueueAction
    {
        [SerializeField]
        public string to;
        [SerializeField]
        public Vector2Int position;

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
