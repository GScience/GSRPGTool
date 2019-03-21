using UnityEngine;
using UnityEditor;
using RPGTool.Save;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Assets.GSRPGTool.Scripts.Editor
{
    [CustomEditor(typeof(SavableBehaviour), true)]
    public class SavableBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var behavior = target as SavableBehaviour;
            if (string.IsNullOrEmpty(behavior.guid))
            {
                behavior.guid = Guid.NewGuid().ToString();
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}