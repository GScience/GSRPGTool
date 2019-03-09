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
    public class GameEvent
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

        void OnStart()
        {
            EventDisposer.OnStart();
        }

        void Update()
        {
            EventDisposer.Update();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 绘制事件编辑UI
        /// </summary>
        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EventDisposer.OnGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("事件ID：" + eventId);

            if (GUILayout.Button("+", GUILayout.MaxWidth(25)))
            {
                var newEvent = eventGroup.AddEvent<MoveActor>(); 

                if (Next != null)
                    newEvent.Next = Next;
                Next = newEvent;
            }

            GUILayout.Button("-", GUILayout.MaxWidth(25));
            EditorGUILayout.EndHorizontal();

            //序列化
            eventValue = JsonUtility.ToJson(EventDisposer);
        }
#endif
        public static GameEvent Create<T>(GameEventGroup eventGroup) where T : IEventDisposer
        {
            var newEvent = new GameEvent { eventId = DateTime.Now.Ticks, eventGroup = eventGroup, disposerType = typeof(T).FullName };
            return newEvent;
        }

        /// <summary>
        /// 下一个事件
        /// </summary>
        [SerializeField] public long nextEventId;

        public GameEvent Next
        {
            get => nextEventId != 0 ? eventGroup.GetEventById(nextEventId) : null;
            set
            {
                if (nextEventId == eventId)
                    throw new ArgumentException("Next event can't set to self");
                nextEventId = value.eventId;
            }
        }

        /// <summary>
        /// 事件所属的事件组
        /// </summary>
        [SerializeField]
        public GameEventGroup eventGroup;
    }
}