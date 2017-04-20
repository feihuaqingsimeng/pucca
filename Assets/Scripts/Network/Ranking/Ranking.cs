using Common.Packet;
using System.Collections.Generic;

public class Ranking : Node
{
    #region Variables
    CFranchiseRankingInfo m_RankingInfo;
    List<CRankerInfo> m_RankerInfoList = new List<CRankerInfo>();
    List<CRankerGuildInfo> m_RankerGuildInfoList = new List<CRankerGuildInfo>();
    CRankReward m_DailyRankReward;
    CRankReward m_SeasonRankReward;
    #endregion

    #region Properties
    // 시즌 시작 시간
    public int seasonRewardTime
    {
        get;
        private set;
    }

    public CFranchiseRankingInfo rankingInfo
    {
        get
        {
            return m_RankingInfo;
        }

        private set
        {
            if (m_RankingInfo != value)
            {
                m_RankingInfo = value;

                if (onRankingInfoUpdate != null)
                {
                    onRankingInfoUpdate(m_RankingInfo);
                }
            }
        }
    }

    public List<CRankerInfo> rankerInfoList
    {
        get
        {
            return m_RankerInfoList;
        }
    }

    public List<CRankerGuildInfo> rankerGuildInfoList
    {
        get
        {
            return m_RankerGuildInfoList;
        }
    }

    public CRankReward dailyRankReward
    {
        get
        {
            return m_DailyRankReward;
        }
    }

    public CRankReward seasonRankReward
    {
        get
        {
            return m_SeasonRankReward;
        }
    }
    #endregion

    #region Delegates
    public delegate void OnRankingInfoUpdate(CFranchiseRankingInfo rankingInfo);
    public OnRankingInfoUpdate onRankingInfoUpdate;

    public delegate void OnRankerInfoListUpdate(byte page, List<CRankerInfo> rankerInfoList, int startIndex, int length);
    public OnRankerInfoListUpdate onRankerInfoListUpdate;

    public delegate void OnRankerGuildInfoListUpdate(byte page, List<CRankerGuildInfo> rankerGuildInfoList, int startIndex, int length);
    public OnRankerGuildInfoListUpdate onRankerGuildInfoListUpdate;

    public delegate void OnUserInfoResult(UserInfo userInfo);
    public OnUserInfoResult onUserInfoResult;

    public delegate void OnGuildInfoResult(CGuildBase guildBase);
    public OnGuildInfoResult onGuildInfoResult;

    public delegate void OnDailyRewardResult(int ruby);
    public OnDailyRewardResult onDailyRewardResult;

