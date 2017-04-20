using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    public GameObject m_Target;
    protected Stack<GameObject> m_Stack = new Stack<GameObject>();

    public int Count
    {
        get
        {
            return (m_Stack != null) ? m_Stack.Count : 0;
        }
    }

    public virtual GameObject Pop()
    {
        lock (m_Stack)
        {
            if (m_Stack != null && m_Stack.Count > 0)
            {
                return m_Stack.Pop();
            }
        }

        if (m_Target)
        {
            GameObject gameObject = GameObject.Instantiate(m_Target);
            if (gameObject)
            {
                return gameObject;
            }
        }

        return null;
    }

    public virtual T Pop<T>() where T : Component
    {
        GameObject gameObject = Pop();
        if (gameObject)
        {
            T component = gameObject.GetComponent<T>();
            if (component)
            {
                return component;
            }
        }

        return null;
    }

    public virtual void Push(GameObject gameObject)
    {
        if (gameObject != null)
        {
            lock (m_Stack)
            {
                m_Stack.Push(gameObject);
            }
        }
    }
}
