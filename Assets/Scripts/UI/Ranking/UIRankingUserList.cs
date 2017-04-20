using Common.Packet;
using System.Collections.Generic;

public class UIRankingUserList : UIRankingList
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onRankerInfoListUpdate += OnRankerInfoListUpdate;

            List<CRankerInfo> rankerInfoList = Kernel.entry.ranking.rankerInfoList;
            if (rankerInfoList != null && rankerInfoList.Count > 0)
            {
                OnRankerInfoListUpdate(m_Page, rankerInfoList, 0, rankerInfoList.Count);
            }
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onRankerInfoListUpdate -= OnRankerInfoListUpdate;
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
                Kernel.entry.ranking.REQ_PACKET_CG_RANK_REQUEST_PVP_RANKING_SYN(++m_Page, false);
            }
        }
        else
        {
            m_ScrollRect.SetActivity(false);
        }
    }

    void OnRankerInfoListUpdate(byte page, List<CRankerInfo> rankerInfoList, int startIndex, int length)
    {
        bool isExist = (rankerInfoList != null) && (rankerInfoList.Count > 0);
        bool isLast = !isExist || (page >= Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Ranking_Page_Count)) || page * Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Ranking_List_Per_Page) > rankerInfoList.Count;

        if (isExist)
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                UIRankingUserObject item = null;
                if (i < m_RankingObjectList.Count)
                {
                    item = (UIRankingUserObject)m_RankingObjectList[i];
                }
                else
                {
                    item = m_Pool.Pop<UIRankingUserObject>();
                    UIUtility.SetParent(item.transform, m_ScrollRect.content);
                    m_RankingObjectList.Add(item);
                }

                if (item)
                {
                    item.SetRankerInfo(rankerInfoList[i]);
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
