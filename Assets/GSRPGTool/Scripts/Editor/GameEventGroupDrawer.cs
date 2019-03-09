using RPGTool.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RPGTool.Editor
{
    [CustomEditor(typeof(GameEventGroup))]
    public class QueueActionListDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var eventGroup = target as GameEventGroup;

            if (GUILayout.Button("Open Event Editor"))
            {
                var window = EditorWindow.GetWindow<WindowEventEditor>();
                window.eventGroup = eventGroup; 
                window.Show();
            }
        }
    }
}