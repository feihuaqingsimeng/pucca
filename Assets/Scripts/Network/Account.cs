using Common.Packet;
using Common.Util;
using System;
using UnityEngine;

public class Account : Node
{
    public long userNo
    {
        get;
        private set;
    }

    public int dbNo
    {
        get;
        private set;
    }


    public int AuthKey
    {
        get;
        private set;
    }

    public eLoginType mainLoginType
    {
        get;
        set;
    }

    public eLoginType subLoginType
    {
        get;
        set;
    }

    public string mainLoginKey
    {
        get { return "mainLogintype"; }
    }

    public string subLoginKey
    {
        get { return "subLogintype"; }
    }

    public bool isNewUser
    {
        get;
        private set;
    }

    public bool isFirstLogIn
    {
        get;
        private set;
    }

    bool m_Initialized;
    CUserBase m_UserBase;
    CGoods m_Goods;
    CPromoteTicket m_PromoteTicket;
    CTreasureMap m_TreasureMap;

    int m_HeartRecoveryCycleSec;

    #region Delegates
    public delegate void OnInitialize();
    public OnInitialize onInitialize;

    public delegate void OnLogInResult(bool isNewUser, bool isFirstLogIn);
    public OnLogInResult onLogInResult;

    //** 계정 연동 결과
    public delegate void OnLinkLogInResult();
    public OnLinkLogInResult onLinkLogInResult;

    public delegate void OnDropOutAccountResult();
    public OnDropOutAccountResult onDropOutAccountResult;

    public delegate void OnCreateNicknameResult(string nickname);
    public OnCreateNicknameResult onCreateNicknameResult;

    public delegate void OnUserBaseUpdate(byte level, long exp, string name, byte currentPvPArea, int leaderCardIndex);
    public OnUserBaseUpdate onUserBaseUpdate;

    public delegate void OnGoodsUpdate(int friendship, int gold, int heart, int ranking, int ruby, int star, int guildPoint, int revengePoint, int smilePoint);
    public OnGoodsUpdate onGoodsUpdate;

    public delegate void OnPromoteTicketUpdate(int attacker, int buffer, int debuffer, int defender, int ranger, int armor, int accessory, int weapon);
    public OnPromoteTicketUpdate onPromoteTicketUpdate;

    public delegate void OnTreasureMapUpdate(int TerrapinMap, int CoconutMap, int IceMap, int LakeMap, int BlackMap);
    public OnTreasureMapUpdate onTreasureMapUpdate;

    public delegate void OnGoldUpdate(int gold, int updateGold);
    public OnGoldUpdate onGoldUpdate;

    public delegate void OnRubyUpdate(int ruby, int updateRuby);
    public OnRubyUpdate onRubyUpdate;

    public delegate void OnLevelUpdate(byte level);
    public OnLevelUpdate onLevelUpdate;

    public delegate void OnGetGameVersion(int verA, int verB, int verC, int verD);
    public OnGetGameVersion onGetGameVersion;


