using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIFranchise : UIObject
{
    private const int   N_MUTI_PLUSE_VALUE      = 1;        // 16:9로 배경 이미지 맞출때 곱셈에서 추가 공간 값.
    private const float F_SMILE_ORIN_SIZE       = 0.83f;    // 스마일 포인트 기본 사이즈.
    private const float F_SMILE_SIZE_UP         = 1.0f;     // 완료되었을때, 스마일 포인트 사이즈 배수
    

    //** 가장 높은 빌딩의 높이
    private float m_fBuildingMostHeight;
    public  float BuildingMostHeight
    {
        get { return m_fBuildingMostHeight; }
        set 
        {
            if(m_fBuildingMostHeight <= value)
                m_fBuildingMostHeight = value; 
        }
    }

    //** 가장 끝 빌딩의 x 위치
    private float m_fBuildingEndPosition;
    public  float BuildingEndPosition
    {
        get { return m_fBuildingEndPosition; }
        set 
        {
            if(m_fBuildingEndPosition <= value)
                m_fBuildingEndPosition = value; 
        }
    }

    //** 빌딩 리스트
    public  List<UIFranchiseBuilding>   m_listBuildings;

    //** 계정 관련
    private TextLevelMaxEffect          m_LevelMaxEffect;
    public  UIMiniCharCard              m_MiniCharCard;
    public  Text                        m_NickNameText;
    public  Text                        m_LevelText;

    //** 줌 아웃, 이미지 관련
    private UIFranchiseZoomInOut        m_ZoomInOut;
    public  GameObject                  m_BackGroundImage;
    public  RectTransform               m_rtrsGround;
    public  Image                       m_ZoomInOutButtonImage;
    public  Sprite                      m_ZoomInButtonSprite;
    public  Sprite                      m_ZoomOutButtonSprite;

    //** 보상 애니 관련
    public  GoodsRewardAnimationData    m_animGoodsData;

    //** 스마일맨
    private DateTime                    m_SmilePointLastOpenTime;
    private float                       m_SmilePointTermSec;
    public  Transform                   m_trsSmileMan;
    public  UISlider                    m_SmilePointSlider;
    public  GameObject                  m_SmileManCompletObject;
    public  GameObject                  m_MaxImage;
    private bool                        m_bCompletTime;

    //** 구름 관련
    public UIFranchiseCloud[]           m_moveClouds;

    //** 버튼 관련
    public  Button                      m_ZoomInOutButton;
    public  Button                      m_RevSmilePoint;

    protected override void Awake()
    {
        base.Awake();

        SetInit();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        Kernel.entry.franchise.onRevSmilePointCallBack  += RevSmilePoint;
        Kernel.entry.franchise.onRevRoomOpen            += CheckOpenableRooms;
        Kernel.entry.franchise.onRevRoomReward          += RevRewardCallBack;

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 900)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Kernel.entry.franchise.onRevSmilePointCallBack  -= RevSmilePoint;
        Kernel.entry.franchise.onRevRoomOpen            -= CheckOpenableRooms;
        Kernel.entry.franchise.onRevRoomReward          -= RevRewardCallBack;
    }
    //** Init
    private void SetInit()
    {
        // 줌 인 아웃
        m_ZoomInOut = GetComponent<UIFranchiseZoomInOut>();

        if (m_ZoomInOut == null)
        {
            Debug.LogError("[UIFranchise] Awake : m_ZoomInOut(UIFranchiseZoomInOut) is Null");
            return;
        }

        m_ZoomInOutButton.onClick.AddListener(OnClickZoomInOut);
        m_ZoomInOut.SetTransForm(m_BackGroundImage);

        // 계정
        m_MiniCharCard.SetCardInfo(Kernel.entry.account.leaderCardIndex, Kernel.entry.account.level, 0);
        m_NickNameText.text = Kernel.entry.account.name;
        m_LevelText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), Kernel.entry.account.level);

        if (m_LevelText != null)
            m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit);

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.Value = Kernel.entry.account.level;

        if (m_MiniCharCard.onClicked == null)
            m_MiniCharCard.onClicked += OnMiniCharCardClicked;

        //스마일 포인트
        m_SmilePointLastOpenTime = Kernel.entry.franchise.SmilePointLastGetTime;
        m_SmilePointTermSec = Kernel.entry.franchise.SmilePointTotalTime;
        m_RevSmilePoint.onClick.AddListener(OnClickRevSmilePoint);

        // 건물
        CreateBuildings();
    }

    //** 줌 인 아웃에 따른 UI 엑티브 변경
    public void SetZoomInOutUI(bool zoom)
    {
        for (int i = 0; i < m_listBuildings.Count; i++)
        {
            UIFranchiseBuilding building = m_listBuildings[i];

            building.SetZoomInOutUI(zoom);
        }
    }

    //** 업데이트
    protected override void Update()
    {
        base.Update();

        UpdateSmileManRemainTime();
    }

    //** 스마일 포인트 시간 업데이트
    private void UpdateSmileManRemainTime()
    {
        if (m_bCompletTime)
            return;

        bool end = Kernel.entry.franchise.SetRemainTime(m_SmilePointLastOpenTime, m_SmilePointTermSec, m_SmilePointSlider);
        m_bCompletTime = end;
        UpdateSmilUI(end);
    }

    //** 스마일 포인트 시간에 따른 UI 세팅
    private void UpdateSmilUI(bool endTime)
    {
        m_MaxImage.SetActive(endTime);
        m_SmilePointSlider.m_Text.gameObject.SetActive(!endTime);

        float smileSize = endTime ? F_SMILE_SIZE_UP : F_SMILE_ORIN_SIZE;

        if(m_SmileManCompletObject.activeInHierarchy != endTime)
            m_SmileManCompletObject.SetActive(endTime);

        if(m_RevSmilePoint.enabled != endTime)
            m_RevSmilePoint.enabled = endTime;

        if (m_trsSmileMan.localScale.y == smileSize && m_trsSmileMan.localScale.x == smileSize)
            return;

        m_trsSmileMan.localScale = new Vector3(smileSize, smileSize, 1.0f);
    }

    #region 빌딩

    //** 빌딩 세우기.
    private void CreateBuildings()
    {
        var buildingDatas = Kernel.entry.franchise.FindAllBuildingData();

        if (buildingDatas == null)
            return;

        foreach (KeyValuePair<int, List<FranchiseRoomData>> roomDatas in buildingDatas)
        {
            UIFranchiseBuilding fineBuilding = m_listBuildings.Find(item => item.m_nBuildingNumber == roomDatas.Key);

            if (fineBuilding == null)
            {
                Debug.LogError(string.Format("[UIFranchise] CreateBuildings : fineBuilding(UIFranchiseBuilding Number : {0} ) is Null or Zero", roomDatas.Key));
                continue;
            }

            fineBuilding.CreateRooms(roomDatas.Value);

            // 가장 높은 빌딩 위치
            BuildingMostHeight = fineBuilding.BuildingHeight;
        }

        // 가장 끝의 빌딩 위치 구하기
        int lastBuildingNum = m_listBuildings.Count - 1; //5번째 건물이 나오면 -1로 변경 안나오면 -2
        UIFranchiseBuilding lastBuilding = lastBuildingNum > 0 ? m_listBuildings[lastBuildingNum] : null;

        if (lastBuilding != null)
        {
            RectTransform lastBuildingPosition = lastBuilding.GetComponent<RectTransform>();
            BuildingEndPosition = lastBuildingPosition.anchoredPosition.x + lastBuilding.BuildingWidth;
        }
        else
            BuildingEndPosition = 1280.0f; //기본

        SetBackGroundSize();
        SetMoveClouds();
    }

    #endregion

    #region 화면 배경 사이즈 조절

    //** 배경 이미지 사이즈 조절
    private void SetBackGroundSize()
    {
        RectTransform rtrsBackGroundImage = m_BackGroundImage.GetComponent<RectTransform>();

        if (rtrsBackGroundImage == null)
            return;

        int screenSizeX = Screen.width;
        int screenSizeY = Screen.height;

        // 화면 비율
        Vector2 screenRatio = GetScreenRatio(screenSizeX, screenSizeY);

        float width = 0.0f;
        float height = 0.0f;

        // 해상 사이즈 비율로 맞추기
        int XValue = (int)(BuildingEndPosition / screenRatio.x);
        int YValue = (int)(BuildingMostHeight / screenRatio.y);

        int mutiValue = XValue > YValue ? XValue : YValue;
        mutiValue += N_MUTI_PLUSE_VALUE;

        width = mutiValue * screenRatio.x;
        height = mutiValue * screenRatio.y;

        rtrsBackGroundImage.sizeDelta = new Vector2(width, height);
        m_rtrsGround.sizeDelta = new Vector2(width, m_rtrsGround.sizeDelta.y);

        // 배수값 저장
        m_ZoomInOut.SaveZoomMutiple(screenSizeX, screenSizeY, width, height);
    }

    //** 화면 해상도 반환
    private Vector2 GetScreenRatio(int screenSizeX, int screenSizeY)
    {
        int maxValue = 0;
        int minValue = 0;
        int temp = 0;
        int endValue = 0;

        maxValue = screenSizeX > screenSizeY ? screenSizeX : screenSizeY;
        minValue = maxValue == screenSizeX ? screenSizeY : screenSizeX;

        while (maxValue % minValue != 0)
        {
            temp = maxValue % minValue;
            maxValue = minValue;
            minValue = temp;
        }

        endValue = minValue;

        int resultSizeX = screenSizeX / endValue;
        int resultSizeY = screenSizeY / endValue;

        return new Vector2(resultSizeX, resultSizeY);
    }

    #endregion

    //** 구름들 세팅
    private void SetMoveClouds()
    {
        float endXPos = m_rtrsGround.sizeDelta.x;

        for (int i = 0; i < m_moveClouds.Length; i++)
        {
            UIFranchiseCloud moveCloud = m_moveClouds[i];

            if (moveCloud == null)
                continue;

            moveCloud.SetBasePos(endXPos);
        }
    }

    //** 빌딩 번호에 따른 빌딩 반환
    private UIFranchiseBuilding GetBuilding(int buildingNum)
    {
        if (m_listBuildings == null)
            return null;

        return m_listBuildings.Find(item => item.m_nBuildingNumber == buildingNum);
    }

    #region 버튼

    //** 리더 이미지를 클릭시
    private void OnMiniCharCardClicked()
    {
        if (Kernel.entry != null)
            Kernel.entry.ranking.REQ_PACKET_CG_READ_DETAIL_USER_INFO_SYN(Kernel.entry.account.userNo);
    }

    //** 스마일포인트 획득 버튼
    private void OnClickRevSmilePoint()
    {
        Kernel.entry.franchise.REQ_PACKET_CG_GAME_RECEIVE_SMILE_POINT_SYN();
    }

    //** 줌 인 아웃 버튼
    private void OnClickZoomInOut()
    {
        bool zoom = m_ZoomInOut.OnClickZoomInOut();
        SetZoomInOutUI(zoom);
        m_ZoomInOutButtonImage.sprite = zoom ? m_ZoomOutButtonSprite : m_ZoomInButtonSprite;
    }

    #endregion

    //** 보상 받은 후 콜백
    private void RevRewardCallBack(int buildingNum, int roomIdex, int lastUpdateTime)
    {
        UIFranchiseBuilding building = GetBuilding(buildingNum);

        if (building == null)
        {
            Debug.LogError(string.Format("[UIFranchise] RevRewardCallBack : building(UIFranchiseBuilding Number : {0} ) is Null or Zero", buildingNum));
            return;
        }

        UIFranchiseRoom room = building.GetRoom(roomIdex);

        if (room == null)
        {
            Debug.LogError(string.Format("[UIFranchise] RevRewardCallBack : room(UIFranchiseRoom Number : {0} ) is Null or Zero", roomIdex));
            return;
        }

        room.RefleshRevReward(TimeUtility.ToDateTime(lastUpdateTime));
        CheckOpenableRooms();

        Vector3 goodsWorldPosition = room.GetGoodsIconWorldPosition();
        Goods_Type goodsType = room.m_eRevGoodsType;
        float goodsSize = room.GetGoodsSize(false);

        // 보상 애니 시작
        UIHUD hud = Kernel.uiManager.Get<UIHUD>(UI.HUD, true, false);
        if (hud != null)
        {
            List<GoodsRewardAnimationData> newAnimlist = new List<GoodsRewardAnimationData>();
            m_animGoodsData.m_eRewardGoodsType = goodsType;
            m_animGoodsData.m_bUseBaseEndPosition = true;
            newAnimlist.Add(m_animGoodsData);
            hud.UseGoodsRewardAnimation(goodsWorldPosition, newAnimlist); //작업
        }
    }

    // 보상을 받고 오픈 가능한 방이 있는지 체크.
    private void CheckOpenableRooms()
    {
        for (int i = 0; i < m_listBuildings.Count; i++)
        {
            UIFranchiseBuilding building = m_listBuildings[i];

            building.CheckOpenableRooms();
        }
    }

    //** 스마일 포인트 받은 후
    private void RevSmilePoint()
    {
        CheckOpenableRooms();
        m_SmilePointLastOpenTime = Kernel.entry.franchise.SmilePointLastGetTime;

        // 보상 애니 시작
        UIHUD hud = Kernel.uiManager.Get<UIHUD>(UI.HUD, true, false);
        if (hud != null)
        {
            List<GoodsRewardAnimationData> newAnimlist = new List<GoodsRewardAnimationData>();
            m_animGoodsData.m_eRewardGoodsType = Goods_Type.SmilePoint;
            m_animGoodsData.m_bUseBaseEndPosition = true;
            newAnimlist.Add(m_animGoodsData);
            hud.UseGoodsRewardAnimation(m_trsSmileMan.position, newAnimlist); //작업
        }

        m_bCompletTime = false;
    }
}
