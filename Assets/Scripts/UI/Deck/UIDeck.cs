using Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDeck : UIObject
{
    public List<Toggle> m_ToggleList;
    public UIScrollRect m_ScrollRect;
    public List<UICharCard> m_CharCardList;
    public Transform m_ArrowContainer;
    public List<Image> m_ArrowImageList;
    public Image m_CrownImage;
    public Image m_SelectImage;
    public GameObjectPool m_GameObjectPool;
    public UICardCollection m_OwnCardCollection;
    public CanvasGroup m_OwnCardCollectionCanvasGroup;
    public UICardCollection m_NotOwnCardCollection;
    public CanvasGroup m_NotOwnCardCollectionCanvasGroup;
    public Text m_ChangeDescriptionText;
    public UICharCard m_ChangeTargetCharCard;
    public CanvasGroup m_ChangeTargetCanvasGroup;
    public UIScrollRectContentActivator m_ScrollRectContentActivator;

    bool m_Edit = false;
    int m_EditIndex = -1;
    long m_EditCID = -1;
    bool m_FormationGuidlineActive = true;
    bool m_EditUse;
    GameObject m_CurrentSelectedGameObject;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_ToggleList.Count; i++)
        {
            m_ToggleList[i].onValueChanged.AddListener(OnToggleValueChange);
        }

        m_ScrollRect.onInitializePotentialDragged += OnInitializePotentialDragged;
        m_OwnCardCollection.deck = this;
        m_OwnCardCollection.onLayoutBuildCallback += OnRectTransformDimensionsChangeCallback;
        m_NotOwnCardCollection.deck = this;
        m_NotOwnCardCollection.onLayoutBuildCallback += OnRectTransformDimensionsChangeCallback;
        m_OwnCardCollectionCanvasGroup.alpha = 1f;
        m_OwnCardCollectionCanvasGroup.interactable = true;
        m_NotOwnCardCollectionCanvasGroup.alpha = 1f;
        m_NotOwnCardCollectionCanvasGroup.interactable = true;
        m_ChangeTargetCanvasGroup.alpha = 0f;
        m_ChangeTargetCanvasGroup.interactable = false;
        // UICharCardOption이 활성화되었을 때 겹치는 현상이 있어, 게임 오브젝트를 비활성화합니다.
        m_ChangeTargetCanvasGroup.gameObject.SetActive(false);

        for (int i = 0; i < m_OwnCardCollection.m_EmptyImages.Count; i++)
        {
            m_ScrollRectContentActivator.Add(m_OwnCardCollection.m_EmptyImages[i].rectTransform);
        }

        m_ScrollRectContentActivator.Add(m_OwnCardCollection.m_EmptyText.rectTransform);

        for (int i = 0; i < m_NotOwnCardCollection.m_EmptyImages.Count; i++)
        {
            m_ScrollRectContentActivator.Add(m_NotOwnCardCollection.m_EmptyImages[i].rectTransform);
        }

        m_ScrollRectContentActivator.Add(m_NotOwnCardCollection.m_EmptyText.rectTransform);

        for (int i = 0; i < m_CharCardList.Count; i++)
        {
            m_ScrollRectContentActivator.AddRange(m_CharCardList[i].children);
            m_CharCardList[i].onClicked += OnCharCardClick;
        }

        m_ScrollRectContentActivator.Add((RectTransform)m_ArrowContainer.transform);
        m_ScrollRectContentActivator.Add((RectTransform)m_ChangeTargetCanvasGroup.transform);
        SetFormationGuidelineActive(false);



        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive)
        {
            if (Kernel.entry.tutorial.WaitSeq == 400)
                Kernel.entry.tutorial.onSetNextTutorial();
        }


    }

    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        if (m_Edit)
        {
            if (m_CurrentSelectedGameObject != EventSystem.current.currentSelectedGameObject)
            {
                m_CurrentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

                if (m_CharCardList.Find(item => Equals(item.gameObject, m_CurrentSelectedGameObject)) == null)
                {
                    edit = false;
                }
            }
        }

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 403)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }
    }

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onDeckLeaderUpdateCallback += OnDeckLeaderUpdate;
            Kernel.entry.character.onDeckDataUpdateCallback += OnDeckDataUpdate;

            UpdateDeckData();
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onDeckLeaderUpdateCallback -= OnDeckLeaderUpdate;
            Kernel.entry.character.onDeckDataUpdateCallback -= OnDeckDataUpdate;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Kernel.uiManager != null)
        {
            // UICharCardOption은 UIDeck 하위에서 동작하기 때문에, UIDeck이 제거될 때 같이 제거되는 문제가 있습니다.
            // 임시로 UIDeck이 제거될 때 UICharCardOption도 같이 삭제하고, 개선이 필요합니다.
            Kernel.uiManager.Close(UI.CharCardOption, true);
        }
    }

    int deckNo
    {
        get
        {
            for (int i = 0; i < m_ToggleList.Count; i++)
            {
                if (m_ToggleList[i].isOn)
                {
                    return i + 1;
                }
            }

            return -1;
        }
    }

    RectTransform select
    {
        set
        {
            bool exist = (value != null);
            if (exist)
            {
                UIUtility.SetParent(m_SelectImage.transform, value, 0);
                m_SelectImage.rectTransform.anchoredPosition = Vector2.zero;
                m_SelectImage.rectTransform.sizeDelta = new Vector2(value.sizeDelta.x + 10f, value.sizeDelta.y + 10f);
            }

            m_SelectImage.rectTransform.gameObject.SetActive(exist);
        }
    }

    long leaderCID
    {
        set
        {
            UICharCard charCard = m_CharCardList.Find(item => Equals(item.cid, value));
            bool exist = (charCard != null);
            if (exist)
            {
                UIUtility.SetParent(m_CrownImage.transform, charCard.transform);
            }
            m_CrownImage.gameObject.SetActive(exist);
        }
    }

    bool m_CancelSoundPlay = false;

    bool edit
    {
        get
        {
            return m_Edit;
        }

        set
        {
            if (m_Edit != value)
            {
                m_Edit = value;

                //** UISound Enable
                for (int i = 0; i < m_CharCardList.Count; i++)
                {
                    SoundUtility sound = SoundDataInfo.FindSoundUtility(m_CharCardList[i].m_Button.gameObject);

                    if (sound == null)
                        continue;

                    sound.enabled = !m_Edit;
                }

                if (m_CancelSoundPlay && !m_Edit)
                    Kernel.soundManager.PlayUISound(SOUND.SND_UI_CANCEL);
                else
                    Kernel.soundManager.PlayUISound(SOUND.SND_UI_OK);

                m_CurrentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

                if (m_Edit)
                {
                    CDeckData deckData = Kernel.entry.character.FindDeckData(deckNo);
                    if (deckData == null)
                    {
                        Debug.LogError(deckNo);
                        return;
                    }
                    int index = deckData.m_CardCidList.IndexOf(m_EditCID);

                    m_EditIndex = index;
                    //m_ChangeTargetCharCard.cid = m_EditCID;
                    SetFormationGuidelineActive(index);

                    if (m_EditUse)
                    {
                        m_ChangeTargetCharCard.cid = m_EditCID;
                        m_ChangeTargetCharCard.gameObject.SetActive(true);
                        m_ChangeDescriptionText.text = Languages.ToString(TEXT_UI.CARD_DECK_CHANGE_CHOISE);
                    }
                    else
                    {
                        m_ChangeTargetCharCard.gameObject.SetActive(false);
                        m_ChangeDescriptionText.text = Languages.ToString(TEXT_UI.CARD_DECK_CHANGE_POSITION);
                    }
                }
                else
                {
                    m_EditCID = -1;
                    m_EditIndex = -1;
                    SetFormationGuidelineActive(false);
                }

                m_ScrollRect.verticalNormalizedPosition = 1f;
                m_ScrollRect.enabled = !value;
                StopAllCoroutines();
                StartCoroutine(CrossFadeAlpha(m_OwnCardCollectionCanvasGroup, m_Edit ? 0f : 1f));
                StartCoroutine(CrossFadeAlpha(m_NotOwnCardCollectionCanvasGroup, m_Edit ? 0f : 1f));
                // UICharCardOption이 활성화되었을 때 겹치는 현상이 있어, 게임 오브젝트를 비활성화합니다.
                m_ChangeTargetCanvasGroup.gameObject.SetActive(m_Edit);
                StartCoroutine(CrossFadeAlpha(m_ChangeTargetCanvasGroup, m_Edit ? 1f : 0f));

                for (int i = 0; i < m_ToggleList.Count; i++)
                {
                    m_ToggleList[i].interactable = !value;
                }
            }
        }
    }

    void UpdateDeckData()
    {
        if (Kernel.entry == null)
        {
            return;
        }

        CDeckData deckData = Kernel.entry.character.FindMainDeckData();
        if (deckData != null)
        {
            int deckNo = deckData.m_iDeckNum - 1;
            Toggle toggle = m_ToggleList[deckNo];
            // 이미 활성화 상태인 경우, OnToggleValueChangedCallback이 호출되지 않을 수 있어 직접 호출한다. (디버깅 필요)
            if (toggle.isOn)
            {
                OnToggleValueChange(true);
            }
            else
            {
                m_ToggleList[deckNo].isOn = true;
            }
        }

        //UpdateCardCollection();
    }

    void UpdateCardCollection()
    {
        if (Kernel.entry == null)
        {
            return;
        }

        CDeckData deckData = Kernel.entry.character.FindMainDeckData();
        List<DB_Card.Schema> table = DB_Card.instance.schemaList;
        for (int i = 0; i < table.Count; i++)
        {
            DB_Card.Schema schema = table[i];
            // 보스 몬스터 제외
            if (schema.Index >= 1000)
            {
                continue;
            }

            CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(schema.Index);
            if (cardInfo != null)
            {
                if (deckData.m_CardCidList.Contains(cardInfo.m_Cid))
                {
                    UICharCard charCard = m_OwnCardCollection.Remove(cardInfo.m_Cid, false);
                    if (charCard != null)
                    {
                        m_ScrollRectContentActivator.RemoveRange(charCard.children);
                    }
                }
                else
                {
                    UICharCard charCard = m_OwnCardCollection.Add(cardInfo.m_iCardIndex, cardInfo.m_Cid);
                    if (charCard != null)
                    {
                        m_ScrollRectContentActivator.AddRange(charCard.children);
                    }
                }
            }
            else
            {
                UICharCard charCard = m_NotOwnCardCollection.Add(schema.Index);
                if (charCard != null)
                {
                    m_ScrollRectContentActivator.AddRange(charCard.children);
                }
            }
        }

        // 카드 추가 시, Add() 내부에서 FillEmpty() 호출 시 중복 호출되므로 카드 추가 완료 후 FillEmpty()를 호출합니다.
        m_OwnCardCollection.BuildLayout();
        m_NotOwnCardCollection.BuildLayout();
        //m_OwnCardCollection.FillEmpty();
        //m_NotOwnCardCollection.FillEmpty();
    }

    void OnRectTransformDimensionsChangeCallback()
    {
        float y = 0f;
        y = m_OwnCardCollection.rectTransform.anchoredPosition.y - m_OwnCardCollection.rectTransform.sizeDelta.y - 10f - 48f; // 10f : , 48f :
        m_NotOwnCardCollection.rectTransform.anchoredPosition = new Vector2(m_NotOwnCardCollection.rectTransform.anchoredPosition.x, y);

        y = y - m_NotOwnCardCollection.rectTransform.sizeDelta.y - 10f; // 10f :
        y = Mathf.Abs(y);
        m_ScrollRect.content.sizeDelta = new Vector2(m_ScrollRect.content.sizeDelta.x, y);
    }

    void SetDeckData(CDeckData deckData)
    {
        if (deckData != null)
        {
            for (int i = 0; i < m_CharCardList.Count; i++)
            {
                UICharCard charCard = m_CharCardList[i];
                // [WARNING] NullRefExcpt!
                charCard.cid = deckData.m_CardCidList[i];
            }

            OnDeckLeaderUpdate(deckData.m_iDeckNum, deckData.m_LeaderCid);
        }
    }

    public void Edit(long cid, bool use)
    {
        m_EditCID = cid;
        m_EditUse = use;
        edit = true;
    }

    static UICharCard m_Clicked;

    public void OnCharCardClick(UICharCard charCard)
    {
        if (m_Clicked != null)
        {
            m_Clicked.m_NameBackgroundImage.gameObject.SetActive(false);
        }

        m_Clicked = charCard;
        m_Clicked.m_NameBackgroundImage.gameObject.SetActive(true && !m_Edit);

        if (charCard.cid > 0)
        {
            select = charCard.rectTransform;
            SoundDataInfo.RevertSound(charCard.m_Button.gameObject);
        }
        else
        {
            DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, charCard.cardIndex);
            if (card != null)
            {
                UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.CARD_DECK_AREA_GET, card.Drop_Area > 0 ? card.Drop_Area.ToString() : Languages.ToString(TEXT_UI.SECRET_EXCHANGE)));
                SoundDataInfo.CancelSound(charCard.m_Button.gameObject);

                // ref. PUC-870
                UISecretCardInfo secretCardInfo = Kernel.uiManager.Get<UISecretCardInfo>(UI.SecretCardInfo, true, false);
                if (secretCardInfo != null)
                {
                    secretCardInfo.SetCharBaseData(card);
                    Kernel.uiManager.Open(UI.SecretCardInfo);
                }
            }
        }

        if (m_Edit)
        {
            if (m_EditCID != charCard.cid)
            {
                if (m_EditIndex > -1)
                {
                    Kernel.entry.character.UpdateDeckData(deckNo, m_EditIndex, charCard.cid);
                }

                // 편집 상태에서 보유 목록, 미보유 목록의 UICharCard가 선택되는 것을 방지합니다.
                int index = m_CharCardList.IndexOf(charCard);
                if (index > -1)
                {
                    Kernel.entry.character.UpdateDeckData(deckNo, index, m_EditCID);
                    m_CancelSoundPlay = false;
                }
            }
            edit = false;
        }
        else
        {
            m_CancelSoundPlay = true;
            #region
            RectTransform rectTransform = null;
            if (charCard.cid > 0)
            {
                UICharCardOption charCardOption = Kernel.uiManager.Open<UICharCardOption>(UI.CharCardOption);
                if (charCardOption != null)
                {
                    charCardOption.target = charCard.rectTransform;
                    charCardOption.deckNo = deckNo;
                    charCardOption.cid = charCard.cid;
                    rectTransform = charCardOption.rectTransform;
                }
            }
            else
            {
                rectTransform = charCard.rectTransform;
            }
            #endregion

            //StopAllCoroutines();
            StopCoroutine("ScrollTo");
            StartCoroutine(ScrollTo(rectTransform));
        }
    }

    IEnumerator CrossFadeAlpha(CanvasGroup cg, float value)
    {
        value = Mathf.Clamp01(value);
        float alpha = cg.alpha;

        // F A D E O U T
        while (alpha > value)
        {
            alpha = Mathf.Clamp01(alpha - (Time.deltaTime * 2f));
            cg.alpha = alpha;
            if (alpha <= value)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;

                yield break;
            }

            yield return 0;
        }

        // F A D E I N
        while (alpha <= value)
        {
            alpha = Mathf.Clamp01(alpha + (Time.deltaTime * 2f));
            cg.alpha = alpha;
            if (alpha >= value)
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;

                yield break;
            }

            yield return 0;
        }
    }

    IEnumerator ScrollTo(RectTransform rectTransform)
    {
        Vector3 worldPosition = rectTransform.TransformPoint(rectTransform.rect.min);
        Vector3 localPosition = m_ScrollRect.viewport.InverseTransformPoint(worldPosition);
        bool contains = m_ScrollRect.viewport.rect.Contains(localPosition);
        while (!contains && m_ScrollRect.velocity.y <= 0f)
        {
            m_ScrollRect.verticalNormalizedPosition = m_ScrollRect.verticalNormalizedPosition - Time.deltaTime;

            worldPosition = rectTransform.TransformPoint(rectTransform.rect.min);
            localPosition = m_ScrollRect.viewport.InverseTransformPoint(worldPosition);
            contains = m_ScrollRect.viewport.rect.Contains(localPosition);

            yield return 0;
        }

        worldPosition = rectTransform.TransformPoint(rectTransform.rect.max);
        localPosition = m_ScrollRect.viewport.InverseTransformPoint(worldPosition);
        contains = m_ScrollRect.viewport.rect.Contains(localPosition);
        while (!contains && m_ScrollRect.velocity.y <= 0f)
        {
            m_ScrollRect.verticalNormalizedPosition = m_ScrollRect.verticalNormalizedPosition + Time.deltaTime;

            worldPosition = rectTransform.TransformPoint(rectTransform.rect.max);
            localPosition = m_ScrollRect.viewport.InverseTransformPoint(worldPosition);
            contains = m_ScrollRect.viewport.rect.Contains(localPosition);

            yield return 0;
        }

        yield break;
    }

    void OnInitializePotentialDragged(PointerEventData eventData)
    {
        //StopAllCoroutines();
        StopCoroutine("ScrollTo");
    }

    void OnDeckDataUpdate(int deckNo, int index, long cid)
    {
        if (this.deckNo != deckNo)
        {
            return;
        }

        // Warning, NullRefExcpt!
        UICharCard charCard = m_CharCardList[index];
        charCard.cid = cid;

        select = charCard.rectTransform;

        // Warning, 리더 캐릭터의 위치가 변경되었을 때를 위한 임시 처리
        CDeckData deckData = Kernel.entry.character.FindDeckData(deckNo);
        if (deckData != null)
        {
            leaderCID = deckData.m_LeaderCid;
        }

        m_OwnCardCollection.Remove(cid, true);
    }

    void OnDeckLeaderUpdate(int deckNo, long leaderCID)
    {
        if (!m_ToggleList[deckNo - 1].isOn)
        {
            return;
        }

        this.leaderCID = leaderCID;
    }

    void OnToggleValueChange(bool value)
    {
        if (!value)
        {
            // To avoid successive invoke.
            return;
        }

        select = null;

        CDeckData deckData = Kernel.entry.character.FindDeckData(deckNo);
        if (deckData != null)
        {
            Kernel.entry.character.SetMainDeck(deckNo);
            SetDeckData(deckData);
        }
        else
        {
            Debug.LogError(deckNo);
        }

        UpdateCardCollection();
    }

    void SetFormationGuidelineActive(bool value)
    {
        if (m_FormationGuidlineActive != value)
        {
            m_FormationGuidlineActive = value;

            m_ArrowContainer.SetAsLastSibling();

            for (int i = 0; i < m_ArrowImageList.Count; i++)
            {
                m_ArrowImageList[i].gameObject.SetActive(value);
            }
        }
    }

    void SetFormationGuidelineActive(int index)
    {
        if (!m_FormationGuidlineActive)
        {
            m_FormationGuidlineActive = true;
        }

        m_ArrowContainer.SetAsLastSibling();

        for (int i = 0; i < m_ArrowImageList.Count; i++)
        {
            m_ArrowImageList[i].gameObject.SetActive(i != index);
        }
    }
}
