using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Alignment
    {
        Auto,
        UpperCenter,
    }

    #region Variables
    [SerializeField]
    Alignment m_Alignment;
    [SerializeField]
    string m_Content;
    static readonly float m_Delay = .5f;
    static UITooltip m_Tooltip;
    RectTransform m_RectTransform;
    #endregion

    #region Properties
    public Alignment alignment
    {
        get
        {
            return m_Alignment;
        }
    }

    public string content
    {
        get
        {
            return m_Content;
        }

        set
        {
            if (m_Content != value)
            {
                m_Content = value;
            }
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (!m_RectTransform)
            {
                m_RectTransform = (RectTransform)transform;
            }

            return m_RectTransform;
        }
    }

    static UITooltip tooltip
    {
        get
        {
            if (m_Tooltip == null)
            {
                m_Tooltip = Kernel.uiManager.Get<UITooltip>(UI.Tooltip, true, false);
            }

            return m_Tooltip;
        }
    }
    #endregion

    // Use this for initialization

    // Update is called once per frame

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(Delay());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        Kernel.uiManager.Close(UI.Tooltip);
    }

    IEnumerator Delay()
    {
        float deltaTime = 0f;
        while (deltaTime < m_Delay)
        {
            deltaTime = deltaTime + Time.deltaTime;

            yield return 0;
        }

        Kernel.uiManager.Open(UI.Tooltip);
        tooltip.tooltipObject = this;

        yield break;
    }
}
