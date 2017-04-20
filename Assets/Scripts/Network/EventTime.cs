using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum eEventTimeType
{
    ET_NONE,
    ET_DOBBLE_ACCOUNTEXP,
    ET_DOBBLE_GOLD,
    ET_HALF_HEART,
    ET_GET_PVE_CHEST,
    ET_GET_PVP_CHEST
}

public class EventTimeData
{
    public bool             m_bAcitve           = false;
    public DateTime         m_EndTime           = DateTime.MinValue; //활성화 리스트만 갱신 중
    public TimeSpan         m_RemainTime        = TimeSpan.MinValue; //활성화 리스트만 갱신 중
}

public class EventTime : Node 
{
    public bool m_bSettingComplet = false;

    public override Node OnCreate()
    {
        //entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_RECEIVE_POST_ALL_ACK>(REV_PACKET_CG_GAME_RECEIVE_POST_ALL_ACK);
        return base.OnCreate();
    }

    //** 모든 데이터
    private Dictionary<eEventTimeType, EventTimeData> m_dicAllEventData      = new Dictionary<eEventTimeType, EventTimeData>();
    private Dictionary<eEventTimeType, EventTimeData> m_dicActiveEventData   = new Dictionary<eEventTimeType, EventTimeData>();

    //** 데이터 세팅
    private void SetData()
    {

    }

    #region Get

    //** 모든 데이터 반환
    public Dictionary<eEventTimeType, EventTimeData> GetAllData()
    {
        return m_dicAllEventData;
    }

    //** 활성화 중인 데이터 반환
    public Dictionary<eEventTimeType, EventTimeData> GetActiveData()
    {
        return m_dicActiveEventData;
    }

    //** 타입으로 데이터 반환
    public EventTimeData GetTypeData(eEventTimeType eventType)
    {
        if (m_dicAllEventData == null)
            return null;

        if (m_dicAllEventData.ContainsKey(eventType))
            return m_dicAllEventData[eventType];

        return null;
    }

    //** 타입으로 활성화 된 것 중 반환
    public EventTimeData GetTypeActiveData(eEventTimeType eventType)
    {
        if (m_dicActiveEventData == null)
            return null;

        if (m_dicActiveEventData.ContainsKey(eventType))
            return m_dicActiveEventData[eventType];

        return null;
    }

    //** 활성화 리스트 중 타입으로 남은 시간 반환
    public TimeSpan GetTypeRemainTime(eEventTimeType eventType)
    {
        EventTimeData eventData = GetTypeActiveData(eventType);

        if (eventData == null)
            return TimeSpan.MinValue;

        return eventData.m_RemainTime;
    }

    #endregion

    #region Update

    //** 타입별 시간 업데이트
    public void TimeUpdate()
    {
        if (m_dicActiveEventData  == null ||m_dicActiveEventData.Count <= 0)
            return;

        List<eEventTimeType> removeType = new List<eEventTimeType>();

        foreach (KeyValuePair<eEventTimeType, EventTimeData> activeData in m_dicActiveEventData)
        {
            EventTimeData data = activeData.Value;

            TimeSpan remainTime = data.m_EndTime - TimeUtility.currentServerTime;

            if (remainTime <= TimeSpan.Zero)
                removeType.Add(activeData.Key);
            else
                data.m_RemainTime = remainTime;
        }

        UpdateActiveData(removeType);
    }

    //** 시간 다된 것들 삭제 (활성화 리스트에서)
    private void UpdateActiveData(List<eEventTimeType> removeList)
    {
        for (int i = 0; i < removeList.Count; i++)
        {
            eEventTimeType removeType = removeList[i];

            // 모든 데이터에 해당 타입 데이터 초기화
            if (m_dicAllEventData.ContainsKey(removeType))
            {
                m_dicAllEventData[removeType].m_bAcitve     = false;
                m_dicAllEventData[removeType].m_RemainTime  = TimeSpan.Zero;
                m_dicAllEventData[removeType].m_EndTime     = DateTime.MinValue;
            }

            // 활성화 데이터에 해당 타입 삭제
            if (m_dicActiveEventData.ContainsKey(removeType))
                m_dicActiveEventData.Remove(removeType);
        }
    }

    #endregion
}
