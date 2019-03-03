using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.Save
{
    public interface ISavable
    {
        void OnSave(BinaryWriter stream);

        void OnLoad(BinaryReader stream);
    }
}
