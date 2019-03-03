using RPGTool.Actions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RPGTool.Editor
{
    [CustomEditor(typeof(EventManager))]
    public class QueueActionListDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var eventManager = target as EventManager;

            EditorGUILayout.BeginVertical("box");

            
            EditorGUILayout.EndVertical();
        }
    }
}