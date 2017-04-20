using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Packet;

public class SecretBoxData
{
    public int                  m_nSlotType;

    public string               m_strBoxIconName;
    public List<CharCardData>   m_listLegendCardData        = new List<CharCardData>();
    public Dictionary<int, SecretSlotData> m_dicSlotData    = new Dictionary<int, SecretSlotData>();
}

public class SecretSlotData
{
    public int                  m_nSlotIndex;

    public Grade_Type           m_eGradeType;
    public ClassType            m_eClassType;
    public int                  m_nNeedCount;
    public List<CharCardData>   m_listCharCardData  = new List<CharCardData>();
}

public class CharCardData
{
    public int              m_nHaveCount;

    public DB_Card.Schema   m_CardData;
    public CCardInfo        m_CardInfo;
}

public class SecretBusiness : Node 
{
    //** Reward Box
    public delegate void OnOpenSecretBoxCallback(int cardIndex);
    public OnOpenSecretBoxCallback onOpenSecretBoxCallback;

    //** CardClick CallBack
    public delegate void OnClickSlotCardCallBack(int slotIndex);
    public OnClickSlotCardCallBack onClickSlotCardCallBack = null;

    public delegate void OnClickHaveCardCallBack(CharCardData charCardData);
    public OnClickHaveCardCallBack onClickHaveCardCallBack = null;

    public delegate void OnClickCardInfoCallBack(CharCardData charCardData);
    public OnClickCardInfoCallBack onClickCardInfoCallBack = null;

    //** 박스 전체 데이터
    private Dictionary<int, SecretBoxData> m_dicBoxData         = new Dictionary<int, SecretBoxData>();
    
    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_CARD_SECRET_EXCHANGE_ACK>(REV_PACKET_CG_CARD_SECRET_EXCHANGE_ACK);

