using Common.Packet;
using System;
using System.Collections.Generic;

[Serializable]
public class BattleLog
{
    public byte         currentPvPArea;     //복수시 UI표시를 위해 필요.
    public string       GuildName;
    public string       GuildEmblem;
    public int          RankPoint;

    public CDeckData m_DeckData;
    public List<CCardInfo> m_CardInfoList;
}
