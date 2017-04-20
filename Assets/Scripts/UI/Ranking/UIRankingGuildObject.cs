using Common.Packet;
using System.Collections.Generic;
using UnityEngine;

public class UIRankingGuildObject : UIRankingObject
{
    public UIGuildFlag m_GuildFlag;
    long m_GID;

    // Use this for initialization

    // Update is called once per frame

    public void SetRankerGuildInfo(CRankerGuildInfo rankerGuildInfo)
    {
        if (rankerGuildInfo != null)
        {
            m_GID = rankerGuildInfo.m_Gid;

            // 길드 Max 레벨 세팅
            maxLevel = (byte)Kernel.entry.guild.MaxLevel;
            rank = (int)rankerGuildInfo.m_GuildRank;
            level = rankerGuildInfo.m_byGuildLevel;
            name = rankerGuildInfo.m_sGuildName;
            rankingPoint = rankerGuildInfo.m_iRankingPoint;
            // ref. PUC-604, 임시 처리
            //m_GuildFlag.guildEmblem = (rankerGuildInfo.m_Gid != Kernel.entry.guild.gid) ? string.Empty : rankerGuildInfo.m_sGuildEmblem; // rankerGuildInfo.m_sGuildEmblem;
            m_GuildFlag.SetGuildEmblem(rankerGuildInfo.m_sGuildEmblem);
        }
    }

    protected override void OnClick()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.REQ_PACKET_CG_READ_DETAIL_GUILD_INFO_SYN(m_GID);
        }
    }
}
