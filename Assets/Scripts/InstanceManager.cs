using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagedInstance : MonoBehaviour
{
}

public static class InstanceManager
{
    static Dictionary<Type, ManagedInstance> m_ManagedInstances = new ();

    public static T Get<T>() where T : ManagedInstance
    {
        if (!m_ManagedInstances.ContainsKey(typeof(T)))
            throw new Exception($"Can't get instance of type {typeof(T)}, it does not exist.");

        return (T)m_ManagedInstances[typeof(T)];
    }

    public static void Add<T>(T _instance) where T : ManagedInstance
    {
        if(m_ManagedInstances.ContainsKey(typeof(T)))
        {
            throw new Exception($"Instance of type {typeof(T)} already exists.");
        }

        m_ManagedInstances[typeof(T)] = _instance;
    }

    public static void Remove<T>(T _instance) where T : ManagedInstance
    {
        if (m_ManagedInstances.ContainsValue(_instance))
        {
            m_ManagedInstances.Remove(typeof(T));
            return;
        }

        throw new Exception($"Instance of type {typeof(T)} does not exist.");
    }
}
