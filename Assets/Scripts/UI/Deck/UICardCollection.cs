using Common.Packet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICardCollection : UIBehaviour
{
    public GameObjectPool m_GameObjectPool;
    // GridLayoutGroup 컴포넌트를 사용하지 않고, BuildLayout() 함수를 사용합니다.
    public GridLayoutGroup m_GridLayoutGroup;
    public Text m_EmptyText;
    public List<UICharCard> m_CharCardList;
    public List<Image> m_EmptyImages;
    public bool m_IsOwnCollection;
    public RectOffset m_Padding;
    public Vector2 m_CellSize;
    public Vector2 m_Spacing;
    public int m_ConstraintCount;
    public float m_MinimumHeight;

    RectTransform m_RectTransform;
    int m_Empty;

    #region Properties
    public UIDeck deck
    {
        private get;
        set;
    }

    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = (RectTransform)transform;
            }

            return m_RectTransform;
        }
    }
    #endregion

    public delegate void OnLayoutBuildCallback();
    public OnLayoutBuildCallback onLayoutBuildCallback;

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            if (m_IsOwnCollection)
            {
                Kernel.entry.character.onDeckDataUpdateCallback += OnDeckDataUpdate;
            }
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            if (m_IsOwnCollection)
            {
                Kernel.entry.character.onDeckDataUpdateCallback -= OnDeckDataUpdate;
            }
        }
    }

    void OnDeckDataUpdate(int deckNo, int slotNo, long cid)
    {
        if (deckNo != 0 || !m_IsOwnCollection)
        {
            return;
        }

        CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(cid);
        if (cardInfo != null)
        {
            Add(cardInfo.m_iCardIndex, cardInfo.m_Cid);
            // ref. PUC-639
            BuildLayout();
        }
    }

    public void BuildLayout()
    {
        List<UICharCard> charCardList;
        if (m_IsOwnCollection)
        {
            charCardList = m_CharCardList.OrderByDescending(item => item.gradeType) // gradeType 내림차순 정렬
                                          .ThenByDescending(item => item.cardIndex).ToList<UICharCard>(); // cardIndex 내림차순 정렬
        }
        else
        {
            charCardList = m_CharCardList.OrderBy(item => item.area) // area 오름차순 정렬
                                          .ThenByDescending(item => item.gradeType) // gradeType 내림차순 정렬
                                          .ThenByDescending(item => item.cardIndex).ToList<UICharCard>(); // cardIndex 내림차순 정렬
        }

        // Anchors
        // Min X : 0f, Y : 1f
        // Max Y : 0f, Y : 1f
        // Pivot
        // X : .5f, Y : .5f

        m_EmptyText.gameObject.SetActive(int.Equals(charCardList.Count, 0));

        float x = m_Padding.left + (m_CellSize.x * .5f);
        float y = -m_Padding.top - (m_CellSize.y * .5f);
        for (int i = 0; i < charCardList.Count; i++)
        {
            RectTransform rectTransform = charCardList[i].rectTransform;
            if (i > 0 && int.Equals((i % 6), 0))
            {
                x = m_Padding.left + (m_CellSize.x * .5f);
                y = y - m_CellSize.y - m_Spacing.y;
            }

            rectTransform.anchoredPosition = new Vector2(x, y);
            x = x + m_CellSize.x + m_Spacing.x;
        }

        #region
        m_Empty = 0;
        if (charCardList != null && charCardList.Count > 0)
        {
            m_Empty = (charCardList.Count % 6);
            if (m_Empty > 0)
            {
                m_Empty = 6 - m_Empty;
            }
        }

        for (int i = 0; i < m_EmptyImages.Count; i++)
        {
            Image image = m_EmptyImages[i];
            bool activeSelf = (m_Empty > 0) && (i < m_Empty);
            if (activeSelf)
            {
                image.rectTransform.anchoredPosition = new Vector2(x, y - ((image.rectTransform.sizeDelta.y - m_CellSize.y) * .5f));
                x = x + m_CellSize.x + m_Spacing.x;
            }

            image.gameObject.SetActive(activeSelf);
        }
        #endregion

        //x = x + m_Padding.right;
        //y = y - m_Padding.bottom;

        #region
        float rowCount = Mathf.Ceil((float)charCardList.Count / (float)m_ConstraintCount);
        y = (rowCount * m_CellSize.y) + ((rowCount - 1) * m_Spacing.y) + m_Padding.top + m_Padding.bottom;
        this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x, Mathf.Max(m_MinimumHeight, y));
        #endregion

        if (onLayoutBuildCallback != null)
        {
            onLayoutBuildCallback();
        }
    }

    #region
    UICharCard Find(long cid)
    {
        return m_CharCardList.Find(item => long.Equals(item.cid, cid));
    }

    UICharCard Find(int cardIndex)
    {
        return m_CharCardList.Find(item => int.Equals(item.cardIndex, cardIndex));
    }
    #endregion

    #region
    public UICharCard Remove(long cid, bool buildLayout)
    {
        UICharCard item = Find(cid);
        if (item)
        {
            return Remove(item, buildLayout);
        }

        return null;
    }

    public UICharCard Remove(int cardIndex, bool buildLayout)
    {
        UICharCard item = Find(cardIndex);
        if (item)
        {
            return Remove(item, buildLayout);
        }

        return null;
    }

    UICharCard Remove(UICharCard item, bool buildLayout)
    {
        if (item != null)
        {
            item.onClicked -= deck.OnCharCardClick;
            item.gameObject.SetActive(false);
            UIUtility.SetParent(item.transform, m_GameObjectPool.transform);
            m_CharCardList.Remove(item);
            m_GameObjectPool.Push(item.gameObject);

            if (buildLayout)
            {
                BuildLayout();
            }

            return item;
        }

        return null;
    }
    #endregion

    #region
    public UICharCard Add(int cardIndex, long cid)
    {
        if (!Find(cardIndex))
        {
            UICharCard item = Add();
            if (item)
            {
                item.cid = cid;

                return item;
            }
        }

        return null;
    }

    public UICharCard Add(int cardIndex)
    {
        if (!Find(cardIndex))
        {
            UICharCard item = Add();
            if (item)
            {
                item.cardIndex = cardIndex;

                return item;
            }
        }

        return null;
    }

    UICharCard Add()
    {
        if (m_GameObjectPool)
        {
            UICharCard item = m_GameObjectPool.Pop<UICharCard>();
            if (item)
            {
                item.onClicked += deck.OnCharCardClick;
                item.gameObject.SetActive(true);
                UIUtility.SetParent(item.transform, m_GridLayoutGroup.transform);
                m_CharCardList.Add(item);

                return item;
            }
        }

        return null;
    }
    #endregion
}
