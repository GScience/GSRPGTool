using System.Collections.Generic;
using JetBrains.Annotations;
using RPGTool.Events;
using UnityEngine;

namespace RPGTool.Events
{
    public class GameEventGroup : MonoBehaviour
    {
        /// <summary>
        /// 开始事件
        /// </summary>
        [SerializeField]
        public List<GameEvent> eventList = new List<GameEvent>();

        public long startEventId;

        public List<Vector3> lalalalaa;

        public int GetEventIndexById(long id)
        {
            var low = 0;
            var high = eventList.Count;

            while (low <= high)
            {
                var middle = (low + high) / 2;
                if (id == eventList[middle].eventId)
                    return middle;
                if (id > eventList[middle].eventId)
                    low = middle + 1;
                else if (id < eventList[middle].eventId)
                    high = middle - 1;
            }

            return -1;
        }

        public GameEvent GetEventById(long id)
        {
            var index = GetEventIndexById(id);
            return GetEventByIndex(index);
        }

        public GameEvent GetEventByIndex(int index)
        {
            return index == -1 ? null : eventList[index];
        }

        public GameEvent AddEvent<T>() where T : IEventDisposer
        {
            var gameEvent = GameEvent.Create<T>(this);
            eventList.Add(gameEvent);

            return gameEvent;
        }

        public void Erase(GameEvent gameEvent)
        {
            eventList.RemoveAt(GetEventIndexById(gameEvent.eventId));
        }
    }
}