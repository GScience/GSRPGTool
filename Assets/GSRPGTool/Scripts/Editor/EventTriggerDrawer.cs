using RPGTool.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RPGTool.Editor
{
    [CustomEditor(typeof(EventTrigger))]
    public class EventTriggerDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var gameEventTrigger = target as EventTrigger;

            if (GUILayout.Button("Open Event Editor"))
            {
                var window = EditorWindow.GetWindow<WindowEventTriggerEditor>();
                window.eventBlock = gameEventTrigger.gameEventBlock;
                window.Show();
            }
        }
    }
}