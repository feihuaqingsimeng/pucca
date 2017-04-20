using UnityEngine;
using UnityEngine.UI;

public class UITooltip : UIObject
{
    // anchorMax : .5f, .5f
    // anchorMin : .5f, .5f
    // pivot : .5f, .5f

    [SerializeField]
    Text m_Text;
    [SerializeField]
    RectOffset m_Padding;
    [SerializeField]
    Vector2 m_Spacing;
    UITooltipObject m_TooltipObject;
    Vector2 m_MaxSize;

    protected override void Awake()
    {
        base.Awake();

        m_MaxSize = m_Text.rectTransform.rect.size;
    }

    // Use this for initialization

    // Update is called once per frame

    public UITooltipObject tooltipObject
    {
        get
        {
            return m_TooltipObject;
        }

        set
        {
            if (m_TooltipObject != value)
            {
                m_TooltipObject = value;
            }

            if (m_TooltipObject)
            {
                m_Text.text = m_TooltipObject.content;
                BuildLayout();

                switch (m_TooltipObject.alignment)
                {
                    case UITooltipObject.Alignment.Auto:
                        Auto();
                        break;
                    case UITooltipObject.Alignment.UpperCenter:
                        UpperCenter();
                        break;
                }
            }
        }
    }

    void BuildLayout()
    {
        m_Text.rectTransform.sizeDelta = m_MaxSize;
        m_Text.horizontalOverflow = (m_Text.preferredWidth > m_MaxSize.x) ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;
        UIUtility.FitSizeToContent(m_Text);
        Vector2 sizeDelta = m_Text.rectTransform.sizeDelta;
        sizeDelta.x = sizeDelta.x + m_Padding.left + m_Padding.right;
        sizeDelta.y = sizeDelta.y + m_Padding.top + m_Padding.bottom;
        m_Text.rectTransform.anchoredPosition = new Vector2(m_Padding.left, -m_Padding.top);
        rectTransform.sizeDelta = sizeDelta;
    }

    void Auto()
    {
        // TooltipObject.RectTransform의 중앙 월드 좌표
        Vector2 worldPosition = m_TooltipObject.rectTransform.TransformPoint(m_TooltipObject.rectTransform.rect.center);
        float x;
        // TooltipObject가 화면 중앙에 위치한 경우, 툴팁을 오른쪽에 띄우기 위해 <= 연산자를 사용합니다.
        bool left = (worldPosition.x <= 0f);
        if (left)
        {
            // xMax == TooltipObject.RectTransform.rect.right
            // rectTransform.rect.width * .5f : rectTransform.pivot is .5f.
            x = m_TooltipObject.rectTransform.rect.xMax + (rectTransform.rect.width * .5f) + m_Spacing.x;
        }
        else
        {
            // xMin == TooltipObject.RectTransform.rect.left
            // rectTransform.rect.width * .5f : rectTransform.pivot is .5f.
            x = m_TooltipObject.rectTransform.rect.xMin - (rectTransform.rect.width * .5f) - m_Spacing.x;
        }

        // TooltipObject.RectTransform.pivot의 값에 따라
        // TransformPoint() 리턴 값이 변해
        float y = (m_TooltipObject.rectTransform.rect.height * (1f - m_TooltipObject.rectTransform.pivot.y));
        // rectTransform.rect.width * .5f : rectTransform.pivot is .5f.
        y = y - (rectTransform.rect.height * .5f);
        worldPosition = m_TooltipObject.transform.TransformPoint(x, y, 0f);

        // 이동
        rectTransform.position = worldPosition;

        // 위 또는 아래로 화면을 벗어난 경우, 높이 값을 보정합니다.
        Vector2 centerViewportPoint = Camera.main.WorldToViewportPoint(worldPosition);
        Vector2 yMaxWorldPosition = rectTransform.TransformPoint(rectTransform.rect.center.x, rectTransform.rect.yMax, 0f);
        Vector2 yMaxViewportPoint = Camera.main.WorldToViewportPoint(yMaxWorldPosition);
        if (yMaxViewportPoint.y > 1f)
        {
            centerViewportPoint.y = 1f - (yMaxViewportPoint.y - centerViewportPoint.y);
            worldPosition = Camera.main.ViewportToWorldPoint(centerViewportPoint);
            rectTransform.position = worldPosition;
        }
        else
        {
            Vector2 yMinWorldPosition = rectTransform.TransformPoint(rectTransform.rect.center.x, rectTransform.rect.yMin, 0f);
            Vector2 yMinViewportPoint = Camera.main.WorldToViewportPoint(yMinWorldPosition);
            if (yMinViewportPoint.y < 0f)
            {
                centerViewportPoint.y = centerViewportPoint.y - yMinViewportPoint.y;
                worldPosition = Camera.main.ViewportToWorldPoint(centerViewportPoint);
                rectTransform.position = worldPosition;
            }
        }

    }

    void UpperCenter()
    {
        float x = m_TooltipObject.rectTransform.rect.center.x;
        float y = m_TooltipObject.rectTransform.rect.yMax + (rectTransform.rect.height * .5f) + m_Spacing.y;
        Vector2 worldPosition = m_TooltipObject.rectTransform.TransformPoint(x, y, 0f);

        rectTransform.position = worldPosition;
    }
}
