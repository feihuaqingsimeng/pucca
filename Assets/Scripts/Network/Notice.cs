using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eNoticeType
{
    NT_NONE,
    NT_EVENT,
    NT_SYSTEM,
    NT_PROMOTION
}

public enum eNoticeIssueType
{
    NIT_NONE,
    NIT_HOT,
    NIT_NEW
}

public class NoticeData
{
    public eNoticeType      m_eNoticeType;
    public eNoticeIssueType m_eNoticeIssueType;
    public string           m_strURL;
    public string           m_strDec;
}

public class Notice : Node
{
    public bool m_bSettingComplet = false;

    private List<NoticeData> m_listNoticeDatas = new List<NoticeData>();

    public override Node OnCreate()
    {
        //entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_RECEIVE_POST_ALL_ACK>(REV_PACKET_CG_GAME_RECEIVE_POST_ALL_ACK);
        return base.OnCreate();
    }

    //** 데이터 세팅
    private void SetData(List<NoticeData> noticeData)
    {
        if (noticeData == null)
            return;

        if (m_listNoticeDatas != null || m_listNoticeDatas.Count > 0)
            m_listNoticeDatas.Clear();

        for (int i = 0; i < noticeData.Count; i++)
        {
            NoticeData data = noticeData[i];

            NoticeData newNoticeData = new NoticeData();
            newNoticeData.m_eNoticeIssueType = data.m_eNoticeIssueType;
            newNoticeData.m_eNoticeType = data.m_eNoticeType;
            newNoticeData.m_strDec = data.m_strDec;
            newNoticeData.m_strURL = data.m_strURL;

            m_listNoticeDatas.Add(data);
        }

        m_bSettingComplet = true;
    }

    public List<NoticeData> GetNoticeDatas()
    {
        return m_listNoticeDatas;
    }

    //** Test
    public void TestNoticePacket()
    {
        m_bSettingComplet = false;

        SetData(TestPacketData());
    }

    public List<NoticeData> TestPacketData()
    {
        List<NoticeData> newlist = new List<NoticeData>();

        // 1개
        NoticeData newData = new NoticeData();
        newData.m_eNoticeIssueType = eNoticeIssueType.NIT_HOT;
        newData.m_eNoticeType = eNoticeType.NT_EVENT;
        newData.m_strDec = "Test01";
        newData.m_strURL = "http://1.255.56.172/Notice/notice_test1.html";
        newlist.Add(newData);

        // 2개
        NoticeData newData02 = new NoticeData();
        newData02.m_eNoticeIssueType = eNoticeIssueType.NIT_NEW;
        newData02.m_eNoticeType = eNoticeType.NT_PROMOTION;
        newData02.m_strDec = "Test02";
        newData02.m_strURL = "http://1.255.56.172/Notice/notice_test2.html";
        newlist.Add(newData02);

        // 3개
        NoticeData newData03 = new NoticeData();
        newData03.m_eNoticeIssueType = eNoticeIssueType.NIT_HOT;
        newData03.m_eNoticeType = eNoticeType.NT_SYSTEM;
        newData03.m_strDec = "Test03";
        newData03.m_strURL = "http://www.naver.com";
        newlist.Add(newData03);

        return newlist;
    }
}
