using System;
using System.IO;
using UnityEngine;

namespace RPGTool.Save
{
    public abstract class SavableBehaviour : MonoBehaviour
    {
        [SerializeField]
        public string guid;
        public virtual void OnSave(BinaryWriter stream) { }
        public virtual void OnLoad(BinaryReader stream) { }
    }
}