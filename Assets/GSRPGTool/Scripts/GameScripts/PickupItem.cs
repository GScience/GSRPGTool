using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGTool.GameScripts;
using RPGTool.GameScripts.Triggers;
using RPGTool.Save;

namespace RPGTool.GameScripts
{
    public class PickupItem : GameScriptBase
    {
        public static string messageFormat = "获得物品：{0}";
        public static string keyFormat = "PickupItem.OwnItem.{0}";
        public string itemName = "";

        public override void Do(TriggerBase trigger)
        {
            AddPickupItemScripts(this, itemName);
        }

        public static int AddPickupItemScripts(GameScriptBase script, string itemName)
        {
            var pos = script.AddMessage(string.Format(messageFormat, itemName));
            script.SetDatabaseValue(string.Format(keyFormat, itemName), 1);
            return pos;
        }

        public static int AddItemCheckScripts(GameScriptBase script, string itemName, Action<int> action)
        {
            return script.Check(() =>
            {
                var key = string.Format(keyFormat, itemName);
                return !SaveManager.database.TryGetValue(key, out var value) ? 0 : value;
            }, action);
        }
    }
}
