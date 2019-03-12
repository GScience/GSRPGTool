using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RPGTool.Events;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace RPGTool.Events
{
    public class GameEventBlockDisposer : IEventDisposer
    {
        private Event _currentEvent;

#if UNITY_EDITOR
        public void OnGUI(Event gameEvent)
        {
            var eventBlock = gameEvent as EventBlock;
            var startEvent = eventBlock.GetEventByIndex(0);

            while (gameEvent != null)
            {
                EditorGUILayout.BeginVertical("box");
                gameEvent.eventBlock = eventBlock;
                gameEvent.OnGUI(); 
                EditorGUILayout.EndVertical();

                gameEvent = gameEvent.Next;
            }
        }
#endif
        public void OnStart(Event gameEvent)
        {
            var eventBlock = gameEvent as EventBlock;

            _currentEvent = eventBlock.GetEventByIndex(0);
            _currentEvent.eventBlock = eventBlock;
            _currentEvent.OnStart();
        }

        public bool Update(Event gameEvent)
        {
            if (_currentEvent == null)
                return false;

            if (!_currentEvent.block || !_currentEvent.Update())
            {
                _currentEvent = _currentEvent.Next;

                if (_currentEvent == null)
                    return false;
                _currentEvent.OnStart();
            }
            else
                _currentEvent.Update();

            return true;
        }
    }
    [Serializable]
    public class EventBlock : Event
    {
        /// <summary>
        /// 开始事件
        /// </summary>
        [SerializeField]
        public List<Event> eventList = new List<Event>();

        public long startEventId;

        public EventBlock()
        {
            disposerType = typeof(GameEventBlockDisposer).Name;
        }
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

        public Event GetEventById(long id)
        {
            var index = GetEventIndexById(id);
            return GetEventByIndex(index);
        }

        public Event GetEventByIndex(int index)
        {
            return index == -1 ? null : eventList[index];
        }

        public Event AddEvent<T>() where T : IEventDisposer
        {
            var gameEvent = Create<T>(this);
            eventList.Add(gameEvent);

            return gameEvent;
        }

        public void RemoveEvent(Event gameEvent)
        {
            eventList.RemoveAt(GetEventIndexById(gameEvent.eventId));
        }
    }
}