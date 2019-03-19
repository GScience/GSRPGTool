using System.IO;

namespace RPGTool.Save
{
    public interface ISavable
    {
        void OnSave(BinaryWriter stream);

        void OnLoad(BinaryReader stream);
    }
}