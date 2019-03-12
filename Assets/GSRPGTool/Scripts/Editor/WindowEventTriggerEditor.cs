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
    public class WindowEventTriggerEditor : EditorWindow
    {
        public EventBlock eventBlock;

        private Vector2 scrollPos = Vector2.zero;

        void OnGUI()
        {
            if (eventBlock == null)
                return;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginVertical("box");

            if (eventBlock.eventList == null || eventBlock.eventList.Count == 0)
                eventBlock.AddEvent<StartEvent>();

            var gameEvent = eventBlock.GetEventByIndex(0);

            while (gameEvent != null)
            {
                EditorGUILayout.BeginVertical("box");
                gameEvent.OnGUI();
                EditorGUILayout.EndVertical();

                gameEvent = gameEvent.Next;
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            EditorGUILayout.EndScrollView();
        }
    }
}
