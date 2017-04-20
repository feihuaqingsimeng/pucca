using Common.Packet;
using Common.Util;

public class Administrator : Node
{

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_GOODS_ACK>(RCV_PACKET_CG_GAME_CHEAT_GOODS_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_REWARD_BOX_ACK>(RCV_PACKET_CG_GAME_CHEAT_REWARD_BOX_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_CARD_ACK>(RCV_PACKET_CG_GAME_CHEAT_CARD_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_ACHIEVE_ACK>(RCV_PACKET_CG_GAME_CHEAT_ACHIEVE_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_ACK>(RCV_PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_ACK>(RCV_PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_CHEAT_SOUL_ACK>(RCV_PACKET_CG_GAME_CHEAT_SOUL_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_GM_ADD_GOODS_ACK>(RCV_PACKET_CG_GAME_GM_ADD_GOODS_ACK);

        return base.OnCreate();
    }

    #region REQ
    public void REQ_PACKET_CG_GAME_CHEAT_GOODS_SYN(eGoodsType goodsType, int amount)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_GOODS_SYN()
            {
                m_eGoodsType = goodsType,
                m_iAmount = amount,
            });
    }

    public void REQ_PACKET_CG_GAME_CHEAT_REWARD_BOX_SYN(int boxIndex, byte area)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_REWARD_BOX_SYN()
        {
            m_iBoxIndex = boxIndex,
            m_byArea = area,
        });
    }

    public void REQ_PACKET_CG_GAME_CHEAT_CARD_SYN(int cardIndex)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_CARD_SYN()
            {
                m_iCardIndex = cardIndex,
            });
    }

