using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EventTimeSprite
{
    public eEventTimeType    m_eSpriteType;
    public Sprite           m_sprite;
}

public class UIEventTime : MonoBehaviour 
{
    public  UIEventTimeObject       m_CopyEventObject;
    private List<UIEventTimeObject> m_listCopyEventObjects = new List<UIEventTimeObject>();
    public  Transform               m_trsParent;

    public  List<EventTimeSprite>   m_arrIconSprite;

    //** 기본 세팅
    public void SetInit()
    {
        // 정보 패킷 요청 -> 정보 갱신 및 세팅 다 될때까지 기다리기 -> 아이템 생성
        //Kernel.entry.eventTime.

        //패킷 나올때까지 주석
        //StartCoroutine(WaitSettingComplet());
        CreateItems();
    }

    //** 데이터 세팅 다 될때 까지 대기
    public IEnumerator WaitSettingComplet()
    {
        while (!Kernel.entry.eventTime.m_bSettingComplet)
            yield return null;

        CreateItems();
    }

    //** 아이템 생성
    public void CreateItems()
    {
        DestroyAllItems();
        m_CopyEventObject.gameObject.SetActive(true);

        Dictionary<eEventTimeType, EventTimeData> activeEventData = Kernel.entry.eventTime.GetActiveData();

        // 아무것도 없음
        if(activeEventData == null)
            return;

        if(m_arrIconSprite == null)
            return;

        foreach(KeyValuePair<eEventTimeType, EventTimeData> activeData in activeEventData)
        {
            EventTimeData eventData = activeData.Value;

            if(eventData == null)
                continue;

            EventTimeSprite iconSprite = m_arrIconSprite.Find(item => item.m_eSpriteType == activeData.Key);

            if(iconSprite == null)
                continue;

            UIEventTimeObject copyObject = Instantiate<UIEventTimeObject>(m_CopyEventObject);
            UIUtility.SetParent(copyObject.transform, m_trsParent);

            copyObject.SetInit(activeData.Key, iconSprite.m_sprite, TEXT_UI.REMAINING); //TextUI 필요
            copyObject.m_RemainTime = eventData.m_RemainTime;

            m_listCopyEventObjects.Add(copyObject);
        }

        m_CopyEventObject.gameObject.SetActive(false);
    }

    //** 시간 업데이트
    private void TimeUpdate()
    {
        Kernel.entry.eventTime.TimeUpdate();

        Dictionary<eEventTimeType, EventTimeData> activeEventData = Kernel.entry.eventTime.GetActiveData();

        // 아무것도 없음
        if (activeEventData == null || activeEventData.Count <= 0)
        {
            m_CopyEventObject.gameObject.SetActive(false);
            return;
        }

        // 업데이트 및 삭제될 아이템 등록
        List<eEventTimeType> removeItem = new List<eEventTimeType>();
        for(int i = 0; i < m_listCopyEventObjects.Count; i++)
        {
            UIEventTimeObject copyObejct = m_listCopyEventObjects[i];

            if (activeEventData.ContainsKey(copyObejct.m_eEventType))
                copyObejct.TimeUpdate(activeEventData[copyObejct.m_eEventType].m_RemainTime);
            else
                removeItem.Add(copyObejct.m_eEventType);
        }

        //삭제 되어야 할 아이템 삭제
        for(int i = 0; i < removeItem.Count; i++)
            DestroyItem(removeItem[i]);
    }

    //** 아이템 하나 지우기
    private void DestroyItem(eEventTimeType eventType)
    {
        if (m_listCopyEventObjects == null)
            return;

        UIEventTimeObject destroyItem =  m_listCopyEventObjects.Find(item => item.m_eEventType == eventType);

        if (destroyItem == null)
            return;

        Destroy(destroyItem.gameObject);
        m_listCopyEventObjects.Remove(destroyItem);
    }

    //** 모든 아이템 지우기
    private void DestroyAllItems()
    {
        if (m_listCopyEventObjects == null)
            return;

        for (int i = 0; i < m_listCopyEventObjects.Count; i++)
        {
            UIEventTimeObject eventObject = m_listCopyEventObjects[i];
            Destroy(eventObject.gameObject);
        }

        m_listCopyEventObjects.Clear();
    }
}
