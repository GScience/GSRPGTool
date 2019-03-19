using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGTool.Save
{
    public class SaveManager : MonoBehaviour
    {
        void Awake()
        {
            Load();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
                Save();
            else if (Input.GetKeyDown(KeyCode.L))
                Load();
        }

        public void Save()
        {
            var saveFile = File.OpenWrite(Application.persistentDataPath + "/Save.sav");
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

        public void Load()
        {
            if (!File.Exists(Application.persistentDataPath + "/Save.sav"))
                return;

            var saveFile = File.OpenRead(Application.persistentDataPath + "/Save.sav");
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
