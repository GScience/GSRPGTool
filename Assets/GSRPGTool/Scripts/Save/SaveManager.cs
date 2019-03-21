using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace RPGTool.Save
{
    public class SaveManager : MonoBehaviour
    {
        public static string SaveName { get; private set; }

        public static string SavePath => Application.persistentDataPath + "/" + SaveName + "/scene" +
                                         SceneManager.GetActiveScene().GetHashCode() + ".sav";
        private void Awake()
        {
            if (string.IsNullOrEmpty(SaveName))
                SaveName = "Save0";

            Debug.Log("Save file locate in " + SavePath);

            LoadCurrentScene();

            //创建目录
            if (!Directory.Exists(Application.persistentDataPath + "/" + SaveName))
                Directory.CreateDirectory(Application.persistentDataPath + "/" + SaveName);

            DontDestroyOnLoad(this);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
                SaveCurrentScene();
        }

        public void SaveCurrentScene()
        {
            var saveFile = File.OpenWrite(SavePath);
            var writer = new BinaryWriter(saveFile);

            foreach (var component in Resources.FindObjectsOfTypeAll<Component>())
            {
                if (!(component is ISavable savable))
                    continue;
#if UNITY_EDITOR
                if (EditorUtility.IsPersistent(component.transform.root.gameObject))
                    continue;
#endif
                var id = component.GetInstanceID();

                writer.Write(true);
                writer.Write(id);
                savable.OnSave(writer);
            }

            writer.Write(false);
            writer.Close();
        }

        public void LoadCurrentScene()
        {
            if (!File.Exists(SavePath))
                return;

            var saveFile = File.OpenRead(SavePath);
            var reader = new BinaryReader(saveFile);
            var objetIndex = new Dictionary<int, ISavable>();

            foreach (var component in Resources.FindObjectsOfTypeAll<Component>())
            {
                if (!(component is ISavable savable))
                    continue;
#if UNITY_EDITOR
                if (EditorUtility.IsPersistent(component.transform.root.gameObject))
                    continue;
#endif
                objetIndex[component.GetInstanceID()] = savable;
            }

            while (true)
            {
                if (!reader.ReadBoolean())
                    break;
                var id = reader.ReadInt32();

                if (objetIndex.ContainsKey(id))
                    objetIndex[id].OnLoad(reader);
            }

            reader.Close();
        }
    }
}