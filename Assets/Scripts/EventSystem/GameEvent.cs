using System;
using UnityEngine;

public class GameEvent
{
    private Type m_DataType;
    private object m_Data;

    public GameEventType EventType { get; private set; }
    public float InvokeTime { get; private set; }

    public dynamic Data
    {
        get
        {
            if (m_DataType == null)
                return null;

            return Convert.ChangeType(m_Data, m_DataType);
        }
    }

    private GameEvent(GameEventType _type, float _delay)
    {
        EventType = _type;
        InvokeTime = Time.time + _delay;
    }

    private GameEvent(GameEventType _type, Type _dataType, object _data, float _delay)
    {
        m_DataType = _dataType;
        EventType = _type;
        m_Data = _data;
        InvokeTime = Time.time + _delay;
    }

    public static GameEvent Create(GameEventType _type, float _delay = 0.0f) 
        => new(_type, _delay);

    public static GameEvent CreateWith<T>(GameEventType _type, T _data, float _delay = 0.0f) 
        => new(_type, typeof(T), _data, _delay);
}
