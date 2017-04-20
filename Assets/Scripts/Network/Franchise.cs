using UnityEngine;
using Common.Packet;
using System.Collections;
using System.Collections.Generic;
using System;

//** 가맹점 방 기본 데이터
public class FranchiseRoomData
{
    // 기본
    public int          m_nFloor;
    public long         m_lSequence;

    public bool         m_bOpened;

    // 보상
    public Goods_Type   m_eRevGoodsType;
    public int          m_nRevGoodsCount;
    public float        m_fCoolTime;
    public DateTime     m_LastOpenTime;

    // 이미지
    public int          m_nCharacterIndex;
    public string       m_strRoomImage;

    // 조건
    public FranchiseOpenTerms m_OpenTerms;
}

//** 오픈 조건
public class FranchiseOpenTerms
{
    public int          m_nTermsType;

    public int          m_nNeedLevel;
    public int          m_nNeedOpenedFloor;
    public bool         m_bNeedFloorOpen; 

    // 사용 재화
    public List<FranchiseOpenGoods> m_listNeedGoods;
}

//** 오픈하는데 사용되는 재화
public class FranchiseOpenGoods
{
    public Goods_Type   m_eNeedGoodsType;
    public int          m_nNeedGoodsCount;
}

public class Franchise : Node
{
    public bool m_bSettingComplet = false;

    //** 모든 방 정보 <빌딩번호, 룸 데이터>
    private Dictionary<int, List<FranchiseRoomData>> m_dicFranciseRoomDatas;
    private Dictionary<int, List<TEXT_UI>> m_dicSpeakDatas;

    //** 스마일 포인트
    private DateTime m_fSmilePointLastGetTime; //스마일 포인트 받은 마지막 시간
    public DateTime SmilePointLastGetTime
    {
        get { return m_fSmilePointLastGetTime; }
        set { m_fSmilePointLastGetTime = value; }
        
    }

    //** 스마일 포인트 전체 시간
    private float m_fSmilePointTotalTime;
    public float SmilePointTotalTime
    {
        get { return m_fSmilePointTotalTime; }
        set { m_fSmilePointTotalTime = value; }
    }

    //** 스마일 포인트 남은 시간
    private TimeSpan m_SmilePointRemainTime;
    public TimeSpan SmilePointRemainTime
    {
        get { return m_SmilePointRemainTime; }
        set { m_SmilePointRemainTime = value; }
    }

    //** 스마일 포인트 받은 후 콜백
    public delegate void OnRevSmilePointCallBack();
    public OnRevSmilePointCallBack onRevSmilePointCallBack;

    //** 방 오픈 후 콜백
    public delegate void OnRevRoomOpen();
    public OnRevRoomOpen onRevRoomOpen;

    //** 방 보상 후 콜백 ( 보상 받은 방 갱신, 오픈 가능한 방이 있는지 체크 )
    public delegate void OnRevRoomReward(int buildingNum, int roomIndex, int lastUpdateTime);
    public OnRevRoomReward onRevRoomReward;

    //** 보상 받을 수 있는 것이 몇개인지 체크
    public delegate void OnChangedOpenableFranchiseRewardCount(int rewardCount);
    public OnChangedOpenableFranchiseRewardCount onChangedOpenableFranchiseRewardCount;

    //** 보상 받을 수 있는 것이 있는지 체크
    public delegate void OnActiveFranchiseRewardCallback(bool rewardExist);
    public OnActiveFranchiseRewardCallback onActiveFranchiseRewardCallback;

    int m_nRewardCompletCount;

