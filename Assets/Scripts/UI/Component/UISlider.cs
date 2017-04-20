using UnityEngine;
using UnityEngine.UI;

public class UISlider : Slider
{
    public Color m_MaximumColor;
    public Text m_Text;

    public override float value
    {
        get
        {
            return base.value;
        }

        set
        {
            base.value = (value > 0f) ? Mathf.Clamp(value, maxValue * .1f, maxValue) : value;

            if (m_Text != null)
            {
                m_Text.text = string.Format("<color={0}>{1:F0}</color>/{2:F0}", ColorUtility.ToHtmlStringRGBA(normalizedValue < 1f ? m_Text.color : m_MaximumColor), Languages.ToString(value), Languages.ToString(maxValue));
            }
        }
    }
}
