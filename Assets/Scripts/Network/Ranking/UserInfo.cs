using Common.Packet;
using System.Collections.Generic;

public class UserInfo
{
    public byte accountLevel
    {
        get;
        set;
    }

    public long aid
    {
        get;
        set;
    }

    public List<CCardInfo> cardInfoList
    {
        get;
        set;
    }

    public byte currentPvPArea
    {
        get;
        set;
    }

    public string guildEmblem
    {
        get;
        set;
    }

    public string guildName
    {
        get;
        set;
    }

    public int guildSupportCardCount
    {
        get;
        set;
    }

    public long leaderCID
    {
        get;
        set;
    }

    public CFranchiseRankingInfo rankingInfo
    {
        get;
        set;
    }

    public int rankingPoint
    {
        get;
        set;
    }

    public string userName
    {
        get;
        set;
    }
}