    public int rewardCompletCount
    {
        get
        {
            return m_nRewardCompletCount;
        }

        private set
        {
            if (m_nRewardCompletCount != value)
            {
                m_nRewardCompletCount = value;

                // 숏컷 new 아이콘용6
                if (onChangedOpenableFranchiseRewardCount != null)
                    onChangedOpenableFranchiseRewardCount(m_nRewardCompletCount);
            }
        }
    }

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_FRANCHISE_BUILDING_INFO_ACK>(REV_PACKET_CG_READ_FRANCHISE_BUILDING_INFO_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_ACK>(REV_PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_ACK>(REV_PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_RECEIVE_SMILE_POINT_ACK>(REV_PACKET_CG_GAME_RECEIVE_SMILE_POINT_ACK);

        return base.OnCreate();
    }

    #region Setting Datas

    //** 템플릿 데이터를 가지고 세팅
    public void SetBuildingData()
    {
        if (m_dicFranciseRoomDatas == null)
            m_dicFranciseRoomDatas = new Dictionary<int, List<FranchiseRoomData>>();

        if (m_dicFranciseRoomDatas.Count > 0)
            return;

        if (m_dicSpeakDatas == null)
            m_dicSpeakDatas = new Dictionary<int, List<TEXT_UI>>();

        

        // 건물 데이터 세팅
        List<DB_FranchiseStructure.Schema> listFranchiseTable = DB_FranchiseStructure.instance.schemaList;

        if (listFranchiseTable == null)
        {
            Debug.LogError("[Franchise] SetData : DB_FranchiseStructure is Null");
            return;
        }
        
        // 랜덤 캐릭터 인덱스 리스트 가져오기
        List<int> listRandomCharIndex = GetRandomCharacterIndex(listFranchiseTable.Count);

        if (listRandomCharIndex == null || listRandomCharIndex.Count <= 0)
        {
            Debug.LogError("[Franchise] SetData : listRandomCharIndex(List<int>) is Null or Empty");
            return;
        }

        for (int i = 0; i < listFranchiseTable.Count; i++)
        {
            DB_FranchiseStructure.Schema tableData = listFranchiseTable[i];

            //*Temp : 대문과 지붕 테이블에서 데이터를 없앰.
            if (tableData.isRoof || tableData.FloorNum == 0)
                continue;

            if (!m_dicFranciseRoomDatas.ContainsKey(tableData.BuildingNum))
                m_dicFranciseRoomDatas.Add(tableData.BuildingNum, new List<FranchiseRoomData>());

            List<FranchiseRoomData> newlistRoomData = m_dicFranciseRoomDatas[tableData.BuildingNum];
            FranchiseRoomData newRoomData = new FranchiseRoomData();
            newlistRoomData.Add(SetRoomData(newRoomData, tableData, listRandomCharIndex[i]));
        }

        // 오픈 조건 데이터 세팅
        List<DB_FranchiseExpansion.Schema> listTermsTable = DB_FranchiseExpansion.instance.schemaList;

        if (listTermsTable == null)
        {
            Debug.LogError("[Franchise] SetData : DB_FranchiseExpansion is Null");
            return;
        }

        for (int i = 0; i < listTermsTable.Count; i++)
        {
            DB_FranchiseExpansion.Schema TermsTableData = listTermsTable[i];

            if (!m_dicFranciseRoomDatas.ContainsKey(TermsTableData.ReqBuildingNum))
                continue;
             
            FranchiseRoomData roomData = m_dicFranciseRoomDatas[TermsTableData.ReqBuildingNum].Find(item => item.m_OpenTerms.m_nTermsType == TermsTableData.Expansion_Type);

            if (roomData == null)
                continue;

            SetOpenTermsData(roomData.m_OpenTerms, TermsTableData);
        }

        // 빌딩 정보 이야기 세팅
        if (m_dicSpeakDatas.Count > 0)
            m_dicSpeakDatas.Clear();

        List<DB_FranchiseExpansionInfo.Schema> listSpeakTable = DB_FranchiseExpansionInfo.instance.schemaList;

        if (listSpeakTable == null)
        {
            Debug.LogError("[Franchise] SetData : DB_FranchiseExpansionInfo is Null");
            return;
        }

        for (int i = 0; i < listSpeakTable.Count; i++)
        {
            DB_FranchiseExpansionInfo.Schema SpeakTableData = listSpeakTable[i];

            if (!m_dicSpeakDatas.ContainsKey(SpeakTableData.BuildingNum))
            {
                List<TEXT_UI> listSpeak = new List<TEXT_UI>();
                listSpeak.Add(SpeakTableData.TEXT_UI);
                m_dicSpeakDatas.Add(SpeakTableData.BuildingNum, listSpeak);
            }

            m_dicSpeakDatas[SpeakTableData.BuildingNum].Add(SpeakTableData.TEXT_UI);
        }

        // 스마일 포인트
        SmilePointTotalTime = Kernel.entry.data.GetValue<float>(Const_IndexID.Const_Franchise_Smilepoint_Refresh_Cycle_Sec);
    }

    //** 패킷 데이터 적용
    public void SettingPacket(List<CFranchiseBuilding> openRoomData, int smilePointLastUpdateTime)
    {
        // 열린 방만 세팅 (보상 마지막 받은 시간, 오픈)
        for(int i = 0; i < openRoomData.Count; i++)
        {
            CFranchiseBuilding roomData = openRoomData[i];

            if (!m_dicFranciseRoomDatas.ContainsKey(roomData.m_iBuildingNumber))
            {
                Debug.LogError(string.Format("[Franchise] SettingPacket : CFranchiseBuilding (BuildingNumber : {0}) is Null", roomData.m_iBuildingNumber));
                continue;
            }

            FranchiseRoomData originRoomData = m_dicFranciseRoomDatas[roomData.m_iBuildingNumber].Find(item => item.m_nFloor == roomData.m_byFloor);

            if (originRoomData == null)
            {
                Debug.LogError(string.Format("[Franchise] SettingPacket : FranchiseRoomData (RoomNumber : {0}) is Null", roomData.m_byFloor));
                continue;
            }

            originRoomData.m_lSequence = roomData.m_Sequence;
            originRoomData.m_LastOpenTime = TimeUtility.ToDateTime(roomData.m_iReceivedTime);
            originRoomData.m_bOpened = true;
        }

        // 스마일 포인트 마지막 받은 시간
        SmilePointLastGetTime = TimeUtility.ToDateTime(smilePointLastUpdateTime);

        // 이전 층 오픈 됬는지 세팅
        SetNeedFloorOpen();
    }

    //** 빌딩 및 방 정보 세팅
    public FranchiseRoomData SetRoomData(FranchiseRoomData roomData, DB_FranchiseStructure.Schema tableData, int charIndex)
    {
        roomData.m_nFloor           = tableData.FloorNum;
        roomData.m_eRevGoodsType    = tableData.Goods_Type;
        roomData.m_nRevGoodsCount   = tableData.RewardCount;
        roomData.m_fCoolTime        = tableData.CoolTime_Sec;
        roomData.m_nCharacterIndex  = charIndex;                //랜덤 캐릭터 인덱스
        //roomData.m_nCharacterIndex  = tableData.CardId;       //템플릿에서 정한 인덱스
        roomData.m_strRoomImage     = tableData.ResourceName;
        
        roomData.m_OpenTerms        = new FranchiseOpenTerms();
        roomData.m_OpenTerms.m_nTermsType = tableData.Expansion_Type;

        return roomData;
    }

    //** 오픈 조건 정보 세팅
    public void SetOpenTermsData(FranchiseOpenTerms openTermsData, DB_FranchiseExpansion.Schema termsTableData)
    {
        // 조건 세팅
        openTermsData.m_nNeedLevel          = termsTableData.ReqAccountLevel;
        openTermsData.m_nNeedOpenedFloor    = termsTableData.ReqFloorNum;

        // 필요 재화 세팅
        if (openTermsData.m_listNeedGoods == null)
            openTermsData.m_listNeedGoods = new List<FranchiseOpenGoods>();

        FranchiseOpenGoods newNeedGoods = new FranchiseOpenGoods();
        newNeedGoods.m_eNeedGoodsType   = termsTableData.Goods_Type;
        newNeedGoods.m_nNeedGoodsCount  = termsTableData.ReqCount;

        openTermsData.m_listNeedGoods.Add(newNeedGoods);
    }
    
    //** 이전 오픈되어야 하는 층이 오픈 되었는지 세팅
    public void SetNeedFloorOpen()
    {
        foreach (var listRooms in m_dicFranciseRoomDatas.Values)
        {
            for (int i = 0; i < listRooms.Count; i++)
            {
                FranchiseRoomData roomData = listRooms[i];

                if (roomData == null)
                    continue;

                int  preFloorNum = roomData.m_OpenTerms.m_nNeedOpenedFloor;
                bool preFloorOpen = false;
                
                // 1층인 경우 이전 층 오픈되있음. (대문)
                if(preFloorNum == 0)
                    preFloorOpen = true;

                if(preFloorNum > 0 && listRooms.Count > preFloorNum)
                    preFloorOpen = listRooms[preFloorNum -1].m_bOpened;

                roomData.m_OpenTerms.m_bNeedFloorOpen = preFloorOpen;
            }
        }
    }

    //** 캐릭터 인덱스 랜덤 리스트
    public List<int> GetRandomCharacterIndex(int count)
    {
        List<int> listRandomIndex = new List<int>();

        // 기본 캐릭터 Max 인덱스 (보스 인덱스 때문에)
        int normalCharCardMaxIndex = 999;
        List<DB_Card.Schema> cardList = DB_Card.instance.schemaList;

        for (int i = 0; i < count; i++)
        {
            int RandomNumber = UnityEngine.Random.Range(1, normalCharCardMaxIndex);

            // 중복 체크. (Index 리스트 0개 이상 존재 && 중복 캐릭 인덱스 존재)
            if (listRandomIndex.Count > 0 && listRandomIndex.Find(item => item == RandomNumber) == RandomNumber)
            {
                i--;
                continue;
            }

            // 캐릭 존재 체크.
            if (cardList.Find(item => item.Index == RandomNumber) == null)
            {
                i--;
                continue;
            }

            listRandomIndex.Add(RandomNumber);
        }

        return listRandomIndex;
    }

    //** 남은 시간 세팅
    public bool SetRemainTime(DateTime lastOpenTime, float totalTime, UISlider slider)
    {
        slider.maxValue = totalTime;

        TimeSpan remainTime = GetRemainTime(lastOpenTime, totalTime);
        remainTime = remainTime > TimeSpan.Zero ? remainTime : TimeSpan.Zero;

        float needSec = totalTime - (float)remainTime.TotalSeconds;

        slider.value = remainTime > TimeSpan.Zero ? needSec : totalTime;
        slider.m_Text.text = string.Format("{0:00} : {1:00} : {2:00}", remainTime.Hours, remainTime.Minutes, remainTime.Seconds);

        return remainTime <= TimeSpan.Zero;
    }

    #endregion

    #region 갱신

    // 업데이트
    public override void Update()
    {
        //** 남은 시간 업데이트
        CheckRewardCount();

        // 로비 new 아이콘용
        if (onActiveFranchiseRewardCallback != null)
            onActiveFranchiseRewardCallback(rewardCompletCount > 0);
    }

    // 방 갱신 (보상 받고 난 후, 방 오픈 하고 난 후) => 시간, 오픈
    private void RefleshRevRoom(CFranchiseBuilding buildingInfo, bool bOpened)
    {
        FranchiseRoomData roomData = FindRoomData(buildingInfo.m_iBuildingNumber, buildingInfo.m_byFloor);

        if (roomData == null)
        {
            Debug.LogError(string.Format("[Franchise] REV_PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_ACK : FranchiseRoomData (Room = 번호 : {0}) Data is Null", buildingInfo.m_byFloor));
            return;
        }

        roomData.m_bOpened = bOpened;
        roomData.m_lSequence = buildingInfo.m_Sequence;
        roomData.m_LastOpenTime = TimeUtility.ToDateTime(buildingInfo.m_iReceivedTime);
    }

    #endregion

    #region Find Data

    //** 모든 빌딩 데이터 반환
    public Dictionary<int, List<FranchiseRoomData>> FindAllBuildingData()
    {
        if (m_dicFranciseRoomDatas == null)
        {
            Debug.LogError("[Franchise] FindAllBuildingData : m_dicFranciseRoomDatas is Null");
            return null;
        }

        return m_dicFranciseRoomDatas;
    }

    //** 해당 빌딩의 방들 정보 반환
    public List<FranchiseRoomData> FindBuildingData(int buildingNum)
    {
        if (m_dicFranciseRoomDatas == null)
        {
            Debug.LogError("[Franchise] FindRoomData : m_dicFranciseRoomDatas is Null");
            return null;
        }

        if (!m_dicFranciseRoomDatas.ContainsKey(buildingNum))
        {
            Debug.LogError(string.Format("[Franchise] FindRoomData : m_dicFranciseRoomDatas의 Building(번호 : {0}) Data is Null", buildingNum));
            return null;
        }

        return m_dicFranciseRoomDatas[buildingNum];
    }

    //** 해당 빌딩 + 해당 층의 방 정보 반환
    public FranchiseRoomData FindRoomData(int buildingNum, int floorNum)
    {
        List<FranchiseRoomData> buildingData = FindBuildingData(buildingNum);

        if (buildingData == null)
            return null;

        return buildingData.Find(item => item.m_nFloor == floorNum);
    }

    //** 해당 빌딩 + 해당 층의 방 오픈 조건 반환
    public FranchiseOpenTerms FindRoomOpenTerms(int buildingNum, int floorNum)
    {
        FranchiseRoomData roomData = FindRoomData(buildingNum, floorNum);

        if (roomData == null)
            return null;

        return roomData.m_OpenTerms;
    }

    //** 해당 빌딩에 포함된 Speak List String 반환
    public List<TEXT_UI> FindBuildingSpeak(int buildingNum)
    {
        if (!m_dicSpeakDatas.ContainsKey(buildingNum))
        {
            Debug.LogError(string.Format("[Franchise] FindBuildingSpeak : List<string>의 Speak (Building = 번호 : {0}) Data is Null", buildingNum));
            return null;
        }

        return m_dicSpeakDatas[buildingNum];
    }

    #endregion

    //** 방 보상 받기 패킷 보내기.
    public void SendRoomRewardPacket(int iFranchiseBuildingIndex, byte byFloor, long lSequence)
    {
        CFranchiseBuilding buildingInfo = new CFranchiseBuilding();

        buildingInfo.m_byFloor = byFloor;
        buildingInfo.m_iBuildingNumber = iFranchiseBuildingIndex;
        buildingInfo.m_Sequence = lSequence;

        REQ_PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_SYN(buildingInfo);
    }

    //** 보상 받은 만한 것이 몇개있는 지 체크.
    public int CheckRewardCount()
    {
        int rewardCount = 0;

        if (!m_bSettingComplet)
            return 0;

        // 방
        foreach (List<FranchiseRoomData> roomDatas in m_dicFranciseRoomDatas.Values)
        {
            for (int i = 0; i < roomDatas.Count; i++)
            {
                FranchiseRoomData roomData = roomDatas[i];

                if (!roomData.m_bOpened)
                    continue;

                TimeSpan remainTime = GetRemainTime(roomData.m_LastOpenTime, roomData.m_fCoolTime);
                if (remainTime <= TimeSpan.Zero)
                    rewardCount++;
            }
        }

        // 스마일맨
        SmilePointRemainTime = GetRemainTime(SmilePointLastGetTime, SmilePointTotalTime);
        if (SmilePointRemainTime <= TimeSpan.Zero)
            rewardCount++;

        this.rewardCompletCount = rewardCount;

        return rewardCount;
    }

    //** 남은 시간 계산
    public TimeSpan GetRemainTime(DateTime lastUpdateTime, float coolTime)
    {
        DateTime currentTime = TimeUtility.currentServerTime;
        DateTime endTime = lastUpdateTime.AddSeconds(coolTime);
        return endTime - currentTime;
    }

    #region Req

    //** 가맹점 정보 받기 요청
    public void REQ_PACKET_CG_READ_FRANCHISE_BUILDING_INFO_SYN()
    {
        m_bSettingComplet = false;
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_FRANCHISE_BUILDING_INFO_SYN(), false);
    }

    //** 방 열기 요청
    public void REQ_PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_SYN(byte byFloor, int iFranchiseBuildingIndex)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_SYN()
            {
                m_byFloor = byFloor,
                m_iFranchiseBuildingIndex = iFranchiseBuildingIndex
            });

    }

    //** 방 보상 받기 요청
    public void REQ_PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_SYN(CFranchiseBuilding info)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_SYN()
            {
                m_FranchiseBuildingInfo = info
            });

    }

    //** 스마일 포인트 받기 요청
    public void REQ_PACKET_CG_GAME_RECEIVE_SMILE_POINT_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_RECEIVE_SMILE_POINT_SYN());
    }

    #endregion

    #region Rev

    //** 가맹점 정보 받기 요청 결과
    private void REV_PACKET_CG_READ_FRANCHISE_BUILDING_INFO_ACK(PACKET_CG_READ_FRANCHISE_BUILDING_INFO_ACK packet)
    {
        SetBuildingData();
        SettingPacket(packet.m_FranchiseBuildingList, packet.m_iSmilePointReceivedTime);
        m_bSettingComplet = true;
    }

    //** 방 열기 요청 결과
    private void REV_PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_ACK(PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_ACK packet)
    {
        // 재화 세팅
        Kernel.entry.account.gold           = packet.m_iRemainGold;
        Kernel.entry.account.heart          = packet.m_iRemainHeart;
        Kernel.entry.account.revengePoint   = packet.m_iRemainRevengePoint;
        Kernel.entry.account.smilePoint     = packet.m_iRemainSmilePoint;
        Kernel.entry.account.starPoint      = packet.m_iRemainStarPoint;

        // 방 업데이트
        CFranchiseBuilding packetRoomData = packet.m_OpenFloorInfo;
        RefleshRevRoom(packetRoomData, true);

        // 이전 오픈 방 갱신
        SetNeedFloorOpen();

        if (onRevRoomOpen != null)
            onRevRoomOpen();
    }

    //** 방 보상 받기 요청 결과
    private void REV_PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_ACK(PACKET_CG_GAME_RECEIVE_FRANCHISE_REWARD_ACK packet)
    {
        Kernel.entry.account.SetValue(packet.m_RewardGoods.m_eGoodsType, packet.m_RewardGoods.m_iTotalAmount);

        // 방 업데이트
        CFranchiseBuilding packetRoomData = packet.m_FranchiseBuildingInfo;
        RefleshRevRoom(packetRoomData, true);

        if(onRevRoomReward != null)
            onRevRoomReward(packetRoomData.m_iBuildingNumber, packetRoomData.m_byFloor, packetRoomData.m_iReceivedTime);
    }

    //** 스마일 포인트 받기 요청 결과
    private void REV_PACKET_CG_GAME_RECEIVE_SMILE_POINT_ACK(PACKET_CG_GAME_RECEIVE_SMILE_POINT_ACK packet)
    {
        m_fSmilePointLastGetTime = TimeUtility.ToDateTime(packet.m_iReceiveTime);

        Kernel.entry.account.smilePoint = packet.m_iSmilePoint;

        if(onRevSmilePointCallBack != null)
            onRevSmilePointCallBack();
    }

    #endregion
}
