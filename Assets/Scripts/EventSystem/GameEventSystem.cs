using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class GameEventSystem : ManagedInstance
//{
//    public delegate void GameEventDelegate<T>(T e) where T : GameEvent;
//    private delegate void GameEventDelegate(GameEvent e);

//    private Dictionary<Type, GameEventDelegate> m_DelegatesMap = new();
//    private Dictionary<Delegate, GameEventDelegate> m_DelegateLookupMap = new();

//    private List<GameEvent> m_PendingEventQueueList = new();

//    private void Awake()
//    {
//        InstanceManager.Add(this);

//        if (m_DelegatesMap.Count > 0)
//            m_DelegatesMap.Clear();

//        if (m_DelegateLookupMap.Count > 0)
//            m_DelegateLookupMap.Clear();

//        if (m_PendingEventQueueList.Count > 0)
//            m_PendingEventQueueList.Clear();
//    }

//    private void Update()
//    {
//        for(int i = 0; i < m_PendingEventQueueList.Count; i++)
//        {
//            var gameEvent = m_PendingEventQueueList[i];

//            if (Time.time > gameEvent.InvokeTime)
//            {
//                GameEventDelegate tempDel; 
//                Type tempType = gameEvent.GetType();
//                if(m_DelegatesMap.TryGetValue(tempType, out tempDel)) // Can't just pass in gameEvent.GetType for some jank reason
//                {
//                    m_DelegatesMap[gameEvent.GetType()].Invoke(gameEvent);
//                    m_PendingEventQueueList.Remove(gameEvent);
//                }
//            }
//        }
//    }

//    public void Subscribe<T>(GameEventDelegate<T> _del) where T : GameEvent
//    {
//        GameEventDelegate internalDelegate = (e) => { _del((T)e); };

//        if (m_DelegateLookupMap.ContainsKey(_del) && m_DelegateLookupMap[_del] == internalDelegate)
//            return;

//        m_DelegateLookupMap[_del] = internalDelegate;

//        GameEventDelegate tempDel;
//        if (m_DelegatesMap.TryGetValue(typeof(T), out tempDel))
//        {
//            m_DelegatesMap[typeof(T)] = tempDel += internalDelegate;
//            return;
//        }

//        m_DelegatesMap[typeof(T)] = internalDelegate;
//    }

//    public void Unsubscribe<T>(GameEventDelegate<T> _del) where T : GameEvent
//    {
//        GameEventDelegate internalDelegate;
//        if (!m_DelegateLookupMap.TryGetValue(_del, out internalDelegate))
//            return;

//        m_DelegateLookupMap.Remove(_del);

//        GameEventDelegate tempDel;
//        if (!m_DelegatesMap.TryGetValue(typeof(T), out tempDel))
//            return;

//        tempDel -= internalDelegate;
//        if (tempDel == null)
//        {
//            m_DelegatesMap.Remove(typeof(T));
//            return;
//        }

//        m_DelegatesMap[typeof(T)] = tempDel;
//    }

//    public void Post(GameEvent e)
//    {
//        m_PendingEventQueueList.Add(e);

//        //Debug.Log($"[{e.GetType()}] posted at {Time.time}. Set to call at {e.InvokeTime}.");
//    }
//}

public class GameEventSystem : ManagedInstance
{
    public delegate void GameEventDelegate(GameEvent e);

    private Dictionary<GameEventType, GameEventDelegate> m_DelegatesMap = new();

    private List<GameEvent> m_PendingEventQueueList = new();

    private void Awake()
    {
        InstanceManager.Add(this);

        if (m_DelegatesMap.Count > 0)
            m_DelegatesMap.Clear();

        if (m_PendingEventQueueList.Count > 0)
            m_PendingEventQueueList.Clear();
    }

    private void Update()
    {
        for (int i = 0; i < m_PendingEventQueueList.Count; i++)
        {
            var gameEvent = m_PendingEventQueueList[i];

            if (Time.time > gameEvent.InvokeTime)
            {
                if (m_DelegatesMap.ContainsKey(gameEvent.EventType))
                {
                    m_DelegatesMap[gameEvent.EventType].Invoke(gameEvent);
                    m_PendingEventQueueList.Remove(gameEvent);
                }
            }
        }
    }

    public void Subscribe(GameEventType _eventType, GameEventDelegate _del)
    {
        if (m_DelegatesMap.ContainsKey(_eventType))
        {
            m_DelegatesMap[_eventType] += _del;
            return;
        }

        m_DelegatesMap[_eventType] = _del;
    }

    public void Unsubscribe(GameEventType _eventType, GameEventDelegate _del)
    {
        GameEventDelegate tempDel;

        if (!m_DelegatesMap.TryGetValue(_eventType, out tempDel))
            return;

        tempDel -= _del;
        if (tempDel == null)
        {
            m_DelegatesMap.Remove(_eventType);
            return;
        }

        m_DelegatesMap[_eventType] = tempDel;
    }

    public void Post(GameEvent e)
    {
        m_PendingEventQueueList.Add(e);
    }

    public void PostNow(GameEvent e)
    {
        if (m_DelegatesMap.ContainsKey(e.EventType))
        {
            m_DelegatesMap[e.EventType].Invoke(e);
        }
    }
}