using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


//** 알림 데이터
public class NotificationObject
{
    public int      m_nNotiID;
    public TimeSpan m_RemainTime;

    public string   m_strTitle;
    public string   m_strSubDec;
    public string   m_strImageName;
}

public class NotificationChecker : MonoBehaviour 
{
    //** 알림 오브젝트 타입 (알림 ID)
    private enum eNotificationObjectType
    {
        NTO_NONE            = 0,
        NTO_SMILEPOINT      = 1,
        NOT_HEARTMAX        = 2,
        NOT_TREASUREBOX01   = 3,
        NOT_TREASUREBOX02   = 4,
        NOT_TREASUREBOX03   = 5,
    }

    private List<NotificationObject> m_listSendData     = new List<NotificationObject>();  //알림 추가 리스트

    //** 각 타입의 알림 저장
    public void SaveNotificationType()
    {
        if (m_listSendData != null && m_listSendData.Count > 0)
            m_listSendData.Clear();

        CheckFranchiseSmilePointRemainTime();
        CheckHeartRemainTime();
        CheckTreasureRemainTime();

        for (int i = 0; i < m_listSendData.Count; i++)
        {
            NotificationObject notiObject = m_listSendData[i];

            DateTime alarmTime = TimeUtility.currentServerTime.AddSeconds(notiObject.m_RemainTime.TotalSeconds);
            Kernel.notificationManager.ScheduleNotification(alarmTime, notiObject.m_strTitle, notiObject.m_strSubDec, notiObject.m_nNotiID);
            Debug.Log(string.Format("NotificationChecker (Type : {0}), (Alarm Time : {1}), (Title : {2}), (Dec : {3})", (eNotificationObjectType)notiObject.m_nNotiID, alarmTime, notiObject.m_strTitle, notiObject.m_strSubDec));
        }
    }

    //** 가맹점 스마일 포인트 남은 시간
    private void CheckFranchiseSmilePointRemainTime()
    {
        TimeSpan remainTime = Kernel.entry.franchise.SmilePointRemainTime;

        // 실행 중 받을 수 있는 상황은 리턴.
        if (remainTime <= TimeSpan.Zero)
            return;

        m_listSendData.Add
            (CreateNotificationObject((int)eNotificationObjectType.NTO_SMILEPOINT, Languages.ToString(TEXT_UI.GOODS_SMILE_POINT), Languages.ToString(TEXT_UI.SHORTCUT), remainTime));
    }

    //** 하트 남은 시간
    private void CheckHeartRemainTime()
    {
        bool isMaxHeart = Kernel.entry.account.maxHeart <= Kernel.entry.account.heart;

        // 실행 중 하트 맥스일 경우 리턴.
        if (isMaxHeart)
            return;

        int m_HeartRecoveryCycleSec = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Heart_Recovery_Cycle_Sec);
        int remainHeartCount = Kernel.entry.account.maxHeart - Kernel.entry.account.heart;
        int remainHeartMaxTime = (remainHeartCount -1) * m_HeartRecoveryCycleSec;
        TimeSpan remainTime = TimeUtility.currentServerTime.AddSeconds(remainHeartMaxTime) - TimeUtility.currentServerTime;

        m_listSendData.Add
            (CreateNotificationObject((int)eNotificationObjectType.NOT_HEARTMAX, Languages.ToString(TEXT_UI.HEART), Languages.ToString(TEXT_UI.SHORTCUT), remainTime));
    }

    //** 보물찾기 남은 시간
    private void CheckTreasureRemainTime()
    {
        int boxIndex = 1;
        int notiTypeIdex = 3;
        foreach (var treasureInfo in Kernel.entry.treasure.m_dicTreasureBox.Values)
        {
            // 실행 중 받을 수 있는 박스는 컨티뉴.
            if (treasureInfo.m_RemainTime <= TimeSpan.Zero)
            {
                boxIndex++;
                notiTypeIdex++;
                continue;
            }

            //TitleName
            string boatName = "TREASURE_BOAT_" + boxIndex;
            TEXT_UI enumBoatName = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), boatName);

            m_listSendData.Add
                (CreateNotificationObject(notiTypeIdex, Languages.ToString(enumBoatName), Languages.ToString(TEXT_UI.SHORTCUT), treasureInfo.m_RemainTime));

            boxIndex++;
            notiTypeIdex++;
        }
    }

    private NotificationObject CreateNotificationObject(int id, string title, string dec, TimeSpan remainTime)
    {
        NotificationObject notiObject = new NotificationObject();
        notiObject.m_nNotiID    = id;
        notiObject.m_strTitle   = title;
        notiObject.m_strSubDec  = dec;
        notiObject.m_RemainTime = remainTime;

        return notiObject;
    }
}
