using System.Collections.Generic;
using UnityEngine;

public class UINotificationCenter : UIObject
{
    static UINotificationCenter m_Instance;

    static UINotificationCenter instance
    {
        get
        {
            if (m_Instance == null)
            {
                if (Kernel.uiManager != null)
                {
                    m_Instance = Kernel.uiManager.Open<UINotificationCenter>(UI.NotificationCenter);
                }
            }

            return m_Instance;
        }
    }

    public float m_LifeTime;
    public float m_CrossFadeDuration;
    public Vector2 m_Margin;
    public float m_LineSpacing;
    public List<UINotificationCenterObject> m_ObjectList;

    Queue<string> m_Queue = new Queue<string>();
    int m_Index;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            UINotificationCenterObject item = m_ObjectList[i];

            item.active = false;
            item.index = -1;
            item.lifeTime = m_LifeTime;
            item.crossFadeDuration = m_CrossFadeDuration;
            item.margin = m_Margin;
        }
    }

    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        if (Equals(m_Queue, null) || Equals(m_Queue.Count, 0))
        {
            return;
        }

        UINotificationCenterObject item = Find();
        if (item == null)
        {
            return;
        }

        string value = m_Queue.Dequeue();
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        item.index = m_Index++;
        item.text = value;

        m_ObjectList.Sort(delegate(UINotificationCenterObject lhs, UINotificationCenterObject rhs)
        {
            return rhs.index.CompareTo(lhs.index);
        });

        Vector2 anchoredPosition = Vector2.zero;
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            if (!m_ObjectList[i].active)
            {
                continue;
            }

            m_ObjectList[i].rectTransform.anchoredPosition = anchoredPosition;

            anchoredPosition.y = anchoredPosition.y + m_ObjectList[i].rectTransform.sizeDelta.y + m_LineSpacing;
        }
    }

    public static void Enqueue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (!instance)
        {
            return;
        }

        m_Instance.m_Queue.Enqueue(value);
    }

    UINotificationCenterObject Find()
    {
        return m_ObjectList.Find(item => Equals(item.index, -1));
    }
}
