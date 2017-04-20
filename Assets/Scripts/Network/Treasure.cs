using Common.Packet;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreasureInfo
{
    public CTreasureBox m_treasureBoxData;
    public DB_TreasureSearch.Schema m_treasureDBData;
    public DateTime m_OpenableTime;
    public TimeSpan m_RemainTime;
}

public class Treasure : Node
{
    public Dictionary<long, TreasureInfo> m_dicTreasureBox = new Dictionary<long, TreasureInfo>();

    int m_OpenableTreasureBoxCount;

    public int openableTreasureBoxCount
    {
        get
        {
            return m_OpenableTreasureBoxCount;
        }

        set
        {
            if (m_OpenableTreasureBoxCount != value)
            {
                m_OpenableTreasureBoxCount = value;

                if (onChangedOpenableTreasureBoxCount != null)
                {
                    onChangedOpenableTreasureBoxCount(m_OpenableTreasureBoxCount);
                }
            }
        }
    }

    // OnChangedOpenableTreasureBoxCount 이벤트로 처리하도록 수정해야합니다.
    public delegate void OnActiveTreasureBoxCallback(bool active);
    public OnActiveTreasureBoxCallback onActiveTreasureBoxCallback;

    public delegate void OnOpenTreasureBoxCallback(int boxIndex, int gold, List<CBoxResult> boxResultList, List<CTreasureBox> newBoxList);
    public OnOpenTreasureBoxCallback onOpenTreasureBoxCallback;

    public delegate void OnChangedOpenableTreasureBoxCount(int openableTreasureBoxCount);
    public OnChangedOpenableTreasureBoxCount onChangedOpenableTreasureBoxCount;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_TREASURE_BOX_LIST_ACK>(REV_PACKET_CG_READ_TREASURE_BOX_LIST_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_OPEN_TREASURE_BOX_ACK>(REV_PACKET_CG_GAME_OPEN_TREASURE_BOX_ACK);

        return base.OnCreate();
    }

    public override void Update()
    {
        //** 남은 시간 업데이트
        foreach (TreasureInfo boxData in m_dicTreasureBox.Values)
            boxData.m_RemainTime = CalculateRemainTime(boxData);

        CheckOpenBoxExist();
    }

    //** Box RemainTime 계산
    public TimeSpan CalculateRemainTime(TreasureInfo boxData)
    {
        if (boxData == null)
            return TimeSpan.MinValue;

        return boxData.m_OpenableTime - TimeUtility.currentServerTime;
    }

    //** Sequence로 박스 정보 반환.
    public TreasureInfo FindTreasureBox(long sequence)
    {
        TreasureInfo treasureInfo = new TreasureInfo();
        m_dicTreasureBox.TryGetValue(sequence, out treasureInfo);

        return treasureInfo != null ? treasureInfo : null;
    }

    //** Sequence로 박스 오픈 레벨 반환.
    public int FindTreasureBoxOpenLevel(long sequence)
    {
        TreasureInfo treasureInfo = FindTreasureBox(sequence);

        return treasureInfo != null ? treasureInfo.m_treasureDBData.Need_AccountLevel : 0;
    }

    //** 한 상자라도 열 것이 있는지 체크
    public void CheckOpenBoxExist()
    {
        int openableTreasureBoxCount = 0;
        if (m_dicTreasureBox != null
            && m_dicTreasureBox.Count > 0)
        {
            foreach (TreasureInfo treasureInfo in m_dicTreasureBox.Values)
            {
                if (treasureInfo != null)
                {
                    if (treasureInfo.m_RemainTime <= TimeSpan.Zero
                        && treasureInfo.m_treasureDBData.Need_AccountLevel <= Kernel.entry.account.level)
                    {
                        openableTreasureBoxCount++;
                    }
                }
            }
        }

        if (m_OpenableTreasureBoxCount != openableTreasureBoxCount)
        {
            this.openableTreasureBoxCount = openableTreasureBoxCount;
        }

        if (onActiveTreasureBoxCallback != null)
        {
            onActiveTreasureBoxCallback(m_OpenableTreasureBoxCount > 0);
        }
    }

    //** Packet에서 받은 박스 리스트 정보 등록
    public void SettingData(List<CTreasureBox> newBoxData)
    {
        if (m_dicTreasureBox != null)
            m_dicTreasureBox.Clear();

        int count = 1;

        foreach (CTreasureBox newData in newBoxData)
        {
            DB_TreasureSearch.Schema boxInfo = DB_TreasureSearch.Query(DB_TreasureSearch.Field.Index, count);

            if (boxInfo == null)
            {
                Debug.LogError(string.Format("DB_TreasureSearch : Index {0} is null", count));
                continue;
            }

            TreasureInfo newBoxInfo = new TreasureInfo();
            newBoxInfo.m_treasureBoxData = newData;
            newBoxInfo.m_treasureDBData = boxInfo;
            newBoxInfo.m_OpenableTime = TimeUtility.ToDateTime(newData.m_iLastOpenTime).AddSeconds(boxInfo.Open_Time);
            m_dicTreasureBox.Add(newData.m_Sequence, newBoxInfo);

            count++;
        }

        //UpdateRemainTime();
        Update();
    }

    #region REQ

    //** TreasureBox List 정보 요청
    public void REQ_PACKET_CG_READ_TREASURE_BOX_LIST_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_TREASURE_BOX_LIST_SYN());

    }

    //** TreasureBox 열기 요청
    public void REQ_PACKET_CG_GAME_OPEN_TREASURE_BOX_SYN(long sequence)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_OPEN_TREASURE_BOX_SYN()
        {
            m_BoxSequence = sequence
        });
    }

    #endregion

    #region REV

    //** TreasureBox List 정보 받기
    private void REV_PACKET_CG_READ_TREASURE_BOX_LIST_ACK(PACKET_CG_READ_TREASURE_BOX_LIST_ACK packet)
    {
        if (m_dicTreasureBox != null && packet.m_TreasureBoxList != null)
            SettingData(packet.m_TreasureBoxList);
    }

    //** TreasureBox Reward 정보 받기
    private void REV_PACKET_CG_GAME_OPEN_TREASURE_BOX_ACK(PACKET_CG_GAME_OPEN_TREASURE_BOX_ACK packet)
    {
        TreasureInfo rewardBox = FindTreasureBox(packet.m_BoxSequence);

        if (rewardBox == null)
            return;

        int boxIndex = 0;
        if (rewardBox != null)
            boxIndex = rewardBox.m_treasureBoxData.m_iBoxIndex;

        entry.account.gold = packet.m_iTotalGold;

        for (int i = 0; i < packet.m_CardList.Count; i++)
            entry.character.UpdateCardInfo(packet.m_CardList[i]);

        for (int i = 0; i < packet.m_SoulList.Count; i++)
            entry.character.UpdateSoulInfo(packet.m_SoulList[i]);

        // 박스 Data 갱신
        SettingData(packet.m_BoxList);

        if (onOpenTreasureBoxCallback != null)
            onOpenTreasureBoxCallback(boxIndex, packet.m_iEarnGold, packet.m_BoxResultList, packet.m_BoxList);
    }

    #endregion
}
