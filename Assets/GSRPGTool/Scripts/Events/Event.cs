using System;
using System.Collections.Generic;
using RPGTool.Events;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGTool.Events
{
    [Serializable]
    public class Event
    {
        /// <summary>
        /// 是否阻塞事件系统
        /// </summary>
        public bool block;

        /// <summary>
        /// 事件Id
        /// </summary>
        public long eventId;

        public string disposerType;

        /// <summary>
        /// 事件处理器
        /// </summary>
        public IEventDisposer EventDisposer
        {
            get
            {
                if (_eventDisposer != null)
                    return _eventDisposer;
                _eventDisposer = (IEventDisposer)GetType().Assembly.CreateInstance(disposerType);
                JsonUtility.FromJsonOverwrite(eventValue, _eventDisposer);

                return _eventDisposer;
            }
        }

        [NonSerialized]
        private IEventDisposer _eventDisposer;

        public string eventValue;

        public void OnStart()
        {
            EventDisposer.OnStart(this);
        }

        public bool Update()
        {
            return EventDisposer.Update(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 绘制事件编辑UI
        /// </summary>
        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EventDisposer.OnGUI(this);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("事件ID：" + eventId);

            if (GUILayout.Button("+", GUILayout.MaxWidth(25)))
            {
                var newEvent = eventBlock.AddEvent<MoveActor>(); 

                if (Next != null)
                    newEvent.Next = Next;
                Next = newEvent;
            }

            //禁止删除第一个事件
            if (Previous != null)
            {
                if (GUILayout.Button("-", GUILayout.MaxWidth(25)))
                {
                    eventBlock.RemoveEvent(this);
                    if (Next != null)
                        Next.Previous = Previous;
                }
            }
            EditorGUILayout.EndHorizontal();

            //序列化
            eventValue = JsonUtility.ToJson(EventDisposer);
        }
#endif
        public static Event Create<T>(EventBlock eventBlock) where T : IEventDisposer
        {
            var newEvent = new Event { eventId = DateTime.Now.Ticks, eventBlock = eventBlock, disposerType = typeof(T).FullName };
            return newEvent;
        }

        /// <summary>
        /// 下一个事件
        /// </summary>
        [SerializeField] private long nextEventId;
        [SerializeField] private long previousEventId;

        public Event Next
        {
            get => nextEventId != 0 ? eventBlock.GetEventById(nextEventId) : null;
            set
            {
                if (nextEventId == eventId)
                    throw new ArgumentException("Next event can't set to self");
                nextEventId = value.eventId;
                value.previousEventId = eventId;
            }
        }

        public Event Previous
        {
            get => previousEventId != 0 ? eventBlock.GetEventById(previousEventId) : null;
            set
            {
                if (previousEventId == eventId)
                    throw new ArgumentException("Next event can't set to self");
                previousEventId = value.eventId;
                value.nextEventId = eventId;
            }
        }

        /// <summary>
        /// 事件所属的事件组
        /// </summary>
        [NonSerialized]
        public EventBlock eventBlock;
    }
}