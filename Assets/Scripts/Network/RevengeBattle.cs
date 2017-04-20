using Common.Packet;
using System.Collections.Generic;
using Common.Util;

public class RevengeBattle : Node
{
    List<CRevengeMatchInfo> m_RevengeMatchInfoList = new List<CRevengeMatchInfo>();

    public List<CRevengeMatchInfo> revengeMatchInfoList
    {
        get
        {
            return m_RevengeMatchInfoList;
        }
    }

    public long revengeSequence
    {
        get;
        set;
    }

    public ePvpResult result
    {
        get;
        private set;
    }

    public delegate void OnUpdatedRevengeMatchInfoList(List<CRevengeMatchInfo> revengeMatchInfoList);
    public OnUpdatedRevengeMatchInfoList onUpdatedRevengeMatchInfoList;

    public delegate void OnStartedRevengeMatch(long sequence);
    public OnStartedRevengeMatch onStartedRevengeMatch;

    public delegate void OnRevengeMatchResult(CReceivedGoods RecvGoodsInfo);
    public OnRevengeMatchResult onRevengeMatchResult;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_REVENGE_MATCH_LIST_ACK>(RCV_PACKET_CG_READ_REVENGE_MATCH_LIST_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_START_REVENGE_MATCH_ACK>(RCV_PACKET_CG_GAME_START_REVENGE_MATCH_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_REVENGE_MATCH_RESULT_ACK>(RCV_PACKET_CG_GAME_REVENGE_MATCH_RESULT_ACK);

        return base.OnCreate();
    }

    public CRevengeMatchInfo FindRevengeMatchInfo(long sequence)
    {
        return m_RevengeMatchInfoList.Find(item => (item.m_Sequence == sequence));
    }

    #region REQ
    public void REQ_PACKET_CG_READ_REVENGE_MATCH_LIST_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_REVENGE_MATCH_LIST_SYN());
    }

    public void REQ_PACKET_CG_GAME_START_REVENGE_MATCH_SYN(long sequence)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_START_REVENGE_MATCH_SYN()
            {
                m_Sequence = sequence,
            });
    }

    public void REQ_PACKET_CG_GAME_REVENGE_MATCH_RESULT_SYN(ePvpResult result)
    {
        this.result = result;
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_REVENGE_MATCH_RESULT_SYN()
            {
                m_MatchResult = result,
            });
    }
    #endregion

    #region RCV
    void RCV_PACKET_CG_READ_REVENGE_MATCH_LIST_ACK(PACKET_CG_READ_REVENGE_MATCH_LIST_ACK packet)
    {
        m_RevengeMatchInfoList.Clear();
        if (packet.m_RevengeMatchInfoList != null
            && packet.m_RevengeMatchInfoList.Count > 0)
        {
            m_RevengeMatchInfoList.AddRange(packet.m_RevengeMatchInfoList);
        }

        if (onUpdatedRevengeMatchInfoList != null)
        {
            onUpdatedRevengeMatchInfoList(m_RevengeMatchInfoList);
        }
    }

    void RCV_PACKET_CG_GAME_START_REVENGE_MATCH_ACK(PACKET_CG_GAME_START_REVENGE_MATCH_ACK packet)
    {
        entry.account.heart = packet.m_iRemainHeart;

        if (onStartedRevengeMatch != null)
        {
            onStartedRevengeMatch(packet.m_Sequence);
        }
    }

    void RCV_PACKET_CG_GAME_REVENGE_MATCH_RESULT_ACK(PACKET_CG_GAME_REVENGE_MATCH_RESULT_ACK packet)
    {
        if (packet.m_RevenceMatchInfo != null)
        {
            CRevengeMatchInfo revengeMatchInfo = FindRevengeMatchInfo(packet.m_RevenceMatchInfo.m_Sequence);
            if (revengeMatchInfo != null)
            {
                // Deep Copy
                revengeMatchInfo.m_Aid = packet.m_RevenceMatchInfo.m_Aid;
                revengeMatchInfo.m_bIsRevenge = packet.m_RevenceMatchInfo.m_bIsRevenge;
                revengeMatchInfo.m_byLevel = packet.m_RevenceMatchInfo.m_byLevel;
                revengeMatchInfo.m_iBattleTime = packet.m_RevenceMatchInfo.m_iBattleTime;
                revengeMatchInfo.m_iEarnRevengePoint = packet.m_RevenceMatchInfo.m_iEarnRevengePoint;
                revengeMatchInfo.m_iLeaderCardIndex = packet.m_RevenceMatchInfo.m_iLeaderCardIndex;
                revengeMatchInfo.m_sDeckInfo = packet.m_RevenceMatchInfo.m_sDeckInfo;
                revengeMatchInfo.m_Sequence = packet.m_RevenceMatchInfo.m_Sequence;
                revengeMatchInfo.m_sUserName = packet.m_RevenceMatchInfo.m_sUserName;

                if (packet.m_ReceivedGoods != null)
                {
                    entry.account.SetValue(packet.m_ReceivedGoods.m_eGoodsType, packet.m_ReceivedGoods.m_iTotalAmount);
                }

                if (onRevengeMatchResult != null)
                {
                    onRevengeMatchResult(packet.m_ReceivedGoods);
                }
            }
        }
    }
    #endregion
}
