using System.IO;

namespace RPGTool.Save
{
    internal interface IDataSaver
    {
        void Save(object data, BinaryWriter stream);
    }
}