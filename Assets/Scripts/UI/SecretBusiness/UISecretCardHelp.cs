using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISecretCardHelp : UIObject 
{
    private const int N_CREAT_ITEMS_GROUP_COUNT = 5;

    private int m_CreatSlotNum;

    [Space(10)]
    [Header("BaseUI")]
    public Text m_Title;
    public Text m_Dec;
    
    [Header("Copy ScrollItem")]
    public  UIScrollRect             m_Scroll;
    public  UISecretCharCard         m_CharCardPrefab;
    private List<UISecretCharCard>   m_listCopyCards = new List<UISecretCharCard>();

    protected override void Awake()
    {
        Kernel.entry.secretBusiness.onClickCardInfoCallBack += OnSelectCard;

        SetUI();

        base.Awake();
    }

    protected override void OnDestroy()
    {
        Kernel.entry.secretBusiness.onClickCardInfoCallBack -= OnSelectCard;

        base.OnDestroy();
    }

    //** 초기화
    private void SetInit()
    {
        if(m_listCopyCards != null)
        {
            for(int i = 0; i < m_listCopyCards.Count; i++)
                Destroy(m_listCopyCards[i].gameObject);

            m_listCopyCards.Clear();
        }
    }

    //** 기본 고정 ui
    private void SetUI()
    {
        m_Title.text = Languages.ToString(TEXT_UI.CARD_POSSIBLE_GET);
        m_Dec.text = Languages.ToString(TEXT_UI.POSSIBLE_GET_INFO);
    }

    //** 스크롤 아이템 생성
    public void CreateItems(int slotType)
    {
        if (slotType == m_CreatSlotNum)
            return;

        SetInit();

        SecretBoxData slotData = Kernel.entry.secretBusiness.GetBoxData(slotType);

        if (slotData == null)
            return;

        List<CharCardData> legendCardList = slotData.m_listLegendCardData;

        if (legendCardList == null || legendCardList.Count == 0)
            return;

        List<RectTransform> trsItems = new List<RectTransform>();

        for (int i = 0; i < legendCardList.Count; i++)
        {
            UISecretCharCard newCharCard = Instantiate<UISecretCharCard>(m_CharCardPrefab);
            UIUtility.SetParent(newCharCard.transform, m_Scroll.content.transform);
            newCharCard.SetLegendCard(legendCardList[i]);

            m_listCopyCards.Add(newCharCard);
            trsItems.Add(newCharCard.gameObject.GetComponent<RectTransform>());
        }

        m_Scroll.enabled = trsItems.Count > N_CREAT_ITEMS_GROUP_COUNT;
        UIUtility.SetReposition(m_Scroll.content, trsItems, 14.0f, true ,0.0f, 0.0f, N_CREAT_ITEMS_GROUP_COUNT);
    }

    //** 카드 선택시
    public void OnSelectCard(CharCardData charCardData)
    {
        UISecretCardInfo InfoPopup = UIManager.Instance.Open<UISecretCardInfo>(UI.SecretCardInfo);
        InfoPopup.SetCharBase(charCardData);
    }
}
