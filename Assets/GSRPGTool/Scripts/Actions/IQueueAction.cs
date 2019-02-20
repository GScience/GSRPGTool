using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGTool.Actions
{
    public interface IQueueAction
    {
        void OnStart();
        void OnFinished();
        bool IsFinished();
    }
}
