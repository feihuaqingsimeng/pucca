using Common.Packet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharCardOption : UIObject
{
    public Image m_BackgroundImage;
    public Button m_InfoButton;
    public Button m_PositionButton;
    public Button m_LeaderButton;
    public Button m_UseButton;

    RectTransform m_Target;
    long m_CID;

    // cid 프로퍼티에서 deckNo 값을 사용하기 때문에, 항상 deckNo 값이 cid 값보다 먼저 설정되어야 합니다.
    public int deckNo
    {
        get;
        set;
    }

    protected override void Awake()
    {
        base.Awake();
        m_InfoButton.onClick.AddListener(OnInfoButtonClick);
        m_PositionButton.onClick.AddListener(OnPositionButtonClick);
        m_LeaderButton.onClick.AddListener(OnLeaderButtonClick);
        m_UseButton.onClick.AddListener(OnUseButtonClick);

        if (Kernel.uiManager != null)
        {
            UIDeck deck = Kernel.uiManager.Get<UIDeck>(UI.Deck);
            if (deck != null)
            {
                deck.m_ScrollRectContentActivator.Add(rectTransform);
            }
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 401)
        {
            Kernel.entry.tutorial.onSetNextTutorial_Delay();
        }

    }


    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (EventSystem.current.currentSelectedGameObject == null ||
            (m_Target != null &&
            EventSystem.current.currentSelectedGameObject != m_Target.gameObject &&
            !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform)))
        {
            active = false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_Target = null;
        deckNo = -1;
        m_CID = -1;
    }

    public RectTransform target
    {
        get
        {
            return m_Target;
        }

        set
        {
            //if (m_Target != value)
            {
                m_Target = value;

                if (m_Target != null)
                {
                    int siblingIndex = (m_Target.parent != null) ? m_Target.parent.childCount - 1 : 0;
                    m_Target.SetSiblingIndex(siblingIndex);
                    UIUtility.SetParent(transform, m_Target, m_Target.FindChild("Slider").GetSiblingIndex() + 1); // 임시
                    rectTransform.anchoredPosition = new Vector2(0f, 11f); // 11f :
                }
            }
        }
    }

    public long cid
    {
        set
        {
            if (m_CID != value)
            {
                m_CID = value;

                if (Kernel.entry.tutorial.TutorialActive)
                    Kernel.entry.tutorial.CardInfo_CID = m_CID;


                // Functionalization, Flexible.
                CDeckData deckData = Kernel.entry.character.FindDeckData(deckNo);
                m_PositionButton.gameObject.SetActive(deckData != null && deckData.m_CardCidList.Contains(m_CID));
                m_LeaderButton.gameObject.SetActive(deckData != null && deckData.m_CardCidList.Contains(m_CID) && deckData.m_LeaderCid != m_CID);
                m_UseButton.gameObject.SetActive(deckData == null || !deckData.m_CardCidList.Contains(m_CID));

                float y = 12f; // 12f : GridLayoutGroup.padding.bottom
                int count = 0;
                if (m_InfoButton.gameObject.activeSelf) count++;
                if (m_PositionButton.gameObject.activeSelf) count++;
                if (m_LeaderButton.gameObject.activeSelf) count++;
                if (m_UseButton.gameObject.activeSelf) count++;
                y = y + (count * 67f); // 67f : GridLayoutGroup.cellSize.y
                y = y + (count * 3f); // 3f : GridLayoutGroup.spacing.y
                y = y + 160f; // 160f : UICharCard.rectTransform.sizeDelta.y
                y = y + 11f; // 11f :

                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, y);
            }
        }
    }

    void OnInfoButtonClick()
    {
        if (Kernel.uiManager != null)
        {
            UICardInfo cardInfo = Kernel.uiManager.Open<UICardInfo>(UI.CardInfo);
            if (cardInfo != null)
            {
                //튜토리얼 강제.
                if (Kernel.entry.tutorial.TutorialActive)
                {
                    cardInfo.cid = Kernel.entry.tutorial.CardInfo_CID;
                    Kernel.entry.tutorial.CardInfo_CID = -1;
                }
                else
                    cardInfo.cid = m_CID;
            }
        }

        UIUtility.SetParent(transform, null);
        active = false;
    }

    void OnPositionButtonClick()
    {
        if (Kernel.uiManager != null)
        {
            UIDeck deck = Kernel.uiManager.Get<UIDeck>(UI.Deck);
            if (deck != null)
            {
                deck.Edit(m_CID, false);
            }
        }

        UIUtility.SetParent(transform, null);
        active = false;
    }

    void OnLeaderButtonClick()
    {
        if (Kernel.entry != null && deckNo != -1 && m_CID != -1)
        {
            Kernel.entry.character.UpdateDeckLeader(deckNo, m_CID);
        }

        UIUtility.SetParent(transform, null);
        active = false;
    }

    void OnUseButtonClick()
    {
        if (Kernel.uiManager != null)
        {
            UIDeck deck = Kernel.uiManager.Get<UIDeck>(UI.Deck);
            if (deck != null)
            {
                deck.Edit(m_CID, true);
            }
        }

        UIUtility.SetParent(transform, null);
        active = false;
    }
}
