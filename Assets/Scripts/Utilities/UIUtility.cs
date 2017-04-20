using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    #region Grayscale
    static Shader m_TransparentShader;

    static Shader transparentShader
    {
        get
        {
            if (m_TransparentShader == null)
            {
                m_TransparentShader = Shader.Find("UI/Unlit/Transparent");
            }

            return m_TransparentShader;
        }
    }

    static Material m_TransparentMaterial;

    public static Material transparentMaterial
    {
        get
        {
            if (m_TransparentMaterial == null)
            {
                m_TransparentMaterial = new Material(transparentShader);
                m_TransparentMaterial.color = Color.gray;
            }

            return m_TransparentMaterial;
        }
    }

    static Shader m_GrayscaleShader;

    static Shader grayscaleSahder
    {
        get
        {
            if (m_GrayscaleShader == null)
            {
                m_GrayscaleShader = Shader.Find("UI/Unlit/Grayscale");
            }

            return m_GrayscaleShader;
        }
    }

    static Material m_GrayscaleMaterial;

    public static Material grayscaleMaterial
    {
        get
        {
            if (m_GrayscaleMaterial == null)
            {
                m_GrayscaleMaterial = new Material(grayscaleSahder);
            }

            return m_GrayscaleMaterial;
        }
    }
    #endregion

    public static void SetParent(Transform child, Transform parent, int siblingIndex = -1)
    {
        if (!child)
        {
            return;
        }

        if (child.parent != parent)
        {
            Vector3 localPosition = child.localPosition;
            Vector3 localScale = child.localScale;
            RectTransform rectTransform = child as RectTransform;
            Vector3 anchoredPosition = (rectTransform != null) ? rectTransform.anchoredPosition : Vector2.zero;
            Vector2 sizeDelta = (rectTransform != null) ? rectTransform.sizeDelta : Vector2.zero;

            child.SetParent(parent);
            child.localPosition = localPosition;
            child.localScale = localScale;
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = sizeDelta;
            }
        }

        switch (siblingIndex)
        {
            case -1:
                child.SetAsLastSibling();
                break;
            case 0:
                child.SetAsFirstSibling();
                break;
            default:
                child.SetSiblingIndex(siblingIndex);
                break;
        }

        //if (child.gameObject.layer != parent.gameObject.layer) child.gameObject.layer = parent.gameObject.layer;
    }

    public static void FitSizeToContent(Text text)
    {
        if (text != null)
        {
            if (Vector2.Equals(Vector2.zero, text.rectTransform.sizeDelta))
            {
                float left = text.rectTransform.rect.xMin;
                float top = text.rectTransform.rect.yMin;
                float width = text.rectTransform.rect.width;
                float height = text.rectTransform.rect.height;

                text.rectTransform.rect.Set(left, top, width, height);
            }
            else
            {
                TextGenerationSettings textGenerationSettings = text.GetGenerationSettings(text.rectTransform.sizeDelta);

                if (textGenerationSettings.horizontalOverflow != HorizontalWrapMode.Wrap)
                {
                    text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.rectTransform.sizeDelta.y);
                }

                if (textGenerationSettings.verticalOverflow != VerticalWrapMode.Truncate)
                {
                    text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight);
                }
            }
        }
    }

    public static void EllipsisSingleLine(Text item, string value)
    {
        if (!item)
        {
            return;
        }

        float charWidth;
        switch (item.fontSize)
        {
            case 16:
                charWidth = 18;
                break;
            case 20:
                charWidth = 21;
                break;
            case 24:
                charWidth = 24;
                break;
            case 30:
                charWidth = 33;
                break;
            default:
                item.cachedTextGeneratorForLayout.Populate(".", item.GetGenerationSettings(item.rectTransform.rect.size));
                charWidth = item.cachedTextGeneratorForLayout.characters[0].charWidth * 3f;
                break;
        }

        List<UICharInfo> charInfos = new List<UICharInfo>();
        item.cachedTextGeneratorForLayout.Populate(value, item.GetGenerationSettings(item.rectTransform.rect.size));
        item.cachedTextGeneratorForLayout.GetCharacters(charInfos);
        int lastIndex = 0;
        for (int i = 0; i < charInfos.Count; i++)
        {
            if (charWidth < item.rectTransform.rect.width)
            {
                charWidth = charWidth + charInfos[i].charWidth;
            }
            else break;

            lastIndex = i;
        }

        item.text = (lastIndex != charInfos.Count - 1) ?
                    string.Format("{0}{1}", value.Substring(0, lastIndex), "...") :
                    value;
    }

    public static Transform FindChildRecursively(Transform transform, string name)
    {
        return null;
    }

    public static List<Transform> GetParentList(Transform transform)
    {
        if (transform && transform.parent)
        {
            return new List<Transform>(GetParentList(transform.parent)) { transform.parent };
        }

        return null;
    }

    public static string GetHierarchy(Transform transform)
    {
        if (transform)
        {
            string hierarchy = string.Empty;
            List<Transform> transforms = GetParentList(transform);
            for (int i = 0; i < transforms.Count; i++)
            {
                hierarchy = string.Format("{0}/{1}", hierarchy, transforms[i].name);
            }

            return hierarchy;
        }

        return string.Empty;
    }

    public static bool TryParseGuildEmblem(string value, ref Color emblemColor, ref int patternIndex)
    {
        if (!string.IsNullOrEmpty(value))
        {
            string[] split = value.Split(',');
            if (split.Length == 2)
            {
                if (ColorUtility.TryParseHtmlString(string.Format("#{0}", split[0]), out emblemColor))
                {
                    if (int.TryParse(split[1], out patternIndex))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static string GuildEmblemToString(Color color, int pattern)
    {
        return string.Format("{0},{1}", ColorUtility.ToHtmlStringRGB(color), pattern);
    }

    public static void SetBaseMeshEffectColor(GameObject gameObject, bool includeInactive, Color shadowEffectColor, Color outlineEffectColor)
    {
        if (gameObject != null)
        {
            //BaseMeshEffect[] comps = gameObject.GetComponentsInChildren<BaseMeshEffect>(includeInactive);
            Shadow[] comps = gameObject.GetComponentsInChildren<Shadow>(includeInactive);
            foreach (var comp in comps)
            {
                if (comp is Shadow)
                {
                    comp.effectColor = shadowEffectColor;
                }
                else if (comp is Outline)
                {
                    comp.effectColor = outlineEffectColor;
                }
            }
        }
    }

    /*************** [ScrollRect Grid Reposition] ***************
        주의! : Children, Parent는 왼쪽 상단 축.
        수평 : 왼쪽 -> 오른쪽 정렬
        수직 : 위 -> 아래 정렬
        그룹 : Colum은 가로의 숫자. 사용할 경우 Parent가 List 전체와 사이즈가 같게 됨.
    ************************************************************/
    public static void SetReposition(RectTransform trsParent, List<RectTransform> trsChildrenList, float space, bool vertical, float frontXSpace = 0.0f, float frontYSpace = 0.0f, int colum = 0)
    {
        if (trsParent == null || trsChildrenList == null || trsChildrenList.Count == 0)
            return;

        // 기본 필요 데이터
        float parentRect_HalfHeight = 0.0f;
        float parentRect_HalfWidth = 0.0f;

        float itemRect_Height = trsChildrenList[0].rect.height;
        float itemRect_Width = trsChildrenList[0].rect.width;

        float parentXSize = 0.0f;
        float parentYSize = 0.0f;

        float preItemPosY = 0.0f;
        float preItemPosX = 0.0f;

        float xPosition = 0.0f;
        float yPosition = 0.0f;

        bool bFrontXSpace = frontXSpace != 0.0f;
        bool bFrontYSpace = frontYSpace != 0.0f;

        // Parent 사이즈 조절
        if (colum > 0)
        {
            int GroupCount = trsChildrenList.Count / colum;
            GroupCount = GroupCount <= 1 ? 1 : GroupCount;

            parentXSize = ((itemRect_Width + space) * colum) + space;
            parentYSize = ((itemRect_Height + space) * GroupCount) + space;
        }
        else
        {
            parentXSize = vertical ? trsParent.rect.width : ((itemRect_Width + space) * trsChildrenList.Count) + space;
            parentYSize = vertical ? ((itemRect_Height + space) * trsChildrenList.Count) + space : trsParent.rect.height;
        }

        trsParent.sizeDelta = new Vector2(parentXSize, parentYSize);

        parentRect_HalfWidth = parentXSize * 0.5f;
        parentRect_HalfHeight = parentYSize * 0.5f;

        // Children 재정렬
        int count = 1;
        for (int i = 0; i < trsChildrenList.Count; i++)
        {
            RectTransform ItemRect = trsChildrenList[i];

            //vertical : 세로
            if (count == 1)
            {
                //** 가로 스크롤 **//
                // X => 0 = Parent의 왼쪽 상단
                // Y => Parent 절반 사이즈 밑으로 + 아이템의 절반 사이즈 만큼 위로 = Parent의 센터
                //** 세로 스크로 **//
                // X => 0 = Parent의 왼쪽 상단
                // Y => 0 = Parent의 왼쪽 상단
                float culxPosition = 0.0f;
                float culyPosition = vertical ? 0.0f : -parentRect_HalfHeight + (itemRect_Height * 0.5f);

                xPosition = bFrontXSpace ? culxPosition + frontXSpace : culxPosition;
                yPosition = bFrontYSpace ? culyPosition - frontYSpace : culyPosition;

                //colum > 0 => y 좌표를 내려준다.
                if (colum > 0 && i > 0)
                    yPosition = (preItemPosY - itemRect_Height) - space;
            }
            else
            {
                //colum > 0 => x축으로 계속 만들어준다. colum 수 만큼...
                if (colum > 0)
                {
                    xPosition = (preItemPosX + itemRect_Width) + space;
                }
                else
                {
                    xPosition = vertical ? preItemPosX : (preItemPosX + itemRect_Width) + space;
                    yPosition = vertical ? (preItemPosY - itemRect_Height) - space : preItemPosY;
                }
            }

            ItemRect.localPosition = new Vector3(xPosition, yPosition, 0);

            preItemPosX = ItemRect.localPosition.x;
            preItemPosY = ItemRect.localPosition.y;

            count = count == colum ? 1 : count + 1;
        }
    }
}
