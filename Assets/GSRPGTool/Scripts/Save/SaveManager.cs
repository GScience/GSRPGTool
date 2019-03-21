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

        public static string SaveDir => Application.persistentDataPath + "/" + SaveName;
        public static string SavePath => SaveDir + "/scene" + SceneManager.GetActiveScene().buildIndex + ".sav";

        private static SaveManager _saveManager;

        public static SaveManager saveManager
        {
            get
            {
                if (_saveManager == null)
                    _saveManager = FindObjectOfType<SaveManager>();
                return _saveManager;
            }
        }
        public int CurrentSceneId = 0;

        private void Awake()
        {
            Load("Save0");

            LoadCurrentScene();

            //创建目录
            if (!Directory.Exists(Application.persistentDataPath + "/" + SaveName))
                Directory.CreateDirectory(Application.persistentDataPath + "/" + SaveName);

            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += (Scene arg0, LoadSceneMode arg1) =>
            {
                if (arg0.buildIndex != 0)
                    CurrentSceneId = arg0.buildIndex;
                LoadCurrentScene();
            };
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
                SaveCurrentScene();
        }

        public void SaveCurrentScene()
        {
            Debug.Log("Save to " + SavePath);

            var saveFile = File.OpenWrite(SavePath);
            var writer = new BinaryWriter(saveFile);

            foreach (var component in Resources.FindObjectsOfTypeAll<Component>())
            {
                if (!(component is SavableBehaviour savable))
                    continue;
#if UNITY_EDITOR
                if (EditorUtility.IsPersistent(component.transform.root.gameObject))
                    continue;
#endif
                var id = savable.guid;

                writer.Write(true);
                writer.Write(id);
                savable.OnSave(writer);
            }

            writer.Write(false);
            writer.Close();
        }

        public void LoadCurrentScene()
        {
            Debug.Log("Load from " + SavePath);

            if (!File.Exists(SavePath))
                return;

            var saveFile = File.OpenRead(SavePath);
            var reader = new BinaryReader(saveFile);
            var objetIndex = new Dictionary<string, SavableBehaviour>();

            foreach (var component in Resources.FindObjectsOfTypeAll<Component>())
            {
                if (!(component is SavableBehaviour savable))
                    continue;
#if UNITY_EDITOR
                if (EditorUtility.IsPersistent(component.transform.root.gameObject))
                    continue;
#endif
                objetIndex[savable.guid] = savable;
            }

            while (true)
            {
                if (!reader.ReadBoolean())
                    break;
                var id = reader.ReadString();

                if (objetIndex.ContainsKey(id))
                    objetIndex[id].OnLoad(reader);
            }

            reader.Close();
        }

        public void Save()
        {
            SaveCurrentScene();
            var writer = new BinaryWriter(File.OpenWrite(SaveDir + "/save.sav"));
            DataSaver.Save(CurrentSceneId, writer);
            writer.Close();
        }

        public void Load(string saveName)
        {
            SaveName = saveName;

            if (File.Exists(SaveDir + "/save.sav"))
            {
                var reader = new BinaryReader(File.OpenRead(SaveDir + "/save.sav"));
                CurrentSceneId = DataLoader.Load<int>(reader);
                reader.Close();
            }
        }

        public void OnApplicationQuit()
        {
            Save();
        }
    }
}