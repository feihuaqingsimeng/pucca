using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : Singleton<ColorManager>
{

    [Serializable]
    public struct IndexedColor
    {
        [SerializeField]
        string m_Name;

        public string name
        {
            get
            {
                return m_Name;
            }
        }

        [SerializeField]
        [ColorHtmlProperty]
        Color m_Color;

        public Color color
        {
            get
            {
                return m_Color;
            }
        }
    }

    public List<IndexedColor> m_IndexedColors;
    Dictionary<string, IndexedColor> m_IndexedColorDictionary = new Dictionary<string, IndexedColor>(StringComparer.OrdinalIgnoreCase);

    protected override void Awake()
    {
        base.Awake();

        m_IndexedColorDictionary.Clear();
        if (m_IndexedColors != null &&
            m_IndexedColors.Count > 0)
        {
            for (int i = 0; i < m_IndexedColors.Count; i++)
            {
                IndexedColor indexedColor = m_IndexedColors[i];
                if (!m_IndexedColorDictionary.ContainsKey(indexedColor.name))
                {
                    m_IndexedColorDictionary.Add(indexedColor.name, indexedColor);
                }
                else Debug.LogError(string.Format("Color name is duplicated. (name : {0})", indexedColor.name));
            }
        }
    }

    // Use this for initialization

    // Update is called once per frame

    public bool TryGetHtmlStringRGBA(string name, out string htmlStringRGBA)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Color color = Color.clear;
            if (TryGetColor(name, out color))
            {
                htmlStringRGBA = string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(color));

                return true;
            }
        }

        htmlStringRGBA = string.Empty;

        return false;
    }

    public bool TryGetColor(string name, out Color color)
    {
        if (!string.IsNullOrEmpty(name))
        {
            IndexedColor indexedColor;
            if (m_IndexedColorDictionary.TryGetValue(name, out indexedColor))
            {
                color = indexedColor.color;

                return true;
            }
        }

        color = Color.clear;

        return false;
    }

    public Color GetColor(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            IndexedColor indexedColor;
            if (m_IndexedColorDictionary.TryGetValue(name, out indexedColor))
            {
                return indexedColor.color;
            }
        }

        return Color.clear;
    }
}
