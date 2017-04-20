using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShortcut : UIObject
{
    Vector2 m_ScreenPoint;
    bool m_Clicked;
    RectTransform m_ButtonRectTransform;

    protected override void Awake()
    {
        if (Kernel.uiManager != null)
        {
            UIHUD hud = Kernel.uiManager.Get<UIHUD>(UI.HUD, false);
            if (hud != null)
            {
                m_ButtonRectTransform = hud.m_ShortcutToggle.image.rectTransform;
            }
        }
    }

    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                m_Clicked = true;
                m_ScreenPoint = t.position;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            m_Clicked = true;
            m_ScreenPoint = Input.mousePosition;
        }

        if (m_Clicked)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(m_ScreenPoint);
            Vector2 inverseTransformPoint = rectTransform.InverseTransformPoint(worldPoint);
            if (!rectTransform.rect.Contains(inverseTransformPoint))
            {
                inverseTransformPoint = m_ButtonRectTransform.InverseTransformPoint(worldPoint);
                if (!m_ButtonRectTransform.rect.Contains(inverseTransformPoint))
                {
                    OnCloseButtonClick();
                }
            }

            m_Clicked = false;
        }
    }
}