        return base.OnCreate();
    }
    
    //** Table 데이터를 Dictionary로 정리
    public void SetParseTableData()
    {
        // 이미 정리되었으면 하지않는다.
        if (m_dicBoxData != null && m_dicBoxData.Count > 0)
            return;

        List<DB_SecretExchange.Schema>      boxTable    = DB_SecretExchange.instance.schemaList;
        List<DB_SecretExchangeSlot.Schema>  slotTable   = DB_SecretExchangeSlot.instance.schemaList;

        if (boxTable == null || slotTable == null)
            return;

        // SecretExchange 정렬.
        for (int i = 0; i < boxTable.Count; i++)
        {
            SecretBoxData newBoxData = new SecretBoxData();
            newBoxData.m_nSlotType = boxTable[i].SlotType_SecretExchangeSlot;
            newBoxData.m_strBoxIconName = boxTable[i].Icon_Name;
            newBoxData.m_listLegendCardData = GetLegendCardList(boxTable[i].SlotType_SecretExchangeSlot);

            m_dicBoxData.Add(boxTable[i].SlotType_SecretExchangeSlot, newBoxData);
        }

        // SecretExchangeSlot 정렬.
        for (int i = 0; i < slotTable.Count; i++)
        {
            if (!m_dicBoxData.ContainsKey(slotTable[i].SlotType))
                Debug.LogError(string.Format("SecretBusiness : SetParseTableData - slotTable의 SlotType({0})에 해당하는 DB_SecretExchange이 없음.", slotTable[i].SlotType));

            DB_SecretExchangeSlot.Schema slotData = slotTable[i];

            SecretSlotData newSlotData          = new SecretSlotData();
            newSlotData.m_nSlotIndex            = slotData.index;
            newSlotData.m_eGradeType            = slotData.Grade_Type;
            newSlotData.m_eClassType            = slotData.ClassType;
            newSlotData.m_nNeedCount            = slotData.Need_Qty_Slot;
            //newSlotData.m_listCharCardData      = GetCharCardList(slotData.Need_Qty_Slot, slotData.Grade_Type, slotData.ClassType);

            m_dicBoxData[slotTable[i].SlotType].m_dicSlotData.Add(slotData.index, newSlotData);
        }
    }

    //** 박스 안의 전설 카드 정보 리스트 갖기
    public List<CharCardData> GetLegendCardList(int slotType)
    {
        List<DB_LegendBox.Schema> legendCardList = DB_LegendBox.instance.schemaList;

        if (legendCardList == null)
            return null;

        List<CCardInfo> cardInfoList = Kernel.entry.character.cardInfoList;

        if (cardInfoList == null)
            return null;

        List<CharCardData> listLegendCard = new List<CharCardData>();

        for (int i = 0; i < legendCardList.Count; i++)
        {
            if (legendCardList[i].Get_Type != slotType)
                continue;

            DB_Card.Schema cardData = DB_Card.Query(DB_Card.Field.Index, legendCardList[i].Card_Index);

            if (cardData == null)
                continue;

            CharCardData newLegendCard = new CharCardData();

            newLegendCard.m_CardData = cardData;
            newLegendCard.m_CardInfo = cardInfoList.Find(item => item.m_iCardIndex == legendCardList[i].Card_Index);
            newLegendCard.m_nHaveCount = 1;

            listLegendCard.Add(newLegendCard);
        }

        return listLegendCard;
    }

    //** 조건 만족하는 보유 캐릭터 리스트 반환
    public List<CharCardData> GetCharCardList(int needCount, Grade_Type gradeType, ClassType classType)
    {
        List<CCardInfo> haveCharCard = Kernel.entry.character.cardInfoList;

        if(haveCharCard == null)
            return null;

        List<CharCardData> charCardList = new List<CharCardData>();

        if (charCardList == null && charCardList.Count > 0)
            charCardList.Clear();

        
        // 조건 체크
        for (int i = 0; i < haveCharCard.Count; i++)
        {
            // 갯수 체크
            CSoulInfo soulInfo = Kernel.entry.character.FindSoulInfo(haveCharCard[i].m_iCardIndex);

            // ref. PUC-883 // 1000번대 부터 보스들이므로 soulInfo가 Null이 됨.
            if (soulInfo == null)
                continue;

            int haveCount = soulInfo.m_iSoulCount;
            // ref. PUC-883
            //if (haveCount < needCount)
            //    continue;

            DB_Card.Schema cardData = DB_Card.Query(DB_Card.Field.Index, haveCharCard[i].m_iCardIndex);

            // 등급 체크
            if (cardData.Grade_Type != gradeType)
                continue;

            // 직업 체크 (None이면 모두 포함)
            if (classType != ClassType.None && cardData.ClassType != classType)
                continue;


            CharCardData newCardData    = new CharCardData();
            newCardData.m_nHaveCount    = haveCount;
            newCardData.m_CardData      = cardData;
            newCardData.m_CardInfo      = haveCharCard[i];

            charCardList.Add(newCardData);
        }

        // 정렬
        charCardList.Sort(delegate(CharCardData x, CharCardData y) { return x.m_CardInfo.m_byLevel.CompareTo(y.m_CardInfo.m_byLevel); });       //레벨 : 낮음 -> 높음
        charCardList.Sort(delegate(CharCardData x, CharCardData y) { return x.m_CardInfo.m_iCardIndex.CompareTo(y.m_CardInfo.m_iCardIndex); }); //카드 인덱스 : 낮음 -> 높음
        charCardList.Sort(delegate(CharCardData x, CharCardData y) { return -x.m_nHaveCount.CompareTo(y.m_nHaveCount); });                      //소유갯수 : 높음 -> 낮음

        return charCardList;
    }

    //** 모든 상자 정보 반환
    public List<SecretBoxData> GetAllBoxDatas()
    {
        List<SecretBoxData> listBoxDatas = new List<SecretBoxData>();

        foreach(SecretBoxData boxData in m_dicBoxData.Values)
        {
            if(boxData == null)
                continue;

            listBoxDatas.Add(boxData);
        }

        return listBoxDatas;
    }

    //** SlotType에 맞는 SecretBoxData 반환
    public SecretBoxData GetBoxData(int slotType)
    {
        if (m_dicBoxData == null)
            return null;

        return m_dicBoxData[slotType];
    }

    //** 선택된 슬롯의 데이터 반환
    public SecretSlotData GetSlotData(int slotType, int selectSlotIndex)
    {
        return GetBoxData(slotType).m_dicSlotData[selectSlotIndex];
    }

    #region REQ

    //** SecretBox 열기 요청
    public void REQ_PACKET_CG_CARD_SECRET_EXCHANGE_SYN(byte slotType, List<long> listSelectCharSeq)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_CARD_SECRET_EXCHANGE_SYN()
        {
            m_bySlotType = slotType,
            m_SidList = listSelectCharSeq
        });
    }

    #endregion

    #region REV

    //** SecretBox Reward 정보 받기
    private void REV_PACKET_CG_CARD_SECRET_EXCHANGE_ACK(PACKET_CG_CARD_SECRET_EXCHANGE_ACK packet)
    {
        entry.character.UpdateCardInfo(packet.m_CardInfo);

        for (int i = 0; i < packet.m_SoulList.Count; i++)
            entry.character.UpdateSoulInfo(packet.m_SoulList[i]);

        if (onOpenSecretBoxCallback != null)
            onOpenSecretBoxCallback(packet.m_iResultCardIndex);
    }

    #endregion
}
