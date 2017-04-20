using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIEventTimeObject : MonoBehaviour 
{
    [HideInInspector]
    public  eEventTimeType  m_eEventType;

    private UITooltipObject m_tooltip;
    private Image           m_Icon;

    private TEXT_UI         m_DecTextUI;

    public TimeSpan m_RemainTime
    {
        set
        {
            if (m_tooltip == null)
                return;

            string strRemainTime = string.Format("{0:00}:{1:00}:{2:00}", value.Hours, value.Minutes, value.Seconds);
            m_tooltip.content = Languages.ToString(m_DecTextUI, strRemainTime);
        }
    }

    //** 정보 세팅
    public void SetInit(eEventTimeType eventType, Sprite iconSprite, TEXT_UI decTextUI)
    {
        m_tooltip   = GetComponent<UITooltipObject>();
        m_Icon      = GetComponent<Image>();

        m_eEventType = eventType;

        if (m_Icon != null)
            m_Icon.sprite = iconSprite;

        m_DecTextUI = decTextUI;
    }

    //** 시간 업데이트
    public void TimeUpdate(TimeSpan remainTime)
    {
        m_RemainTime = remainTime;
    }
}
