using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Packet;

public enum eSpeakType
{
    ST_NONE,
    ST_START,
    ST_CARD_DECK_OPEN,
    ST_CARD_DECK_CLOSE,
    ST_OPEN_ABLE,
}


public class UISecretBusiness : UIObject
{
    [Space(10)]
    public RectTransform[]          m_arrCardPosition;
    public UISecretCharCard         m_CharCardPrefab;
    private List<UISecretCharCard>  m_listCopyCharCard = new List<UISecretCharCard>();

    public UISecretChoiceDeck       m_ChoiceDeck;

    public SkeletonAnimation        m_BoxSkeletonAnim;
    public SkeletonAnimation        m_GoldManSkeletonAnim;

    public GameObject               m_CompletObject;

    private Sprite                  m_OkAbleSprite;
    private Sprite                  m_OkEnAbleSprite;

    public Image                    m_SecretBoxImage;
    public Image                    m_OkButtonImg;

    public Text                     m_SpeakText;
    public Text                     m_OkButtonText;

    [Header("Button")]
    public Button                   m_OkButton;
    public Button                   m_HelpButton;

    //** Current Data
    private Dictionary<int, CharCardData> m_dicSlotSelectCard = new Dictionary<int, CharCardData>();

    private int m_nCurrentSlotType;
    public int currentSlotType
    {
        get { return m_nCurrentSlotType; }
        set 
        { m_nCurrentSlotType = value <= 0 ? 1 : value; }
    }

    [HideInInspector]
    public  int  m_nSelectedSlotIndex;

    private bool m_bComplet;


    #region MonoBehavior 기본 함수