    public delegate void OnSeasonRewardResult(int ruby);
    public OnSeasonRewardResult onSeasonRewardResult;
    #endregion

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_REQUEST_PVP_RANKING_ACK>(RCV_PACKET_CG_GAME_REQUEST_PVP_RANKING_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_DETAIL_USER_INFO_ACK>(RCV_PACKET_CG_READ_DETAIL_USER_INFO_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_RANK_REQUEST_GUILD_RANKING_ACK>(RCV_PACKET_CG_RANK_REQUEST_GUILD_RANKING_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_RANK_RECEIVE_DAILY_REWARD_ACK>(RCV_PACKET_CG_RANK_RECEIVE_DAILY_REWARD_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_RANK_RECEIVE_SEASON_REWARD_ACK>(RCV_PACKET_CG_RANK_RECEIVE_SEASON_REWARD_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_RANK_REWARD_INFO_ACK>(RCV_PACKET_CG_READ_RANK_REWARD_INFO_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_DETAIL_GUILD_INFO_ACK>(RCV_PACKET_CG_READ_DETAIL_GUILD_INFO_ACK);

        return base.OnCreate();
    }

    void UpdateRankerGuildInfoList(byte page, List<CRankerGuildInfo> rankerGuildInfoList)
    {
        int startIndex = 0, length = 0;
        if (rankerGuildInfoList != null && rankerGuildInfoList.Count > 0)
        {
            length = rankerGuildInfoList.Count;
            startIndex = (page - 1) * entry.data.GetValue<int>(Const_IndexID.Const_Ranking_List_Per_Page); //length;

            int index = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                if (i < m_RankerGuildInfoList.Count)
                {
                    m_RankerGuildInfoList[i] = rankerGuildInfoList[index];
                }
                else
                {
                    m_RankerGuildInfoList.Add(rankerGuildInfoList[index]);
                }

                index++;
            }
        }

        if (onRankerGuildInfoListUpdate != null)
        {
            onRankerGuildInfoListUpdate(page, m_RankerGuildInfoList, startIndex, length);
        }
    }

    void UpdateRankerInfoList(byte page, List<CRankerInfo> rankerInfoList)
    {
        int startIndex = 0, length = 0;
        if (rankerInfoList != null && rankerInfoList.Count > 0)
        {
            length = rankerInfoList.Count;
            startIndex = (page - 1) * entry.data.GetValue<int>(Const_IndexID.Const_Ranking_List_Per_Page); //length;

            int index = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                if (i < m_RankerInfoList.Count)
                {
                    m_RankerInfoList[i] = rankerInfoList[index];
                }
                else
                {
                    m_RankerInfoList.Add(rankerInfoList[index]);
                }

                index++;
            }
        }

        if (onRankerInfoListUpdate != null)
        {
            onRankerInfoListUpdate(page, m_RankerInfoList, startIndex, length);
        }
    }

    #region REQ
    public void REQ_PACKET_CG_RANK_REQUEST_PVP_RANKING_SYN(byte page, bool indication)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_RANK_REQUEST_PVP_RANKING_SYN()
        {
            m_byPage = page,
        }, indication);
    }

    public void REQ_PACKET_CG_READ_DETAIL_USER_INFO_SYN(long aid)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_DETAIL_USER_INFO_SYN()
        {
            m_TagetUserrAid = aid,
        });
    }

    public void REQ_PACKET_CG_RANK_REQUEST_GUILD_RANKING_SYN(byte page, bool indication)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_RANK_REQUEST_GUILD_RANKING_SYN()
        {
            m_byPage = page,
        }, indication);
    }

    public void REQ_PACKET_CG_READ_DETAIL_GUILD_INFO_SYN(long gid)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_DETAIL_GUILD_INFO_SYN()
        {
            m_Gid = gid,
        });
    }

    public void REQ_PACKET_CG_RANK_RECEIVE_DAILY_REWARD_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_RANK_RECEIVE_DAILY_REWARD_SYN());
    }

    public void REQ_PACKET_CG_RANK_RECEIVE_SEASON_REWARD_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_RANK_RECEIVE_SEASON_REWARD_SYN());
    }

    public void REQ_PACKET_CG_READ_RANK_REWARD_INFO_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_RANK_REWARD_INFO_SYN());
    }
    #endregion

    #region RCV
    void RCV_PACKET_CG_GAME_REQUEST_PVP_RANKING_ACK(PACKET_CG_GAME_REQUEST_PVP_RANKING_ACK packet)
    {
        rankingInfo = packet.m_MyRankInfo;
        UpdateRankerInfoList(packet.m_byPage, packet.m_RankerInfo);
    }

    void RCV_PACKET_CG_READ_DETAIL_USER_INFO_ACK(PACKET_CG_READ_DETAIL_USER_INFO_ACK packet)
    {
        if (onUserInfoResult != null)
        {
            onUserInfoResult(new UserInfo()
            {
                accountLevel = packet.m_byTargetUserLevel,
                aid = packet.m_TargetUserrAid,
                cardInfoList = packet.m_MainDeckCardInfoList,
                currentPvPArea = packet.m_byCurrentArea,
                guildEmblem = packet.m_sGuildEmblem,
                guildName = packet.m_sGuildName,
                guildSupportCardCount = packet.m_iGuildSupportCardCount,
                leaderCID = packet.m_MainDeckData.m_LeaderCid,
                rankingInfo = packet.m_UserRankInfo,
                rankingPoint = packet.m_iTargetRankingPoint,
                userName = packet.m_sTargetUserName,
            });
        }
    }

    void RCV_PACKET_CG_RANK_REQUEST_GUILD_RANKING_ACK(PACKET_CG_RANK_REQUEST_GUILD_RANKING_ACK packet)
    {
        var byPage = packet.m_byPage;
        if (packet.m_MyGuildInfo != null)
        {
            entry.guild.guildBase = packet.m_MyGuildInfo;
            entry.guild.totalRankedGuildCount = packet.m_MyGuildRankInfo.m_TotalRankedGuild;
            entry.guild.guildRanking = packet.m_MyGuildRankInfo.m_GuildRank;
            //entry.guild.guildRankingPoint = packet.m_MyGuildRankInfo.m_iRankingPoint;
        }

        UpdateRankerGuildInfoList(1, packet.m_RankerGuildInfo);
    }

    void RCV_PACKET_CG_READ_DETAIL_GUILD_INFO_ACK(PACKET_CG_READ_DETAIL_GUILD_INFO_ACK packet)
    {
        if (onGuildInfoResult != null)
        {
            onGuildInfoResult(packet.m_TargetGuildBase);
        }
    }

    void RCV_PACKET_CG_RANK_RECEIVE_DAILY_REWARD_ACK(PACKET_CG_RANK_RECEIVE_DAILY_REWARD_ACK packet)
    {
        entry.account.ruby = packet.m_iTotalRuby;

        if (onDailyRewardResult != null)
        {
            onDailyRewardResult(packet.m_iObtainRuby);
        }
    }

    void RCV_PACKET_CG_RANK_RECEIVE_SEASON_REWARD_ACK(PACKET_CG_RANK_RECEIVE_SEASON_REWARD_ACK packet)
    {
        entry.account.ruby = packet.m_iTotalRuby;

        if (onSeasonRewardResult != null)
        {
            onSeasonRewardResult(packet.m_iObtainRuby);
        }
    }

    void RCV_PACKET_CG_READ_RANK_REWARD_INFO_ACK(PACKET_CG_READ_RANK_REWARD_INFO_ACK packet)
    {
        seasonRewardTime = packet.m_iSeasonRewardTime;
        m_DailyRankReward = packet.m_DailyRewardInfo;
        m_SeasonRankReward = packet.m_SeasonRewardInfo;
    }
    #endregion
}
