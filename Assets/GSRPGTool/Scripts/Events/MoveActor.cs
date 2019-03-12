using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGTool.Events
{
    public class MoveActor : IEventDisposer
    {
        public Actor actor;

        public void OnStart(Event gameEvent)
        {
        }

        public bool Update(Event gameEvent) 
        {
            return false;
        }

#if UNITY_EDITOR
        public void OnGUI(Event gameEvent)
        {
            EditorGUILayout.LabelField("移动角色：");
            actor = (Actor) EditorGUILayout.ObjectField("移动的角色", actor, typeof(Actor), true);
        }
#endif
    }
}
