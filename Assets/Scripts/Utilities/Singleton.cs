using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_Instance;

    public static T Instance
    {
        get
        {
            /*
            if (m_Destroyed)
            {
                Debug.LogWarning(string.Format("Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T)));
                return null;
            }
            */

            // Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?)
            /*
            if (Application.isLoadingLevel)
            {
                return null;
            }
            */
            /*
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError("Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                    return m_Instance;
                }

                if (m_Instance == null)
                {
                    CreateInstance();
                }
            }
            */
            return m_Instance;
        }
    }

    //static bool m_Destroyed = false;

    protected virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        //m_Destroyed = true;
    }

    public static T CreateInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = (new GameObject(typeof(T).ToString())).AddComponent<T>();
        }

        return m_Instance;
    }
}
