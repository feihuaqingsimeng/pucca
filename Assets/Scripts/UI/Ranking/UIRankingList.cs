using System.Collections.Generic;
using UnityEngine;

public class UIRankingList : MonoBehaviour
{
    public UIScrollRect m_ScrollRect;
    public GameObjectPool m_Pool;
    public float m_Padding;
    public float m_Spacing;
    protected List<UIRankingObject> m_RankingObjectList = new List<UIRankingObject>();
    protected byte m_Page = 1;
    protected float m_CellHalfHeight;

    protected virtual void Awake()
    {
        m_ScrollRect.onActivityIndicatorActiveChanged += OnActivityIndicatorActive;
        m_CellHalfHeight = ((RectTransform)(m_Pool.m_Target.transform)).rect.size.y * .5f;
    }

    // Use this for initialization

    // Update is called once per frame

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnActivityIndicatorActive(bool activityIndicatorActive)
    {

    }

    protected virtual void BuildLayout()
    {
        float y = -(m_Padding + m_CellHalfHeight);
        if (m_RankingObjectList != null && m_RankingObjectList.Count > 0)
        {
            for (int i = 0; i < m_RankingObjectList.Count; i++)
            {
                UIRankingObject item = m_RankingObjectList[i];
                if (item != null)
                {
                    item.rectTransform.anchoredPosition = new Vector2(0f, y);
                    y = y - item.rectTransform.sizeDelta.y - m_Spacing;
                }
            }
        }

        y = y + m_Spacing - m_Padding + m_CellHalfHeight;
        y = Mathf.Abs(y);
        m_ScrollRect.content.sizeDelta = new Vector2(m_ScrollRect.content.sizeDelta.x, y);
    }
}
