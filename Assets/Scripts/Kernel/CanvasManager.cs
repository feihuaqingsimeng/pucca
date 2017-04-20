using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    public int canvasCount
    {
        get;
        private set;
    }

    Dictionary<int, Canvas> m_Canvases = new Dictionary<int, Canvas>();
    Dictionary<int, CanvasGroup> m_CanvasGroups = new Dictionary<int, CanvasGroup>();

    protected override void Awake()
    {
        base.Awake();

        Canvas[] children = GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < children.Length; i++)
        {
            Canvas child = children[i];
            int sortingOrder = child.sortingOrder; // -32,768 ~ 32,767 (Range of short type.)

            if (!m_Canvases.ContainsKey(sortingOrder))
                m_Canvases.Add(sortingOrder, child);
            else
                Debug.LogError(sortingOrder);

            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();

            if(canvasGroup == null)
            {
                Debug.LogError(string.Format("cavasGroup is Null : {0}", sortingOrder));
                continue;
            }

            if (!m_CanvasGroups.ContainsKey(sortingOrder))
                m_CanvasGroups.Add(sortingOrder, canvasGroup);
            else
                Debug.LogError(string.Format("Aready Add CanvasGroup Number : {0}",sortingOrder));
        }

        canvasCount = m_Canvases.Count;
    }

    //** Sorting Canvas Block
    public Canvas GetCanvas(int sortingOrder)
    {
        return m_Canvases.ContainsKey(sortingOrder) ? m_Canvases[sortingOrder] : null;
    }

    public bool SetCanvasGroupBlock(int sortingOrder, bool block)
    {
        if (m_CanvasGroups == null)
            return false;

        if (!m_CanvasGroups.ContainsKey(sortingOrder))
            Debug.LogError(string.Format("cavasGroup is Null : {0}", sortingOrder));
        else
            CanvasBlock(m_CanvasGroups[sortingOrder], block);

        return true;
    }

    //** All Canvas Block
    public bool GetAllCanvasBlock(bool block)
    {
        if (m_CanvasGroups == null)
            return false;

        foreach (var canvasGroup in m_CanvasGroups.Values)
        {
            if (canvasGroup != null)
                CanvasBlock(canvasGroup, block);
        }

        return true;
    }

    private void CanvasBlock(CanvasGroup group, bool block)
    {
        group.interactable = !block;
        group.blocksRaycasts = !block;
    }
}
