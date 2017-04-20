using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISecretChoiceDeck : MonoBehaviour
{
    //** Copy Scroll Items
    [Space(10)]
    public  UISecretCharCard        m_CharCardPrefab;
    private List<UISecretCharCard>  m_listCardDatas         = new List<UISecretCharCard>();
    private List<RectTransform>     m_listCopyPrefabs       = new List<RectTransform>();
    public  UIScrollRect            m_Scroll;
    
    public  Image                   m_Arrow;
    public  RectTransform           m_trsArrow;

    public  Text                    m_NothingDec;

    private CharCardData            m_PreSelectCard;

    private void Awake()
    {
        m_NothingDec.text = Languages.ToString(TEXT_UI.CHARACTER_LIST_NONE);

        Kernel.entry.secretBusiness.onClickHaveCardCallBack += OnSelectCard;
    }

    private void OnDestroy()
    {
        Kernel.entry.secretBusiness.onClickHaveCardCallBack -= OnSelectCard;
    }

    //** 초기화
    private void SetInit()
    {
        m_Scroll.content.anchoredPosition = Vector2.zero;
    }

    //** ScrollItem 생산
    public void CreateItems(int slotType, int slotIndex, CharCardData selectCard)
    {
        SetInit();

        SecretSlotData slotData = Kernel.entry.secretBusiness.GetSlotData(slotType, slotIndex);
        List<CharCardData> listCharCard = slotData.m_listCharCardData;

        m_NothingDec.gameObject.SetActive(listCharCard.Count <= 0);

        m_PreSelectCard = selectCard;

        bool areadyCreate = listCharCard != null && m_listCardDatas.Count > 0;
        int dataCount = listCharCard.Count;
        int remainCount = areadyCreate ? m_listCardDatas.Count - dataCount : 0;

        // 재사용 및 생성
        for (int i = 0; i < dataCount; i++)
        {
            if (areadyCreate)
            {
                if (i < m_listCardDatas.Count)
                {
                    m_listCardDatas[i].SetCard(slotData, listCharCard[i], eCardType.CT_HAVE_CARD);
                    m_listCardDatas[i].SetSelectCardMark(selectCard);
                    m_listCardDatas[i].gameObject.SetActive(true);
                    continue;
                }
            }
            CreateItems(slotData, listCharCard[i], selectCard);
        }

        // 남는 것은 액티브를 꺼준다.
        if (remainCount > 0)
        {
            for (int i = remainCount; i > 0; i--)
                m_listCardDatas[m_listCardDatas.Count - i].gameObject.SetActive(false);
        }

        //켜져 있는 것들만 위치 정리한다.
        List<RectTransform> listRepositionRect = new List<RectTransform>();
        for (int i = 0; i < m_listCardDatas.Count; i++)
        {
            if (m_listCardDatas[i].gameObject.activeSelf)
                listRepositionRect.Add(m_listCopyPrefabs[i]);
        }

        // 위치 정리
        UIUtility.SetReposition(m_Scroll.content, listRepositionRect, 9.0f, false, 20.0f, 0.0f);
    }

    private void CreateItems(SecretSlotData slotData, CharCardData cardData, CharCardData selectCard)
    {
        UISecretCharCard newCharCard = Instantiate<UISecretCharCard>(m_CharCardPrefab);
        UIUtility.SetParent(newCharCard.transform, m_Scroll.content.transform);
        m_listCardDatas.Add(newCharCard);
        m_listCopyPrefabs.Add(newCharCard.gameObject.GetComponent<RectTransform>());

        newCharCard.SetCard(slotData, cardData, eCardType.CT_HAVE_CARD);
        newCharCard.SetSelectCardMark(selectCard);
    }

    //** Arrow 이미지 위치 세팅
    public void SetArrowPosition(float xPosition)
    {
        m_trsArrow.anchoredPosition = new Vector2(xPosition, m_trsArrow.anchoredPosition.y);
    }

    //** 선택된 카드 표현 
    private void OnSelectCard(CharCardData charCardData)
    {
        UISecretCharCard selectCard = m_listCardDatas.Find(item => item.m_CharCardData == charCardData);
        UISecretCharCard preCard = m_listCardDatas.Find(item => item.m_CharCardData == m_PreSelectCard);

        // 이전 카드는 선택 마크를 꺼줌
        if (preCard != null)
            preCard.SetSelectCardMark(charCardData);

        if (preCard == selectCard)
            return;

        selectCard.SetSelectCardMark(charCardData);

        m_PreSelectCard = charCardData;
    }
}
