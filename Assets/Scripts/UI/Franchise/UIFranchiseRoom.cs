using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIFranchiseRoom : MonoBehaviour 
{
    private const float F_GOODS_NORMAL_SIZE = 1.0f;
    private const float F_GOODS_MAX_SIZE_   = 1.5f;
    private const float F_CARD_NORMAL_SIZE  = 0.36f;
    private const float F_CARD_MAX_SIZE     = 0.5f;

    [HideInInspector]
    public UIFranchiseBuilding m_owner;

    //** 방 기본 관련
    [HideInInspector]
    public long         m_lSequence;

    [HideInInspector]
    public  int         m_nFloor;

    private bool        m_bOpened;

    //** 오픈 관련
    [HideInInspector]
    public  FranchiseOpenTerms m_OpenTrems;

    public  Text        m_OpenLevelText; 

    //** 보상 관련
    [HideInInspector]
    public  Goods_Type  m_eRevGoodsType;

    [HideInInspector]
    public  int         m_nRevGoodsCount;

    public  Text        m_RevRewardCountText;

    [HideInInspector]
    public  float       m_fCoolTime;

    private DateTime    m_LastOpenTime;
    public  GameObject  m_MaxImage;
    public  UISlider    m_CoolTimeSlider;
    private bool        m_bCompletTime;

    [HideInInspector]
    public  Transform   m_trsGoodsIcon;
    public  GameObject  m_CompletGoods;

    public GameObject   m_ZoomObject;

    //** 캐릭터 관련
    [HideInInspector]
    public  int         m_CharIndex;
    public  FranchiseCharAI m_CharAI;

    //** 이미지 관련
    private string      m_strRoomImageName;
    public  Image       m_Lock;
    public  Image       m_RoomImg;
    public  Image       m_RoomTableImg;
    public  Image       m_GoodsImg;
    public  Image       m_ZoomOutGoods;
    public  Sprite      m_LockImg;
    public  Sprite      m_OpenAbleImg;

    //** 잠김 오픈 관련
    public  GameObject  m_LockObject;
    public  GameObject  m_OpenObject;
    public  GameObject  m_OpenAbleEffect;
    public  GameObject  m_ZoomOutLockObject;
         
    //** 버튼 관련
    public  Button      m_InfoButton;
    public  Button      m_LockButton;
    public  Button      m_RevButton;

    private void Awake()
    {
        if (m_InfoButton != null)
            m_InfoButton.onClick.AddListener(OnClickInfoButton);

        if (m_LockButton != null)
            m_LockButton.onClick.AddListener(OnClickInfoButton);

        if (m_RevButton != null)
            m_RevButton.onClick.AddListener(OnClickRevButton);

        if (m_trsGoodsIcon == null)
            m_trsGoodsIcon = m_GoodsImg.GetComponent<Transform>();

        m_ZoomObject.SetActive(false);
    }


    //** 방 데이터 세팅.
    public void SetRoomData(UIFranchiseBuilding owner, FranchiseRoomData roomData)
    {
        m_owner = owner;

        m_lSequence         = roomData.m_lSequence;
        m_nFloor            = roomData.m_nFloor;
        m_bOpened           = roomData.m_bOpened;
        m_eRevGoodsType     = roomData.m_eRevGoodsType;
        m_nRevGoodsCount    = roomData.m_nRevGoodsCount;
        m_fCoolTime         = roomData.m_fCoolTime;
        m_CharIndex         = roomData.m_nCharacterIndex;
        m_strRoomImageName  = roomData.m_strRoomImage;
        m_LastOpenTime      = roomData.m_LastOpenTime;
        m_OpenTrems         = roomData.m_OpenTerms;

        SetRoomUI();
    }

    //** 방 UI 세팅
    private void SetRoomUI()
    {
        float goodsSize = GetGoodsSize(true);

        m_GoodsImg.sprite = TextureManager.GetGoodsTypeSprite(m_eRevGoodsType);
        m_GoodsImg.SetNativeSize();
        m_GoodsImg.GetComponent<Transform>().localScale = new Vector3(goodsSize, goodsSize, 1.0f);

        m_ZoomOutGoods.sprite = TextureManager.GetGoodsTypeSprite(m_eRevGoodsType);
        m_ZoomOutGoods.SetNativeSize();
        m_ZoomOutGoods.GetComponent<Transform>().localScale = new Vector3(goodsSize, goodsSize, 1.0f);

        m_RevRewardCountText.text = m_nRevGoodsCount.ToString();

        m_RoomImg.sprite = TextureManager.GetSprite(SpritePackingTag.Franchise, m_strRoomImageName);
        m_RoomImg.SetNativeSize();

        string tableImageName = string.Format("Building0{0}_Table", m_owner.m_nBuildingNumber);
        Sprite tableSprite = TextureManager.GetSprite(SpritePackingTag.Franchise, tableImageName);

        m_RoomTableImg.gameObject.SetActive(tableSprite);

        if (tableSprite != null)
        {
            m_RoomTableImg.sprite = tableSprite;
            m_RoomTableImg.SetNativeSize();
        }

        m_OpenLevelText.text = Languages.ToString(TEXT_UI.ACCOUNT_LEVEL_OPEN, m_OpenTrems.m_nNeedLevel);
        
        SetLock();
    }

    //** 잠김(오픈) 세팅
    public void SetLock()
    {
        m_OpenObject.SetActive(m_bOpened);
        m_LockObject.SetActive(!m_bOpened);
        m_ZoomOutLockObject.SetActive(!m_bOpened);
        m_ZoomOutGoods.gameObject.SetActive(m_bOpened);

        if(m_bOpened)
            m_CharAI.Setting(m_CharIndex, m_RoomImg.GetComponent<RectTransform>().sizeDelta.x);

        m_InfoButton.gameObject.SetActive(!m_bOpened);

        // Lock 이미지
        bool openable = IsOpenAble();
        m_Lock.gameObject.SetActive(!m_bOpened && m_OpenTrems.m_bNeedFloorOpen);
        m_Lock.sprite = openable ? m_OpenAbleImg : m_LockImg;
        m_OpenAbleEffect.SetActive(openable);
        m_Lock.SetNativeSize();
    }

    //** 줌 인 아웃에 따른 UI 엑티브 변경
    public void SetZoomInOutUI(bool zoom)
    {
        m_CharAI.gameObject.SetActive(zoom);
        m_OpenObject.SetActive(zoom);
        m_LockObject.SetActive(zoom);
        m_InfoButton.gameObject.SetActive(zoom);
        m_ZoomObject.SetActive(!zoom);

        //줌 일경우 켜줄것은 켜주고 하기 위해 다시 세팅
        if (zoom)
            SetLock();
    }

    //** 보상 시간 업데이트
    private void Update()
    {
        UpdateRemainTime();
    }

    private void UpdateRemainTime()
    {
        if (!m_bOpened)
            return;

        if (m_bCompletTime)
            return;

        bool endTime = Kernel.entry.franchise.SetRemainTime(m_LastOpenTime, m_fCoolTime, m_CoolTimeSlider);
        m_bCompletTime = endTime;
        UpdateGoodsUI(endTime);
    }

    //** 보상 시간에 따른 UI 세팅
    private void UpdateGoodsUI(bool endTime)
    {
        m_MaxImage.SetActive(endTime);
        m_CoolTimeSlider.m_Text.gameObject.SetActive(!endTime);

        float goodsSize = 1.0f;

        goodsSize = GetGoodsSize(endTime);

        if (m_trsGoodsIcon.localScale.y == goodsSize && m_trsGoodsIcon.localScale.x == goodsSize)
            return;

        m_trsGoodsIcon.localScale = new Vector3(goodsSize, goodsSize, 1.0f);

        if (m_CompletGoods.activeInHierarchy != endTime)
            m_CompletGoods.SetActive(endTime);

        if (m_RevRewardCountText.gameObject.activeInHierarchy != endTime)
            m_RevRewardCountText.gameObject.SetActive(endTime);

        m_RevButton.interactable = endTime;
    }

    //** Goods 사이즈 구하기
    public float GetGoodsSize(bool maxSize)
    {
        if ( //골드, 루비, 하트, 길드 포인트, 복수 포인트, 랭킹 포인트, 별, 트레이닝 포인트, 스마일 포인트, 친구 포인트
            m_eRevGoodsType == Goods_Type.Gold || m_eRevGoodsType == Goods_Type.Ruby || m_eRevGoodsType == Goods_Type.Heart
            || m_eRevGoodsType == Goods_Type.GuildPoint || m_eRevGoodsType == Goods_Type.RevengePoint || m_eRevGoodsType == Goods_Type.RankingPoint
            || m_eRevGoodsType == Goods_Type.StarPoint || m_eRevGoodsType == Goods_Type.TrainingPoint || m_eRevGoodsType == Goods_Type.SmilePoint || m_eRevGoodsType == Goods_Type.FriendPoint
            )
        {
            return maxSize ? F_GOODS_MAX_SIZE_ : F_GOODS_NORMAL_SIZE;
        }
        else // 나머지 카드형태
        {
            return maxSize ? F_CARD_MAX_SIZE : F_CARD_NORMAL_SIZE;
        }
    }

    //** 방 사이즈 반환
    public Vector2 GetRoomSize()
    {
        RectTransform trsRoom = m_RoomImg.gameObject.GetComponent<RectTransform>();

        if (trsRoom != null)
            return trsRoom.sizeDelta;

        return Vector2.zero;
    }

    //** 보상 아이콘 월드 포지션
    public Vector3 GetGoodsIconWorldPosition()
    {
        if (m_trsGoodsIcon == null)
            return Vector3.zero;

        return m_trsGoodsIcon.position;
    }

    //** 오픈 가능한가?
    public bool IsOpenAble()
    {
        if (m_bOpened)
            return true;

        // 밑에 방이 열렸는지 체크
        if(!m_OpenTrems.m_bNeedFloorOpen)
            return false;

        // 레벨 체크
        if (m_OpenTrems.m_nNeedLevel > Kernel.entry.account.level)
            return false;

        // 재화 수량 체크
        for (int i = 0; i < m_OpenTrems.m_listNeedGoods.Count; i++)
        {
            FranchiseOpenGoods openTremsGoodsData = m_OpenTrems.m_listNeedGoods[i];

            if (openTremsGoodsData == null)
                continue;

            if(openTremsGoodsData.m_nNeedGoodsCount > Kernel.entry.account.GetValue(openTremsGoodsData.m_eNeedGoodsType))
                return false;
        }

        return true;
    }

    #region 버튼

    //** Help 버튼
    private void OnClickInfoButton()
    {
        UIFranchiseInfo franchiseInfo = Kernel.uiManager.Get<UIFranchiseInfo>(UI.FranchiseInfo, true, false);
        franchiseInfo.SetData(this);
        UIManager.Instance.Open(UI.FranchiseInfo);
    }

    //** 보상 받기 버튼
    private void OnClickRevButton()
    {
        Kernel.entry.franchise.SendRoomRewardPacket(m_owner.m_nBuildingNumber ,(byte)m_nFloor, m_lSequence);
    }

    #endregion

    #region 갱신

    //** 보상 받은 후 갱신
    public void RefleshRevReward(DateTime lastUpdateTime)
    {
        m_LastOpenTime = lastUpdateTime;
        m_bCompletTime = false;
    }

    #endregion
}
