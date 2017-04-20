using Common.Packet;
using Common.Util;
using System.Collections.Generic;

public class Chest : Node
{
    List<CRewardBox> m_RewardBoxList = new List<CRewardBox>();

    public List<CRewardBox> rewardBoxList
    {
        get
        {
            return m_RewardBoxList;
        }

        private set
        {
            if (m_RewardBoxList != value)
            {
                m_RewardBoxList = value;

                if (onChestListUpdate != null)
                {
                    onChestListUpdate(m_RewardBoxList);
                }
            }
        }
    }

    public delegate void OnChestOpen(int boxIndex, int gold, List<CBoxResult> boxResultList);
    public OnChestOpen onChestOpen;

    public delegate void OnChestListUpdate(List<CRewardBox> rewardBoxList);
    public OnChestListUpdate onChestListUpdate;

    public delegate void OnChestAdd(int boxOrder, int boxIndex, int obtainArea);
    public OnChestAdd onChestAdd;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_REWARD_BOX_LIST_ACK>(RCV_PACKET_CG_READ_REWARD_BOX_LIST_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_OPEN_REWARD_BOX_ACK>(RCV_PACKET_CG_GAME_OPEN_REWARD_BOX_ACK);

        return base.OnCreate();
    }

    public void UpdateRewardBoxList(List<CRewardBox> rewardBoxList)
    {
        if (rewardBoxList != null && rewardBoxList.Count > 0)
        {
            for (int i = 0; i < rewardBoxList.Count; i++)
            {
                CRewardBox rewardBox = rewardBoxList[i];
                if (rewardBox != null)
                {
                    CRewardBox tempRewardBox = m_RewardBoxList.Find(item => item.m_byBoxOrder == rewardBox.m_byBoxOrder);
                    if (tempRewardBox != null)
                    {
                        if (rewardBox.m_iBoxIndex != 0 && tempRewardBox.m_iBoxIndex == 0)
                        {
                            if (onChestAdd != null)
                            {
                                onChestAdd(rewardBox.m_byBoxOrder, rewardBox.m_iBoxIndex, rewardBox.m_byObtainArea);
                            }
                        }
                    }
                    else
                    {
                        LogError("CRewardBox could not be found by boxOrder ({0}).", rewardBox.m_byBoxOrder);
                    }
                }
            }
        }

        this.rewardBoxList = rewardBoxList;
    }

    public bool isSlotFull
    {
        get
        {
            for (int i = 0; i < m_RewardBoxList.Count; i++)
            {
                if (int.Equals(m_RewardBoxList[i].m_iBoxIndex, 0))
                {
                    return false;
                }
            }

            return true;
        }
    }

    void RemoveRewardBox(long sequence)
    {
        if (m_RewardBoxList != null && m_RewardBoxList.Count > 0)
        {
            CRewardBox rewardBox = m_RewardBoxList.Find(item => Equals(item.m_Sequence, sequence));
            if (rewardBox != null)
            {
                rewardBox.m_byObtainArea = 0;
                rewardBox.m_iBoxIndex = 0;
                rewardBox.m_Sequence = 0;

                if (onChestListUpdate != null)
                {
                    onChestListUpdate(m_RewardBoxList);
                }
            }
        }
    }

    public void AddRewardBox(CRewardBox value)
    {
        if (value != null && m_RewardBoxList != null)
        {
            CRewardBox rewardBox = m_RewardBoxList.Find(item => Equals(item.m_byBoxOrder, value.m_byBoxOrder));
            if (rewardBox != null)
            {
                rewardBox.m_byObtainArea = value.m_byObtainArea;
                rewardBox.m_iBoxIndex = value.m_iBoxIndex;
                rewardBox.m_Sequence = value.m_Sequence;
            }

            if (onChestListUpdate != null)
            {
                onChestListUpdate(m_RewardBoxList);
            }
        }
    }

    public CRewardBox FindRewardBox(long sequence)
    {
        return (m_RewardBoxList != null && m_RewardBoxList.Count > 0) ? m_RewardBoxList.Find(item => Equals(item.m_Sequence, sequence)) : null;
    }

    #region REQ
    public void REQ_PACKET_CG_READ_REWARD_BOX_LIST_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_REWARD_BOX_LIST_SYN());
    }

    public void REQ_PACKET_CG_GAME_OPEN_REWARD_BOX_SYN(long sequence, eGoodsType goodsType)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_OPEN_REWARD_BOX_SYN()
        {
            m_BoxSequence = sequence,
            m_ePayType = goodsType,
        });
    }
    #endregion

    #region RCV
    void RCV_PACKET_CG_READ_REWARD_BOX_LIST_ACK(PACKET_CG_READ_REWARD_BOX_LIST_ACK packet)
    {
        rewardBoxList = packet.m_RewardBoxList;

        // RCV_PACKET_CG_AUTH_LOGIN_ACK() 함수에서 마지막 REQ가 변경되면 아래 코드의 위치도 변경되어야 합니다.
        if (!entry.account.initialized)
        {
            entry.account.initialized = true;
        }
    }

    void RCV_PACKET_CG_GAME_OPEN_REWARD_BOX_ACK(PACKET_CG_GAME_OPEN_REWARD_BOX_ACK packet)
    {
        var boxList = packet.m_BoxList;

        int boxIndex = 0;
        CRewardBox rewardBox = FindRewardBox(packet.m_BoxSequence);
        if (rewardBox != null)
        {
            boxIndex = rewardBox.m_iBoxIndex;
        }

        RemoveRewardBox(packet.m_BoxSequence);

        entry.account.ruby = packet.m_iRemainRuby;
        entry.account.starPoint = packet.m_iRemainStar;
        entry.account.gold = packet.m_iTotalGold;

        for (int i = 0; i < packet.m_CardList.Count; i++)
        {
            entry.character.UpdateCardInfo(packet.m_CardList[i]);
        }

        for (int i = 0; i < packet.m_SoulList.Count; i++)
        {
            entry.character.UpdateSoulInfo(packet.m_SoulList[i]);
        }

        if (onChestOpen != null)
        {
            onChestOpen(boxIndex, packet.m_iEarnGold, packet.m_BoxResultList);
        }
    }
    #endregion
}
