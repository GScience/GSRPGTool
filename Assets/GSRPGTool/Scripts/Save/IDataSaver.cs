using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.Save
{
    interface IDataSaver
    {
        void Save(object data, BinaryWriter stream);
    }
}
