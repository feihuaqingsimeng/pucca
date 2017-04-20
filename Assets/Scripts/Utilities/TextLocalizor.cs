using UnityEngine;
using UnityEngine.UI;

public class TextLocalizor : MonoBehaviour
{
    public Text m_TextComponent;
    public UITooltipObject m_TooltipObject;
    public string m_Value;
    public bool m_FitSizeToContent;

    void Reset()
    {
        m_TextComponent = GetComponent<Text>();
        m_TooltipObject = GetComponent<UITooltipObject>();
    }

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(m_Value))
        {
            string value = Languages.StringToTEXT_UI(m_Value);

            if (m_TextComponent)
            {
                m_TextComponent.text = value;

                if (m_FitSizeToContent)
                {
                    UIUtility.FitSizeToContent(m_TextComponent);
                }
            }

            if (m_TooltipObject)
            {
                m_TooltipObject.content = value;
            }
        }
    }
}
