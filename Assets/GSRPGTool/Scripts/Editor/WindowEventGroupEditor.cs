using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool.Events;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGTool.Editor
{
    public class WindowEventEditor : EditorWindow
    {
        public GameEventGroup eventGroup;

        void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");

            if (eventGroup.eventList == null || eventGroup.eventList.Count == 0)
                eventGroup.AddEvent<StartEvent>();

            var gameEvent = eventGroup.GetEventByIndex(0);

            while (gameEvent != null)
            {
                EditorGUILayout.BeginVertical("box");
                gameEvent.OnGUI();
                EditorGUILayout.EndVertical();

                gameEvent = gameEvent.Next;
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                EditorUtility.SetDirty(eventGroup);
            }
        }
    }
}
