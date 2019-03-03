using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.Save
{
    interface IDataLoader
    {
        object Load(BinaryReader stream);
    }
}
