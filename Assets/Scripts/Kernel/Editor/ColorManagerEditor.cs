using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorManager))]
public class ColorManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Sort", GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false)))
        {
            ColorManager colorManager = target as ColorManager;
            if (colorManager != null)
            {
                colorManager.m_IndexedColors.Sort((lhs, rhs) => lhs.name.CompareTo(rhs.name));
            }
        }
    }
}
