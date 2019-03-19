using System.IO;

namespace RPGTool.Save
{
    internal interface IDataLoader
    {
        object Load(BinaryReader stream);
    }
}