    #endregion

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_LOGIN_ACK>(RCV_PACKET_CG_AUTH_LOGIN_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_USER_BASE_ACK>(RCV_PACKET_CG_READ_USER_BASE_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_HEART_REFRESH_ACK>(RCV_PACKET_CG_GAME_HEART_REFRESH_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_CREATE_NICKNAME_ACK>(RCV_PACKET_CG_AUTH_CREATE_NICKNAME_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_ACCOUNT_LINK_ACK>(RCV_PACKET_CG_AUTH_ACCOUNT_LINK_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_LOG_OUT_ACK>(RCV_PACKET_CG_AUTH_LOG_OUT_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_WITHDRAW_ACCOUNT_ACK>(RCV_PACKET_CG_AUTH_WITHDRAW_ACCOUNT_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_AUTH_GET_CLIENT_VERSION_ACK>(RCV_PACKET_CG_AUTH_GET_CLIENT_VERSION_ACK);
        return base.OnCreate();
    }

    public override void Update()
    {
        if (!initialized)
        {
            return;
        }

        // Local timer for heart.
        if (heart < maxHeart)
        {
            if (m_HeartRecoveryCycleSec == 0)
            {
                m_HeartRecoveryCycleSec = entry.data.GetValue<int>(Const_IndexID.Const_Heart_Recovery_Cycle_Sec);
            }

            TimeSpan ts = TimeUtility.currentServerTime - TimeUtility.ToDateTime(heartLastUpdateTime);
            if (ts.TotalSeconds >= m_HeartRecoveryCycleSec)
            {
                heart++;
                REQ_PACKET_CG_GAME_HEART_REFRESH_SYN();
                heartLastUpdateTime = (int)TimeUtility.ToUnixEpoch(TimeUtility.currentServerTime);
            }
        }
        else
        {
            // Timer reset when heart is max.
            //heartLastUpdateTime = 0;
            heartLastUpdateTime = (int)TimeUtility.ToUnixEpoch(TimeUtility.currentServerTime);
        }
    }

    #region Properties
    public bool initialized
    {
        get
        {
            return m_Initialized;
        }

        set
        {
            if (m_Initialized != value)
            {
                m_Initialized = value;

                if (onInitialize != null)
                {
                    onInitialize();
                }
            }
        }
    }

    public int maxHeart
    {
        get
        {
            if (level > 1)
            {
                DB_AccountLevel.Schema accountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, level - 1);
                if (accountLevel != null)
                {
                    return accountLevel.Max_Heart;
                }
            }
            else
            {
                return entry.data.GetValue<int>(Const_IndexID.Const_Heart_Default_Limit);
            }

            return 0;
        }
    }

    #region CUserBase
    public byte level
    {
        get
        {
            if (m_UserBase != null)
            {
                return m_UserBase.m_byLevel;
            }

            return 0;
        }

        set
        {
            if (m_UserBase != null && m_UserBase.m_byLevel != value)
            {
                m_UserBase.m_byLevel = value;

                if (onLevelUpdate != null)
                {
                    onLevelUpdate(m_UserBase.m_byLevel);
                }

                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public int exp
    {
        get
        {
            return m_UserBase.m_iExp;
        }

        set
        {
            if (m_UserBase.m_iExp != value)
            {
                m_UserBase.m_iExp = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public string name
    {
        get
        {
            return m_UserBase.m_sNickName;
        }

        set
        {
            if (m_UserBase.m_sNickName != value)
            {
                m_UserBase.m_sNickName = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public byte prePVPArea
    {
        get;
        set;
    }

    public byte currentPvPArea
    {
        get
        {
            return m_UserBase.m_byCurrentPvPArea;
        }

        set
        {
            if (m_UserBase.m_byCurrentPvPArea != value)
            {
                m_UserBase.m_byCurrentPvPArea = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public string guildName
    {
        get
        {
            if (string.IsNullOrEmpty(m_UserBase.m_sGuildName))
            {
                return Languages.ToString(TEXT_UI.GUILD_NONE);
            }
            else
            {
                return m_UserBase.m_sGuildName;
            }

            /*return entry.guild.guildName;*/
        }

        set
        {
            if (m_UserBase != null && m_UserBase.m_sGuildName != value)
            {
                m_UserBase.m_sGuildName = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public string guildEmblem
    {
        get
        {
            return m_UserBase.m_sGuildEmblem;
        }

        set
        {
            if (m_UserBase != null && m_UserBase.m_sGuildEmblem != value)
            {
                m_UserBase.m_sGuildEmblem = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public long gid
    {
        get
        {
            return m_UserBase.m_Gid;
            /*return entry.guild.gid;*/
        }

        set
        {
            if (m_UserBase != null && m_UserBase.m_Gid != value)
            {
                m_UserBase.m_Gid = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public int leaderCardIndex
    {
        get
        {
            /*return m_UserBase.m_iLeaderCardIndex;*/
            CDeckData deckData = entry.character.FindMainDeckData();
            if (deckData != null)
            {
                CCardInfo cardInfo = entry.character.FindCardInfo(deckData.m_LeaderCid);
                if (cardInfo != null)
                {
                    return cardInfo.m_iCardIndex;
                }
            }

            return 0;
        }
    }

    public int lastStageIndex
    {
        get
        {
            if (m_UserBase != null)
            {
                return m_UserBase.m_iLastStageIndex;
            }

            return 0;
        }

        set
        {
            if (m_UserBase != null)
            {
                m_UserBase.m_iLastStageIndex = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public int lastLoginTime
    {
        get
        {
            if (m_UserBase != null)
            {
                return m_UserBase.m_iLastLoginTime;
            }

            return 0;
        }
    }

    public int supportCardCount
    {
        get
        {
            return (m_UserBase != null) ? m_UserBase.m_iSupportCardCount : 0;
        }

        set
        {
            if (m_UserBase != null)
            {
                m_UserBase.m_iSupportCardCount = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }


    public int TutorialGroup
    {
        get
        {
            return (m_UserBase != null) ? m_UserBase.m_iTutorialGroup : 0;
        }

        set
        {
            if (m_UserBase != null)
            {
                m_UserBase.m_iTutorialGroup = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }

    public int winningStreak
    {
        get
        {
            return m_UserBase.m_iWinningStreak;
        }

        set
        {
            if (m_UserBase.m_iWinningStreak != value)
            {
                m_UserBase.m_iWinningStreak = value;
                InvokeUserBaseUpdateCallback();
            }
        }
    }
    #endregion

    #region CGoods
    public int heartLastUpdateTime
    {
        get
        {
            return m_Goods.m_HeartLastUpdateTime;
        }

        set
        {
            if (m_Goods != null && m_Goods.m_HeartLastUpdateTime != value)
            {
                m_Goods.m_HeartLastUpdateTime = value;
            }
        }
    }

    public int friendshipPoint
    {
        get
        {
            return m_Goods.m_iFriendShipPoint;
        }

        set
        {
            if (m_Goods.m_iFriendShipPoint != value)
            {
                m_Goods.m_iFriendShipPoint = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int gold
    {
        get
        {
            return (m_Goods != null) ? m_Goods.m_iGold : 0;
        }

        set
        {
            if (m_Goods != null && m_Goods.m_iGold != value)
            {
                int updateGold = value - m_Goods.m_iGold;

                m_Goods.m_iGold = value;

                if (onGoldUpdate != null)
                {
                    onGoldUpdate(m_Goods.m_iGold, updateGold);
                }

                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int guildPoint
    {
        get
        {
            return (m_Goods != null) ? m_Goods.m_iGuildPoint : 0;
        }

        set
        {
            if (m_Goods != null && m_Goods.m_iGuildPoint != value)
            {
                m_Goods.m_iGuildPoint = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int heart
    {
        get
        {
            return (m_Goods != null) ? m_Goods.m_iHeart : 0;
        }

        set
        {
            if (m_Goods != null)
            {
                // Clamp
                /*
                if (value.CompareTo(0) < 0) value = 0;
                if (value.CompareTo(Settings.Battle.MaximumHeart) > 0) value = Settings.Battle.MaximumHeart;
                */
                if (m_Goods.m_iHeart != value)
                {
                    m_Goods.m_iHeart = value;
                    InvokeGoodsUpdateCallback();
                }
            }
        }
    }

    public int rankingPoint
    {
        get
        {
            return m_Goods.m_iRankingPoint;
        }

        set
        {
            if (m_Goods.m_iRankingPoint != value)
            {
                m_Goods.m_iRankingPoint = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int ruby
    {
        get
        {
            return (m_Goods != null) ? m_Goods.m_iRuby : 0;
        }

        set
        {
            if (m_Goods != null && m_Goods.m_iRuby != value)
            {
                int updateRuby = value - m_Goods.m_iRuby;

                m_Goods.m_iRuby = value;

                if (onRubyUpdate != null)
                {
                    onRubyUpdate(m_Goods.m_iRuby, updateRuby);
                }

                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int starPoint
    {
        get
        {
            return m_Goods.m_iStarPoint;
        }

        set
        {
            if (m_Goods.m_iStarPoint != value)
            {
                m_Goods.m_iStarPoint = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }


    public int clearTicket
    {
        get
        {
            return m_Goods.m_iClearTicketCount;
        }

        set
        {
            if (m_Goods.m_iClearTicketCount != value)
            {
                m_Goods.m_iClearTicketCount = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }


    public int revengePoint
    {
        get
        {
            return m_Goods.m_iRevengePoint;
        }

        set
        {
            if (m_Goods.m_iRevengePoint != value)
            {
                m_Goods.m_iRevengePoint = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int winPoint
    {
        get
        {
            return m_Goods.m_byWinPoint;
        }

        set
        {
            if (m_Goods.m_byWinPoint != value)
            {
                m_Goods.m_byWinPoint = (byte)value;
                InvokeGoodsUpdateCallback();
            }
        }
    }

    public int smilePoint
    {
        get
        {
            return m_Goods.m_iSmilePoint;
        }

        set
        {
            if (m_Goods.m_iSmilePoint != value)
            {
                m_Goods.m_iSmilePoint = value;
                InvokeGoodsUpdateCallback();
            }
        }
    }
    #endregion

    #region CPromoteTicket

    public CPromoteTicket promoteTicket
    {
        set
        {
            m_PromoteTicket = value;
            InvokePromoteTicketUpdateCallback();
        }
    }


    public int armorTicket
    {
        get
        {
            return m_PromoteTicket.m_iArmorTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iArmorTicket != value)
            {
                m_PromoteTicket.m_iArmorTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int attackerTicket
    {
        get
        {
            return m_PromoteTicket.m_iAttackerTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iAttackerTicket != value)
            {
                m_PromoteTicket.m_iAttackerTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int bufferTicket
    {
        get
        {
            return m_PromoteTicket.m_iBufferTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iBufferTicket != value)
            {
                m_PromoteTicket.m_iBufferTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int debufferTicket
    {
        get
        {
            return m_PromoteTicket.m_iDebufferTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iDebufferTicket != value)
            {
                m_PromoteTicket.m_iDebufferTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int defenderTicket
    {
        get
        {
            return m_PromoteTicket.m_iDefenderTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iDefenderTicket != value)
            {
                m_PromoteTicket.m_iDefenderTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int accessoryTicket
    {
        get
        {
            return m_PromoteTicket.m_iAccessoryTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iAccessoryTicket != value)
            {
                m_PromoteTicket.m_iAccessoryTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int rangerTicket
    {
        get
        {
            return m_PromoteTicket.m_iRangerTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iRangerTicket != value)
            {
                m_PromoteTicket.m_iRangerTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }

    public int weaponTicket
    {
        get
        {
            return m_PromoteTicket.m_iWeaponTicket;
        }

        set
        {
            if (m_PromoteTicket.m_iWeaponTicket != value)
            {
                m_PromoteTicket.m_iWeaponTicket = value;
                InvokePromoteTicketUpdateCallback();
            }
        }
    }
    #endregion

    #region C
    public CTreasureMap treasureMap
    {
        set
        {
            m_TreasureMap = value;
            InvokeTreasureMapUpdateCallback();
        }
    }

    public int detect_TerrapinMap
    {
        get
        {
            return m_TreasureMap.m_iTerrapinMap;
        }

        set
        {
            if (m_TreasureMap.m_iTerrapinMap != value)
            {
                m_TreasureMap.m_iTerrapinMap = value;
                InvokeTreasureMapUpdateCallback();
            }
        }
    }

    public int detect_CoconutMap
    {
        get
        {
            return m_TreasureMap.m_iCoconutMap;
        }

        set
        {
            if (m_TreasureMap.m_iCoconutMap != value)
            {
                m_TreasureMap.m_iCoconutMap = value;
                InvokeTreasureMapUpdateCallback();
            }
        }
    }

    public int detect_IceMap
    {
        get
        {
            return m_TreasureMap.m_iIceMap;
        }

        set
        {
            if (m_TreasureMap.m_iIceMap != value)
            {
                m_TreasureMap.m_iIceMap = value;
                InvokeTreasureMapUpdateCallback();
            }
        }
    }

    public int detect_LakeMap
    {
        get
        {
            return m_TreasureMap.m_iLakeMap;
        }

        set
        {
            if (m_TreasureMap.m_iLakeMap != value)
            {
                m_TreasureMap.m_iLakeMap = value;
                InvokeTreasureMapUpdateCallback();
            }
        }
    }

    public int detect_BlackMap
    {
        get
        {
            return m_TreasureMap.m_iBlackMap;
        }

        set
        {
            if (m_TreasureMap.m_iBlackMap != value)
            {
                m_TreasureMap.m_iBlackMap = value;
                InvokeTreasureMapUpdateCallback();
            }
        }
    }
    #endregion

    #endregion

    void InvokeUserBaseUpdateCallback()
    {
        if (onUserBaseUpdate != null)
        {
            onUserBaseUpdate(m_UserBase.m_byLevel,
                                     m_UserBase.m_iExp,
                                     m_UserBase.m_sNickName,
                                     m_UserBase.m_byCurrentPvPArea,
                                     m_UserBase.m_iLeaderCardIndex);
        }
    }

    void InvokeGoodsUpdateCallback()
    {
        if (onGoodsUpdate != null)
        {
            onGoodsUpdate(m_Goods.m_iFriendShipPoint,
                                  m_Goods.m_iGold,
                                  m_Goods.m_iHeart,
                                  m_Goods.m_iRankingPoint,
                                  m_Goods.m_iRuby,
                                  m_Goods.m_iStarPoint,
                                  m_Goods.m_iGuildPoint,
                                  m_Goods.m_iRevengePoint,
                                  m_Goods.m_iSmilePoint);
        }
    }

    void InvokePromoteTicketUpdateCallback()
    {
        if (onPromoteTicketUpdate != null)
        {
            onPromoteTicketUpdate(m_PromoteTicket.m_iAttackerTicket,
                                          m_PromoteTicket.m_iBufferTicket,
                                          m_PromoteTicket.m_iDefenderTicket,
                                          m_PromoteTicket.m_iDefenderTicket,
                                          m_PromoteTicket.m_iRangerTicket,
                                          m_PromoteTicket.m_iArmorTicket,
                                          m_PromoteTicket.m_iAccessoryTicket,
                                          m_PromoteTicket.m_iWeaponTicket);
        }
    }


    void InvokeTreasureMapUpdateCallback()
    {
        if (onTreasureMapUpdate != null)
        {
            onTreasureMapUpdate(m_TreasureMap.m_iTerrapinMap,
                                          m_TreasureMap.m_iCoconutMap,
                                          m_TreasureMap.m_iIceMap,
                                          m_TreasureMap.m_iLakeMap,
                                          m_TreasureMap.m_iBlackMap);
        }
    }


    public int GetValue(ClassType value)
    {
        switch (value)
        {
            case ClassType.ClassType_Healer:
                return bufferTicket;
            case ClassType.ClassType_Hitter:
                return attackerTicket;
            case ClassType.ClassType_Keeper:
                return defenderTicket;
            case ClassType.ClassType_Ranger:
                return rangerTicket;
            case ClassType.ClassType_Wizard:
                return debufferTicket;
        }

        return 0;
    }

    public int GetValue(eGoodsType goodsType)
    {
        DB_Goods.Schema goods = DB_Goods.Query(DB_Goods.Field.Index, goodsType);
        if (goods != null)
        {
            return GetValue(goods.Goods_Type);
        }

        LogError("{0}", goodsType);
        return 0;
    }

    public int GetValue(Goods_Type goodsType)
    {
        switch (goodsType)
        {
            case Goods_Type.AccountExp:
                return exp;
            case Goods_Type.EquipUpAccessory:
                return accessoryTicket;
            case Goods_Type.EquipUpArmor:
                return armorTicket;
            case Goods_Type.EquipUpWeapon:
                return weaponTicket;
            case Goods_Type.FriendPoint:
                return friendshipPoint;
            case Goods_Type.Gold:
                return gold;
            case Goods_Type.GuildExp:
                return (int)entry.guild.guildExp;
            case Goods_Type.GuildPoint:
                return guildPoint;
            case Goods_Type.Heart:
                return heart;
            case Goods_Type.RankingPoint:
                return rankingPoint;
            case Goods_Type.Ruby:
                return ruby;
            case Goods_Type.SkillUpHealer:
                return bufferTicket;
            case Goods_Type.SkillUpHitter:
                return attackerTicket;
            case Goods_Type.SkillUpKeeper:
                return defenderTicket;
            case Goods_Type.SkillUpRanger:
                return rangerTicket;
            case Goods_Type.SkillUpWizard:
                return debufferTicket;
            case Goods_Type.StarPoint:
                return starPoint;
            case Goods_Type.RevengePoint:
                return revengePoint;
            case Goods_Type.SmilePoint:
                return smilePoint;
            case Goods_Type.TreasureDetectMap_Terrapin:
                return detect_TerrapinMap;
            case Goods_Type.TreasureDetectMap_Coconut:
                return detect_CoconutMap;
            case Goods_Type.TreasureDetectMap_Ice:
                return detect_IceMap;
            case Goods_Type.TreasureDetectMap_Lake:
                return detect_LakeMap;
            case Goods_Type.TreasureDetectMap_Black:
                return detect_BlackMap;
            case Goods_Type.SweepTicket:
                return clearTicket;
            default:
                LogError("{0}", goodsType);
                break;
        }

        return 0;
    }

    public void SetValue(eGoodsType goodsType, int value)
    {
        DB_Goods.Schema goods = DB_Goods.Query(DB_Goods.Field.Index, (int)goodsType);
        if (goods != null)
        {
            SetValue(goods.Goods_Type, value);
        }
    }

    public void SetValue(Goods_Type goodsType, int value)
    {
        switch (goodsType)
        {
            case Goods_Type.AccountExp:
                exp = value;
                break;
            case Goods_Type.EquipUpAccessory:
                accessoryTicket = value;
                break;
            case Goods_Type.EquipUpArmor:
                armorTicket = value;
                break;
            case Goods_Type.EquipUpWeapon:
                weaponTicket = value;
                break;
            case Goods_Type.FriendPoint:
                friendshipPoint = value;
                break;
            case Goods_Type.Gold:
                gold = value;
                break;
            case Goods_Type.Heart:
                heart = value;
                break;
            case Goods_Type.Ruby:
                ruby = value;
                break;
            case Goods_Type.SkillUpHealer:
                bufferTicket = value;
                break;
            case Goods_Type.SkillUpHitter:
                attackerTicket = value;
                break;
            case Goods_Type.SkillUpKeeper:
                defenderTicket = value;
                break;
            case Goods_Type.SkillUpRanger:
                rangerTicket = value;
                break;
            case Goods_Type.SkillUpWizard:
                debufferTicket = value;
                break;
            case Goods_Type.StarPoint:
                starPoint = value;
                break;
            case Goods_Type.RevengePoint:
                revengePoint = value;
                break;
            case Goods_Type.SmilePoint:
                smilePoint = value;
                break;
            case Goods_Type.TreasureDetectMap_Terrapin:
                detect_TerrapinMap = value;
                break;
            case Goods_Type.TreasureDetectMap_Coconut:
                detect_CoconutMap = value;
                break;
            case Goods_Type.TreasureDetectMap_Ice:
                detect_IceMap = value;
                break;
            case Goods_Type.TreasureDetectMap_Lake:
                detect_LakeMap = value;
                break;
            case Goods_Type.TreasureDetectMap_Black:
                detect_BlackMap = value;
                break;
            case Goods_Type.SweepTicket:
                clearTicket = value;
                break;
        }
    }

    public void SetLevelWithoutUpdateCallback(byte level)
    {
        if (m_UserBase != null && m_UserBase.m_byLevel != level)
        {
            m_UserBase.m_byLevel = level;

            InvokeUserBaseUpdateCallback();
        }
    }

    #region REQ
    void REQ_ALL()
    {
        entry.data.REQ_PACKET_CG_READ_CONST_TABLE_LIST_SYN();
        REQ_PACKET_CG_READ_USER_BASE_SYN();
        entry.character.REQ_PACKET_CG_READ_CARD_LIST_SYN();
        entry.character.REQ_PACKET_CG_READ_DECK_LIST_SYN();
        entry.character.REQ_PACKET_CG_READ_SOUL_LIST_SYN();
        entry.treasure.REQ_PACKET_CG_READ_TREASURE_BOX_LIST_SYN();
        entry.achieve.REQ_PACKET_CG_READ_COMPLETE_ACHIEVE_LIST_SYN();
        entry.achieve.REQ_PACKET_CG_READ_DAILY_ACHIEVE_LIST_SYN();
        entry.strangeShop.REQ_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_SYN();
        entry.chest.REQ_PACKET_CG_READ_REWARD_BOX_LIST_SYN();
    }

    void REQ_PACKET_CG_READ_USER_BASE_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_USER_BASE_SYN()
        {
            m_AID = userNo,
            m_iDBIndex = dbNo,
            m_iAuthKey = AuthKey,
        });
    }

    public void REQ_PACKET_CG_AUTH_LOGIN_SYN(eLoginType loginType, string uid, string accessToken, string serverAuthCode)
    {
        Log("eLoginType : {0}\nUID : {1}\nAccess Token : {2}\nServer Auth Code : {3}",
            loginType,
            uid,
            accessToken,
            serverAuthCode);

        this.mainLoginType = loginType;

        PACKET_CG_AUTH_LOGIN_SYN protocol = new PACKET_CG_AUTH_LOGIN_SYN()
        {
            m_eLoginType = loginType,
            m_sUID = uid,
        };

        switch (loginType)
        {
            case eLoginType.Google:
                protocol.m_sAuthToken = serverAuthCode;
                break;
            default:
                protocol.m_sIDToken = accessToken;
                break;
        }

        Kernel.networkManager.WebRequest(protocol);
    }

    //** 로그아웃 요청
    public void REQ_PACKET_CG_AUTH_LOG_OUT_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_AUTH_LOG_OUT_SYN());
    }

    //** 계정 연동 요청 (게스트 -> 구글/페이스북/게임센터...)
    public void REQ_PACKET_CG_AUTH_ACCOUNT_LINK_SYN(eLoginType loginLinkType, string uid, string accessToken, string serverAuthCode)
    {
        Log("계정 연동 => {0}\nUID : {1}\nAccess Token : {2}\nServer Auth Code : {3}",
           loginLinkType,
           uid,
           accessToken,
           serverAuthCode);

        this.subLoginType = loginLinkType;

        PACKET_CG_AUTH_ACCOUNT_LINK_SYN protocol = new PACKET_CG_AUTH_ACCOUNT_LINK_SYN()
        {
            m_eLoginType = loginLinkType,
            m_sUID = uid,
        };

        switch (loginLinkType)
        {
            case eLoginType.Google:
                protocol.m_sAuthToken = serverAuthCode;
                break;
            default:
                protocol.m_sIDToken = accessToken;
                break;
        }

        Kernel.networkManager.WebRequest(protocol);
    }

    //** 계정 탈퇴 요청
    public void REQ_PACKET_CG_AUTH_WITHDRAW_ACCOUNT_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_AUTH_WITHDRAW_ACCOUNT_SYN());
    }

    void REQ_PACKET_CG_GAME_HEART_REFRESH_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_HEART_REFRESH_SYN(), false);
    }

    public void REQ_PACKET_CG_AUTH_CREATE_NICKNAME_SYN(string nickname)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_AUTH_CREATE_NICKNAME_SYN()
            {
                m_sNickName = nickname,
            });
    }
    #endregion

    #region RCV
    void RCV_PACKET_CG_READ_USER_BASE_ACK(PACKET_CG_READ_USER_BASE_ACK packet)
    {
        TimeUtility.serverTime = TimeUtility.ToDateTime(packet.m_UserBase.m_iServerTime);

        m_UserBase = packet.m_UserBase;
        InvokeUserBaseUpdateCallback();

        m_Goods = packet.m_UserGoods;
        InvokeGoodsUpdateCallback();

        m_PromoteTicket = packet.m_PromoteTicket;
        InvokePromoteTicketUpdateCallback();

        m_TreasureMap = packet.m_TreasureMap;
        InvokeTreasureMapUpdateCallback();

        Log("gid : {0}, name : {1}, lastLoginTime : {2}, serverTime : {3}, DatTime.Now : {4}, gap : {5}",
            gid, name, lastLoginTime, TimeUtility.currentServerTime, DateTime.Now, TimeUtility.gap);
    }

    void RCV_PACKET_CG_AUTH_LOGIN_ACK(PACKET_CG_AUTH_LOGIN_ACK packet)
    {
        Log("Client Packet Version : {0}, Server Packet Version : {1}", CPuccaPakcet.m_iPacketVersion, packet.m_iPacketVersion);
        if (CPuccaPakcet.m_iPacketVersion != packet.m_iPacketVersion)
        {
            // 임시
            NetworkEventHandler.OnNetworkException(0, string.Format("Packet version has mismatched. (Client packet version : {0}, Server packet version : {1})", CPuccaPakcet.m_iPacketVersion, packet.m_iPacketVersion));
        }
        else
        {
            string uid = Kernel.uid;

            if (packet.m_eMainAccountType != eLoginType.None)
            {
                mainLoginType = packet.m_eMainAccountType;
                PlayerPrefs.SetInt(mainLoginKey, (int)mainLoginType);
                Log("mainAccountType : {0}", packet.m_eMainAccountType);
            }

            if (packet.m_eSubAccountType != eLoginType.None)
            {
                subLoginType = packet.m_eSubAccountType;
                PlayerPrefs.SetInt(subLoginKey, (int)subLoginType);
                Log("subAccountType : {0}", packet.m_eSubAccountType);
            }

            Log("mainAccountType : {0}", packet.m_eMainAccountType);
            Log("subAccountType : {0}", packet.m_eSubAccountType);

            PlayerPrefs.SetString("uid", uid);
            PlayerPrefs.Save();
            Log("loginType : {0}, uid : {1}", mainLoginType, uid);

            userNo = packet.m_UserNo;
            dbNo = packet.m_iDBNo;
            AuthKey = packet.m_iAuthKey;
            Log("userNo : {0}, dbNo : {1}, AuthKey : {2}", userNo, dbNo, AuthKey);

            if (!packet.m_bIsNewUser)
            {
                REQ_ALL();
            }

            if (onLogInResult != null)
            {
                onLogInResult(packet.m_bIsNewUser, packet.m_bIsFirstLogin);
            }

            // 로그인 이후에 결제 시스템 초기화.
            if(Application.platform != RuntimePlatform.WindowsEditor)
                Kernel.iapManager.InitializePurchasing();
        }
    }

    //** 로그아웃 응답
    void RCV_PACKET_CG_AUTH_LOG_OUT_ACK(PACKET_CG_AUTH_LOG_OUT_ACK packet)
    {
        Log("mainLoginType : {0}", mainLoginType);
        switch (mainLoginType)
        {
            case eLoginType.Facebook:
                Kernel.facebookManager.LogOut();
                break;
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
            case eLoginType.Google:
                Kernel.gpgsManager.SignOut();
                break;
#endif
        }

        PlayerPrefs.SetInt(Kernel.entry.account.mainLoginKey, (int)Common.Util.eLoginType.None);

        if (Kernel.entry.account.subLoginType != Common.Util.eLoginType.None)
            PlayerPrefs.SetInt(Kernel.entry.account.subLoginKey, (int)Common.Util.eLoginType.None);

        PlayerPrefs.Save();

        Kernel.Reload();
    }

    //** 계정 연동 응답
    void RCV_PACKET_CG_AUTH_ACCOUNT_LINK_ACK(PACKET_CG_AUTH_ACCOUNT_LINK_ACK packet)
    {
        string uid = Kernel.uid;

        PlayerPrefs.SetInt(subLoginKey, (int)subLoginType);
        PlayerPrefs.Save();
        Log("계정 연동 완료 => loginType : {0}, uid : {1}", subLoginType, uid);

        if (onLinkLogInResult != null)
            onLinkLogInResult();
    }

    //** 계정 탈퇴 응답
    void RCV_PACKET_CG_AUTH_WITHDRAW_ACCOUNT_ACK(PACKET_CG_AUTH_WITHDRAW_ACCOUNT_ACK packet)
    {
        if (onDropOutAccountResult != null)
            onDropOutAccountResult();
    }

    void RCV_PACKET_CG_GAME_HEART_REFRESH_ACK(PACKET_CG_GAME_HEART_REFRESH_ACK packet)
    {
        heart = packet.m_iTotalHeart;
    }

    void RCV_PACKET_CG_AUTH_CREATE_NICKNAME_ACK(PACKET_CG_AUTH_CREATE_NICKNAME_ACK packet)
    {
        REQ_ALL();

        if (onCreateNicknameResult != null)
        {
            onCreateNicknameResult(packet.m_sNickName);
        }
    }






    //** 게임버전 체크.
    public void REQ_PACKET_CG_AUTH_GET_CLIENT_VERSION_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_AUTH_GET_CLIENT_VERSION_SYN());
    }

    void RCV_PACKET_CG_AUTH_GET_CLIENT_VERSION_ACK(PACKET_CG_AUTH_GET_CLIENT_VERSION_ACK packet)
    {
        if (onGetGameVersion != null)
        {
            onGetGameVersion(packet.m_ClientVer[0], packet.m_ClientVer[1], packet.m_ClientVer[2], packet.m_ClientVer[3]);
        }
    }





    #endregion
}
