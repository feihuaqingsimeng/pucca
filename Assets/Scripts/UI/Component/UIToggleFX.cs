using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggleFX : MonoBehaviour
{
    public Toggle m_Toggle;
    public Text m_Text;
    public List<Outline> m_OutlineList;
    public List<Shadow> m_ShadowList;
    public Color m_OnTextColor = Color.white;
    public Color m_OffTextColor = Color.white;
    public Color m_OnOutlineColor = Color.white;
    public Color m_OffOutlineColor = Color.white;
    public Color m_OnShadowColor = Color.white;
    public Color m_OffShadowColor = Color.white;
    public bool m_TargetGraphicEnabledWithValue;

    void Reset()
    {
        if (m_Toggle == null)
        {
            m_Toggle = GetComponent<Toggle>();
        }

        if (m_Text == null)
        {
            m_Text = GetComponentInChildren<Text>(true);
        }

        if (m_Text != null)
        {
            m_OutlineList = new List<Outline>();
            m_ShadowList = new List<Shadow>();

            BaseMeshEffect[] components = m_Text.GetComponents<BaseMeshEffect>();
            for (int i = 0; i < components.Length; i++)
            {
                BaseMeshEffect component = components[i];
                if (component is Outline)
                {
                    m_OutlineList.Add(component as Outline);
                }
                else if (component is Shadow)
                {
                    m_ShadowList.Add(component as Shadow);
                }
            }
        }
    }

    void Awake()
    {
        m_Toggle.onValueChanged.AddListener(OnToggleValueChange);
    }

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        OnToggleValueChange(m_Toggle.isOn);
    }

    void OnToggleValueChange(bool value)
    {
        if (m_Text != null)
        {
            m_Text.color = value ? m_OnTextColor : m_OffTextColor;

            for (int i = 0; i < m_OutlineList.Count; i++)
            {
                m_OutlineList[i].effectColor = value ? m_OnOutlineColor : m_OffOutlineColor;
            }

            for (int i = 0; i < m_ShadowList.Count; i++)
            {
                m_ShadowList[i].effectColor = value ? m_OnShadowColor : m_OffShadowColor;
            }
        }

        if (m_TargetGraphicEnabledWithValue)
        {
            m_Toggle.targetGraphic.enabled = !value;
        }
    }
}
