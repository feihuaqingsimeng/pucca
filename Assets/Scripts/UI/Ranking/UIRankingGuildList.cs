using Common.Packet;
using System.Collections.Generic;

public class UIRankingGuildList : UIRankingList
{

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onRankerGuildInfoListUpdate += OnRankerGuildInfoListUpdate;

            List<CRankerGuildInfo> rankerGuildInfoList = Kernel.entry.ranking.rankerGuildInfoList;
            if (rankerGuildInfoList != null && rankerGuildInfoList.Count > 0)
            {
                OnRankerGuildInfoListUpdate(m_Page, rankerGuildInfoList, 0, rankerGuildInfoList.Count);
            }
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onRankerGuildInfoListUpdate -= OnRankerGuildInfoListUpdate;
        }
    }

    protected override void OnActivityIndicatorActive(bool activityIndicatorActive)
    {
        if (!activityIndicatorActive)
        {
            return;
        }

        if (m_Page < 4)
        {
            if (Kernel.entry != null)
            {
                Kernel.entry.ranking.REQ_PACKET_CG_RANK_REQUEST_GUILD_RANKING_SYN(++m_Page, false);
            }
        }
        else
        {
            m_ScrollRect.SetActivity(false);
        }
    }

    void OnRankerGuildInfoListUpdate(byte page, List<CRankerGuildInfo> rankerGuildInfoList, int startIndex, int length)
    {
        bool isExist = (rankerGuildInfoList != null) && (rankerGuildInfoList.Count > 0);
        bool isLast = !isExist || (page >= Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Ranking_Page_Count)) || page * Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Ranking_List_Per_Page) > rankerGuildInfoList.Count;

        if (isExist)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                UIRankingGuildObject item = null;
                if (i < m_RankingObjectList.Count)
                {
                    item = (UIRankingGuildObject)m_RankingObjectList[i];
                }
                else
                {
                    item = m_Pool.Pop<UIRankingGuildObject>();
                    UIUtility.SetParent(item.transform, m_ScrollRect.content);
                    m_RankingObjectList.Add(item);
                }

                if (item)
                {
                    item.SetRankerGuildInfo(rankerGuildInfoList[i]);
                }
            }
        }
        else
        {
            m_Page--;
        }

        if (!isExist || isLast)
        {
            m_ScrollRect.useActivityIndicator = false;
        }

        m_ScrollRect.SetActivity(false);
        BuildLayout();
    }
}

