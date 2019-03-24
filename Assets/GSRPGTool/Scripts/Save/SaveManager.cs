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
        private static SaveManager _saveManager;
        public int CurrentSceneId;
        public static string SaveName { get; private set; }

        public static string SaveDir => Application.persistentDataPath + "/" + SaveName;
        public static string SavePath => SaveDir + "/scene" + SceneManager.GetActiveScene().buildIndex + ".sav";

        public static Dictionary<string, int> database = new Dictionary<string, int>();

        public static SaveManager saveManager
        {
            get
            {
                if (_saveManager == null)
                    _saveManager = FindObjectOfType<SaveManager>();
                return _saveManager;
            }
        }

        private void Awake()
        {
            Load("Save0");

            LoadCurrentScene();

            //创建目录
            if (!Directory.Exists(Application.persistentDataPath + "/" + SaveName))
                Directory.CreateDirectory(Application.persistentDataPath + "/" + SaveName);

            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += (arg0, arg1) =>
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
            saveFile.Close();
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
            saveFile.Close();
        }

        public void Save()
        {
            SaveCurrentScene();
            var file = File.OpenWrite(SaveDir + "/save.sav");
            var writer = new BinaryWriter(file);
            DataSaver.Save(CurrentSceneId, writer);

            //保存数据库
            foreach (var data in database)
            {
                DataSaver.Save(true, writer);
                DataSaver.Save(data.Key, writer);
                DataSaver.Save(data.Value, writer);
            }
            DataSaver.Save(false, writer);

            writer.Close();
            file.Close();
        }

        public void Load(string saveName)
        {
            SaveName = saveName;

            if (File.Exists(SaveDir + "/save.sav"))
            {
                var file = File.OpenRead(SaveDir + "/save.sav");
                var reader = new BinaryReader(file);
                CurrentSceneId = DataLoader.Load<int>(reader);

                //读取数据库
                database = new Dictionary<string, int>();
                while (DataLoader.Load<bool>(reader))
                {
                    var key = DataLoader.Load<string>(reader);
                    var value = DataLoader.Load<int>(reader);
                    database.Add(key, value);
                }

                reader.Close();
                file.Close();
            }
        }

        public void OnApplicationQuit()
        {
            Save();
        }
    }
}