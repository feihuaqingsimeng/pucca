using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleSiblingGroup : MonoBehaviour
{
    public List<Toggle> m_Toggles;
    List<int> m_CachedSiblingIndexes;
    int m_MaxSiblingIndex;

    void Awake()
    {
        if (m_Toggles != null && m_Toggles.Count > 0)
        {
            m_CachedSiblingIndexes = new List<int>(m_Toggles.Count);
            for (int i = 0; i < m_Toggles.Count; i++)
            {
                Toggle toggle = m_Toggles[i];
                if (toggle != null)
                {
                    m_Toggles[i].onValueChanged.AddListener(OnToggleValueChange);

                    int siblingIndex = toggle.transform.GetSiblingIndex();
                    m_CachedSiblingIndexes.Insert(i, siblingIndex);
                    if (m_MaxSiblingIndex < siblingIndex)
                    {
                        m_MaxSiblingIndex = siblingIndex;
                    }
                }
            }
        }
    }

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        OnToggleValueChange(true);
    }

    void OnToggleValueChange(bool value)
    {
        if (!value)
        {
            // To avoid successive invoke.
            return;
        }

        for (int i = 0; i < m_Toggles.Count; i++)
        {
            Toggle toggle = m_Toggles[i];
            if (toggle != null)
            {
                int siblingIndex = toggle.isOn ? m_MaxSiblingIndex : m_CachedSiblingIndexes[i];
                toggle.transform.SetSiblingIndex(siblingIndex);
            }
        }
    }
}
