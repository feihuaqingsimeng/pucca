using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UIEditorUtility
{

    [MenuItem("UI Utility/Create Text #&X")]
    public static void CreateText()
    {
        Transform parent = Selection.activeGameObject ? Selection.activeGameObject.transform : null;
        GameObject go = new GameObject();
        go.name = "Text";
        Text text = go.AddComponent<Text>();
        text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/NotoSansKR-Black.otf");
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.color = Color.white;
        text.text = "New Text";
        text.rectTransform.sizeDelta = new Vector2(100f, 30f);
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.black;
        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        go.transform.SetParent(parent ? parent : canvas ? canvas.transform : null);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        Selection.activeGameObject = go;
    }

    [MenuItem("UI Utility/Create Image #&C")]
    public static void CreateImage()
    {
        Canvas canvas = null;
        string[] assetGUIDs = Selection.assetGUIDs;
        foreach (var assetGUID in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            if (!string.IsNullOrEmpty(assetPath) /*&& assetPath.EndsWith(".png")*/)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite)
                {
                    GameObject go = new GameObject();
                    go.name = sprite.name;
                    Image image = go.AddComponent<Image>();
                    image.sprite = sprite;
                    image.SetNativeSize();

                    if (!canvas) canvas = GameObject.FindObjectOfType<Canvas>();
                    if (canvas)
                    {
                        go.transform.SetParent(canvas.transform);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localScale = Vector3.one;
                    }

                    Selection.activeGameObject = go;
                }
            }
        }
    }
}