    protected override void Awake()
    {
        m_OkButton.onClick.AddListener(OnClickOk);
        m_HelpButton.onClick.AddListener(OnClickHelp);

        Kernel.entry.secretBusiness.onOpenSecretBoxCallback += OnOpenSecretBox;
        Kernel.entry.secretBusiness.onClickHaveCardCallBack += UpdateCard;
        Kernel.entry.secretBusiness.onClickSlotCardCallBack += OnSelectCard;

        SetBaseUI(); //FGT : 이후에 BoxSelect 씬이 추가가 되면 필요가 없어짐.
        SetInit(); //FGT : 이후에 BoxSelect 씬이 추가가 되면 필요가 없어짐.

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 1100)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }

        base.Awake();
    }

    protected override void OnDestroy()
    {
        m_OkButton.onClick.RemoveAllListeners();
        m_HelpButton.onClick.RemoveAllListeners();

        Kernel.entry.secretBusiness.onOpenSecretBoxCallback -= OnOpenSecretBox;
        Kernel.entry.secretBusiness.onClickHaveCardCallBack -= UpdateCard;
        Kernel.entry.secretBusiness.onClickSlotCardCallBack -= OnSelectCard;

        base.OnDestroy();
    }

    ////FGT : 이후에 BoxSelect 씬이 추가가 되면 필요함.
    //protected override void OnEnable()
    //{
    //    UIHUD.instance.onBackButtonClicked += OnCloseBackButton;

    //    SetBaseUI();
    //    SetInit();

    //    base.OnEnable();
    //}

    ////FGT : 이후에 BoxSelect 씬이 추가가 되면 필요함.
    //protected override void OnDisable()
    //{
    //    UIHUD.instance.onBackButtonClicked -= OnCloseBackButton;
    //    base.OnDisable();
    //}

    #endregion

    #region Get 함수

    //** 스파인 캐릭터의 말 반환
    private string GetSpeakText(eSpeakType speakType)
    {
        if (m_bComplet)
            speakType = eSpeakType.ST_OPEN_ABLE;

        int randomNum = 0;

        switch (speakType)
        {
            case eSpeakType.ST_NONE: return "";
            case eSpeakType.ST_START:
                randomNum = Random.Range(1, 3);
                break;
            case eSpeakType.ST_CARD_DECK_OPEN:
                randomNum = Random.Range(4, 7);
                break;
            case eSpeakType.ST_CARD_DECK_CLOSE:
                randomNum = Random.Range(2, 3);
                break;
            case eSpeakType.ST_OPEN_ABLE:
                randomNum = Random.Range(8, 9);
                break;
        }

        string speakText = string.Format("SECRET_EXCHANGE_NPC_0{0}", randomNum);
        TEXT_UI speakTextUI = (TEXT_UI)System.Enum.Parse(typeof(TEXT_UI), speakText);
        string speak = Languages.ToString(speakTextUI);

        return speak;
    }

    //** 골드 맨 상황에 맞는 애니메이션
    private string GetGoldManAnimation(eSpeakType speakType)
    {
        if (m_bComplet)
            speakType = eSpeakType.ST_OPEN_ABLE;

        switch (speakType)
        {
            case eSpeakType.ST_NONE:
            case eSpeakType.ST_START:               
            case eSpeakType.ST_CARD_DECK_CLOSE:     return "talk1";
            case eSpeakType.ST_CARD_DECK_OPEN:      return "talk3";
            case eSpeakType.ST_OPEN_ABLE:           return "talk2";
        }

        return "talk1";
    }

    //** 선택된 슬롯아이템(UISecretCharCard) 반환.
    private UISecretCharCard GetSelectSlotItem()
    {
        return m_listCopyCharCard.Find(item => item.m_SlotData.m_nSlotIndex == m_nSelectedSlotIndex);
    }

    //** 현재 슬롯의 이미 선택된 카드 반환
    public CharCardData GetSlotSelectedCard()
    {
        if (!m_dicSlotSelectCard.ContainsKey(m_nSelectedSlotIndex))
            return null;

        return m_dicSlotSelectCard[m_nSelectedSlotIndex];
    }

    //** 박스를 열수 있는지??
    public bool IsOpenBox()
    {
        bool isFull = true;

        if (m_dicSlotSelectCard.Count <= 0)
            isFull = false;

        foreach (CharCardData selectCard in m_dicSlotSelectCard.Values)
        {
            if (selectCard != null)
                continue;

            isFull = false;
        }
        return isFull;
    }

    #endregion

    #region Set 함수

    //** 초기화
    private void SetInit()
    {
        if (m_dicSlotSelectCard != null)
        {
            List<int> seletCardKey = new List<int>();

            foreach (int key in m_dicSlotSelectCard.Keys)
                seletCardKey.Add(key);

            for (int i = 0; i < seletCardKey.Count; i++)
                m_dicSlotSelectCard[seletCardKey[i]] = null;
        }

        
        m_bComplet = false;
        CloseDeck();
        SetBaseData(0); //FGT : 이후에 BoxSelect 씬이 추가가 되면 필요가 없어짐.
        UpdateOpenState();
        SetUI();

    }

    private void CloseDeck()
    {
        if (m_ChoiceDeck.gameObject.activeSelf)
            m_ChoiceDeck.gameObject.SetActive(false);

        m_nSelectedSlotIndex = 0;
    }

    //** 기본 UI 세팅
    private void SetBaseUI()
    {
        m_OkButtonText.text = Languages.ToString(TEXT_UI.EXCHANGE);
        m_OkAbleSprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_02");
        m_OkEnAbleSprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");
    }

    //** 기본 Data 세팅
    public void SetBaseData(int slotType)
    {
        currentSlotType = slotType;

        SecretBoxData listBoxData = Kernel.entry.secretBusiness.GetBoxData(currentSlotType);

        foreach (SecretSlotData slotData in listBoxData.m_dicSlotData.Values)
        {
            if (slotData.m_listCharCardData.Count > 0)
                slotData.m_listCharCardData.Clear();

            slotData.m_listCharCardData = Kernel.entry.secretBusiness.GetCharCardList(slotData.m_nNeedCount, slotData.m_eGradeType, slotData.m_eClassType);
        }
    }

    //** 슬롯 카드 정보들 세팅
    private void SetUI()
    {
        SecretBoxData listBoxData = Kernel.entry.secretBusiness.GetBoxData(currentSlotType);

        if (listBoxData == null)
            return;

        //m_SpeakText.text = GetSpeakText(eSpeakType.ST_START);
        SetStateAnimAndSpeak(eSpeakType.ST_START);

        //스켈레톤 애니메이션 및 이미지
        SetGoldManSkeletonAnimation();
        SetBoxSkeletonAnimation(listBoxData.m_strBoxIconName);
        m_SecretBoxImage.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, listBoxData.m_strBoxIconName);

        Dictionary<int, SecretSlotData> dicSlotData = listBoxData.m_dicSlotData;

        if (dicSlotData == null)
            return;

        // 이미 만든거면 갱신, 아니면 새로 만들기
        int count = 0;
        bool areadyCreate = m_listCopyCharCard != null && m_listCopyCharCard.Count > 0;

        foreach (KeyValuePair<int, SecretSlotData> slotData in dicSlotData)
        {
            if (areadyCreate)
                m_listCopyCharCard[count].SetCard(slotData.Value, null, eCardType.CT_NEED_SELECT_CARD);
            else
            {
                UISecretCharCard secretCard = Instantiate<UISecretCharCard>(m_CharCardPrefab);
                UIUtility.SetParent(secretCard.transform, m_arrCardPosition[count].transform);
                secretCard.SetCard(slotData.Value, null, eCardType.CT_NEED_SELECT_CARD);
                m_listCopyCharCard.Add(secretCard);

                // 선택 카드 담을 빈 dic 제작
                if (!m_dicSlotSelectCard.ContainsKey(slotData.Value.m_nSlotIndex))
                    m_dicSlotSelectCard.Add(slotData.Value.m_nSlotIndex, null);
            }
            count++;
        }
    }

    private void SetStateAnimAndSpeak(eSpeakType type)
    {
        m_SpeakText.text = GetSpeakText(type);

        // 이미 같은 애니메이션 실행 중이라면..
        if (m_GoldManSkeletonAnim.AnimationName == GetGoldManAnimation(type))
            return;

        m_GoldManSkeletonAnim.AnimationName = GetGoldManAnimation(type);
        m_GoldManSkeletonAnim.loop = true;
        m_GoldManSkeletonAnim.Reset();
    }

    //** 골드 맨 SkeletonAnimation
    private void SetGoldManSkeletonAnimation()
    {
        string assetPath = "Spines/SecretGoldMan/SecretGoldMan_SkeletonData";

        SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
        if (skeletonDataAsset != null)
        {
            m_GoldManSkeletonAnim.skeletonDataAsset = skeletonDataAsset;
            m_GoldManSkeletonAnim.initialSkinName = "Secret_goldman";
        }
    }

    //** 전설 상자 SkeletonAnimation
    private void SetBoxSkeletonAnimation(string identificationName)
    {
        if (!string.IsNullOrEmpty(identificationName))
        {
            string assetPath = string.Format("Spines/RewardBox/{0}/{0}_SkeletonData", identificationName);
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                m_BoxSkeletonAnim.skeletonDataAsset = skeletonDataAsset;
                m_BoxSkeletonAnim.initialSkinName = identificationName;
                m_BoxSkeletonAnim.AnimationName = "lock";
                m_BoxSkeletonAnim.loop = true;
                m_BoxSkeletonAnim.Reset();
            }
            else
            {
                Debug.LogError(assetPath);
            }
        }
    }

    //** 슬롯에 선택된 카드 등록 (같은 카드면 선택 해지)
    public void SetSelectCard(CharCardData charCard)
    {
        if (m_dicSlotSelectCard[m_nSelectedSlotIndex] == charCard)
            m_dicSlotSelectCard[m_nSelectedSlotIndex] = null;
        else
            m_dicSlotSelectCard[m_nSelectedSlotIndex] = charCard;
    }

    #endregion

    #region Update 함수

    //** 선택된 슬롯 아이템의 카드 정보를 업데이트.
    private void UpdateCard(CharCardData charCardData)
    {
        SecretBusiness secret = Kernel.entry.secretBusiness;

        if (secret == null)
            return;

        SetSelectCard(charCardData);

        CharCardData selectedCard = GetSlotSelectedCard();
        UISecretCharCard selectSlotItem = GetSelectSlotItem();

        if (selectedCard != null)
            //selectSlotItem.SetChoiceCard(charCardData);
            selectSlotItem.SetSlotInputCard(charCardData);
        else
            selectSlotItem.SetEmptyCard();

        UpdateOpenState();
    }

    //** 거래 버튼 상태 업데이트
    private void UpdateOpenState()
    {
        bool isOpen = IsOpenBox();

        // 버튼 활성/비활성 화
        SoundDataInfo.CancelSound(m_OkButton.gameObject);
        m_OkButton.enabled = isOpen;
        m_OkButtonImg.sprite = isOpen ? m_OkAbleSprite : m_OkEnAbleSprite;

        // 스켈레톤 이미지 또는 고정 이미지 활성/비활성 화
        m_SecretBoxImage.gameObject.SetActive(!isOpen);
        m_CompletObject.SetActive(isOpen);

        m_bComplet = isOpen;

        // Speak
        if (isOpen)
        {
            SoundDataInfo.ChangeUISound(SOUND.SND_UI_OK, m_OkButton.gameObject);
            Kernel.soundManager.PlayUISound(SOUND.SND_UI_SECRETEXCHANGE_READY);
            SetStateAnimAndSpeak(eSpeakType.ST_OPEN_ABLE);
            CloseDeck();
        }
        else
        {
            SetStateAnimAndSpeak(eSpeakType.ST_CARD_DECK_OPEN);
        }
            
    }

    //** 거래 후 카운트 및 해당 유무로 카드들 정리
    private void UpdateCharCardDataInOrder()
    {
        SecretBoxData boxData = Kernel.entry.secretBusiness.GetBoxData(currentSlotType);

        if (boxData == null)
            return;

        foreach (SecretSlotData slotData in boxData.m_dicSlotData.Values)
        {
            CharCardData charCard = m_dicSlotSelectCard[slotData.m_nSlotIndex];

            if (charCard == null)
                continue;

            CharCardData cardDataItem = slotData.m_listCharCardData.Find(item => item.m_CardData == charCard.m_CardData);

            if (cardDataItem != null)
            {
                // 01. 카드 카운드 재등록.
                cardDataItem.m_nHaveCount = charCard.m_nHaveCount - slotData.m_nNeedCount;

                // 02. 갱신된 카드 갯수가 필요 갯수보다 적으면 조건 만족 카드 리스트에서 제외시키기.
                if (cardDataItem.m_nHaveCount < slotData.m_nNeedCount)
                    slotData.m_listCharCardData.Remove(cardDataItem);
            }
        }
    }

    #endregion

    #region Button 함수

    //** 거래 버튼
    private void OnClickOk()
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.SECRET_EXCHANGE_INFO),
                          UIAlerter.Composition.Confirm_Cancel,
                          OnResponseCallback,
                          Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }

    //** 카드 선택시
    private void OnSelectCard(int slotIndex)
    {
        // 선택 창이 열어야하는가?
        bool bAreadyOpenSlot = m_nSelectedSlotIndex == slotIndex;

        m_ChoiceDeck.gameObject.SetActive(!bAreadyOpenSlot);
        m_nSelectedSlotIndex = bAreadyOpenSlot ? 0 : slotIndex;

        // 이미 열려있지 않는다면? => 열어야 되는 상황이면
        if (!bAreadyOpenSlot)
        {
            m_ChoiceDeck.CreateItems(currentSlotType, m_nSelectedSlotIndex, GetSlotSelectedCard());

            RectTransform transItem = GetSelectSlotItem().transform.parent.GetComponent<RectTransform>();
            m_ChoiceDeck.SetArrowPosition(transItem.anchoredPosition.x + transItem.rect.width * 0.5f);

            SetStateAnimAndSpeak(eSpeakType.ST_CARD_DECK_OPEN);
        }
        // 이미 열려 있다면? =? 닫아야 하는 상황이면
        else
        {
            SetStateAnimAndSpeak(eSpeakType.ST_CARD_DECK_CLOSE);
        }
    }

    //** 카드 정보 도움 버튼
    private void OnClickHelp()
    {
        UISecretCardHelp helpPopup = UIManager.Instance.Open<UISecretCardHelp>(UI.SecretCardHelp);
        helpPopup.CreateItems(currentSlotType);
    }

    #endregion

    #region CallBack

    //** 거래 확인 버튼 눌렀을때 콜백
    private void OnResponseCallback(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
            return;

        List<long> selectCardSeq = new List<long>();

        foreach (CharCardData card in m_dicSlotSelectCard.Values)
            selectCardSeq.Add(Kernel.entry.character.FindSoulInfo(card.m_CardInfo.m_iCardIndex).m_Sequence);

        Kernel.entry.secretBusiness.REQ_PACKET_CG_CARD_SECRET_EXCHANGE_SYN((byte)currentSlotType, selectCardSeq);
    }

    //** 거래하고 난 뒤의 CallBack
    private void OnOpenSecretBox(int cardIndex)
    {
        // 허용 캐릭터 카드 갱신
        UpdateCharCardDataInOrder();

        UIChestDirector chestDirector = Kernel.uiManager.Get<UIChestDirector>(UI.ChestDirector, true, false);
        if (chestDirector != null)
        {
            CBoxResult boxResult = new CBoxResult();
            boxResult.m_iCardCount = 1;
            boxResult.m_iCardIndex = cardIndex;

            List<CBoxResult> listBoxResult = new List<CBoxResult>();
            listBoxResult.Add(boxResult);

            SecretBoxData listBoxData = Kernel.entry.secretBusiness.GetBoxData(currentSlotType);

            chestDirector.SetReward(0, 0, listBoxResult, listBoxData.m_strBoxIconName);
            Kernel.uiManager.Open(UI.ChestDirector);
            chestDirector.DirectionByCoroutine();
        }

        SetInit();
    }

    #endregion

    private bool OnCloseBackButton()
    {
        OnCloseButtonClick();

        return true;
    }
}
