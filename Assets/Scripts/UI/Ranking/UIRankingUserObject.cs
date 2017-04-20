using Common.Packet;

public class UIRankingUserObject : UIRankingObject
{
    public UIMiniCharCard m_MiniCharCard;
    long m_AID;

    // Use this for initialization

    // Update is called once per frame

    public void SetRankerInfo(CRankerInfo rankerInfo)
    {
        if (rankerInfo != null)
        {
            m_AID = rankerInfo.m_AID;

            maxLevel = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit);

            rank = rankerInfo.m_iRank;
            level = rankerInfo.m_byLevel;
            name = rankerInfo.m_sUserName;
            rankingPoint = rankerInfo.m_iRankingPoint;
            m_MiniCharCard.SetCardInfo(rankerInfo.m_iLeaderCardIndex);
        }
    }

    protected override void OnClick()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.REQ_PACKET_CG_READ_DETAIL_USER_INFO_SYN(m_AID);
        }
    }
}
