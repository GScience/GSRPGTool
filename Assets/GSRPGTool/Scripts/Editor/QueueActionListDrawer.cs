using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using RPGTool.Actions;

namespace RPGTool.Editor
{
    [CustomEditor(typeof(QueueActionList))]
    public class QueueActionListDrawer : UnityEditor.Editor
    {
        ReorderableList reorderableList;

        void OnEnable()
        {
            var prop = serializedObject.FindProperty("queueActionList");

            reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true)
            {
                elementHeight = 80,
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = prop.GetArrayElementAtIndex(index);
                    rect.height -= 4;
                    rect.y += 2;
                    EditorGUI.PropertyField(rect, element);
                },
                drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
                {
                    GUI.backgroundColor = Color.yellow;
                },
                drawHeaderCallback = (rect) =>
                    EditorGUI.LabelField(rect, prop.displayName)
            };

            //设置单个元素的高度

            //绘制单个元素

            //背景色

            //头部

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //reorderableList.DoLayoutList(); 
            serializedObject.ApplyModifiedProperties();
        }
    }
}