    public void REQ_PACKET_CG_GAME_CHEAT_ACHIEVE_SYN(bool isDaily, int achieveIndex, int achieveAccumulate)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_ACHIEVE_SYN()
        {
            m_bIsDaily = isDaily,
            m_iAchieveIndex = achieveIndex,
            m_iAchieveAddAmount = achieveAccumulate,
        });
    }

    public void REQ_PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_SYN(long targetAID)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_SYN()
            {
                m_TargetAid = targetAID,
            });
    }

    public void REQ_PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_SYN(byte level)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_SYN()
            {
                m_byLevel = level,
            });
    }

    public void REQ_PACKET_CG_GAME_CHEAT_SOUL_SYN(int soulIndex, int soulCount)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_CHEAT_SOUL_SYN()
        {
            m_iSoulIndex = soulIndex,
            m_iAddSoulCount = soulCount,
        });
    }

    public void REQ_PACKET_CG_GAME_GM_ADD_GOODS_SYN(string title, string message, ePostType goodsType, int achieveIndex, int amount)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_GM_ADD_GOODS_SYN()
        {
            m_ePostType = goodsType,
            m_iAchieveIndex = achieveIndex,
            m_iAchieveAddAmount = amount,
            m_sTitle = title,
            m_sMessage = message
        });
    }

    #endregion

    #region RCV
    void RCV_PACKET_CG_GAME_CHEAT_GOODS_ACK(PACKET_CG_GAME_CHEAT_GOODS_ACK packet)
    {
        switch (packet.m_eGoodsType)
        {
            case eGoodsType.EquipUpAccessory:
                entry.account.accessoryTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.EquipUpArmor:
                entry.account.armorTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.EquipUpWeapon:
                entry.account.weaponTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.FriendShip:
                entry.account.friendshipPoint = packet.m_iResultAmount;
                break;
            case eGoodsType.Heart:
                entry.account.heart = packet.m_iResultAmount;
                break;
            case eGoodsType.RankingPoint:
                entry.account.rankingPoint = packet.m_iResultAmount;
                break;
            case eGoodsType.Ruby:
                entry.account.ruby = packet.m_iResultAmount;
                break;
            case eGoodsType.SkillUpHealer:
                entry.account.bufferTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.SkillUpHitter:
                entry.account.attackerTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.SkillUpKeeper:
                entry.account.defenderTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.SkillUpRanger:
                entry.account.rangerTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.SkillUpWizard:
                entry.account.debufferTicket = packet.m_iResultAmount;
                break;
            case eGoodsType.StarPoint:
                entry.account.starPoint = packet.m_iResultAmount;
                break;
            case eGoodsType.Gold:
                entry.account.gold = packet.m_iResultAmount;
                break;
            case eGoodsType.GuildPoint:
                entry.account.guildPoint = packet.m_iResultAmount;
                break;
            case eGoodsType.SmilePoint:
                entry.account.smilePoint = packet.m_iResultAmount;
                break;
            case eGoodsType.TreasureDetectMapTerrapin:
                entry.account.detect_TerrapinMap = packet.m_iResultAmount;
                break;
            case eGoodsType.TreasureDetectMapCoconut:
                entry.account.detect_CoconutMap = packet.m_iResultAmount;
                break;
            case eGoodsType.TreasureDetectMapIce:
                entry.account.detect_IceMap = packet.m_iResultAmount;
                break;
            case eGoodsType.TreasureDetectMapLake:
                entry.account.detect_LakeMap = packet.m_iResultAmount;
                break;
            case eGoodsType.TreasureDetectMapBlack:
                entry.account.detect_BlackMap = packet.m_iResultAmount;
                break;

        }
    }

    void RCV_PACKET_CG_GAME_CHEAT_REWARD_BOX_ACK(PACKET_CG_GAME_CHEAT_REWARD_BOX_ACK packet)
    {
        entry.chest.AddRewardBox(packet.m_RewardBox);
    }

    void RCV_PACKET_CG_GAME_CHEAT_CARD_ACK(PACKET_CG_GAME_CHEAT_CARD_ACK packet)
    {
        entry.character.cardInfoList = packet.m_CardList;
    }

    void RCV_PACKET_CG_GAME_CHEAT_ACHIEVE_ACK(PACKET_CG_GAME_CHEAT_ACHIEVE_ACK packet)
    {
        if (packet.m_bIsDaily)
        {
            entry.achieve.UpdateDailyAchieve(packet.m_iAchieveIndex, packet.m_iResultAmount);
        }
        else
        {
            DB_AchieveList.Schema achieveList = DB_AchieveList.Query(DB_AchieveList.Field.Index, packet.m_iAchieveIndex);
            if (achieveList != null)
            {
                byte achieveCompleteStep = 0;
                CAchieve achieve = entry.achieve.FindAchieve(achieveList.Achieve_Group);
                if (achieve != null)
                {
                    achieveCompleteStep = achieve.m_byCompleteStep;
                }
                else LogError("CAchieve could not be found. (achieveGroup : {0})", achieveList.Achieve_Group);

                entry.achieve.UpdateAchieve(achieveList.Achieve_Group, packet.m_iResultAmount, achieveCompleteStep);
            }
        }
    }

    void RCV_PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_ACK(PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_ACK packet)
    {
        UnityEngine.PlayerPrefs.DeleteKey(Kernel.entry.account.mainLoginKey);
        UnityEngine.PlayerPrefs.DeleteKey(Kernel.entry.account.subLoginKey);
        UnityEngine.PlayerPrefs.DeleteKey("uid");
        UnityEngine.PlayerPrefs.Save();

        if (UnityEngine.Application.isEditor && UnityEngine.Application.isPlaying)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }

    void RCV_PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_ACK(PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_ACK packet)
    {
        entry.account.level = packet.m_byLevel;
    }

    void RCV_PACKET_CG_GAME_CHEAT_SOUL_ACK(PACKET_CG_GAME_CHEAT_SOUL_ACK packet)
    {
        for (int i = 0; i < packet.m_SoulList.Count; i++)
        {
            CSoulInfo soulInfo = packet.m_SoulList[i];
            if (soulInfo != null)
            {
                entry.character.UpdateSoulInfo(packet.m_SoulList[i]);
            }
        }
    }

    void RCV_PACKET_CG_GAME_GM_ADD_GOODS_ACK(PACKET_CG_GAME_GM_ADD_GOODS_ACK packet)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog("GM Command", "보상 처리가 완료되었습니다.", "확인");
#endif
    }

    #endregion
}
