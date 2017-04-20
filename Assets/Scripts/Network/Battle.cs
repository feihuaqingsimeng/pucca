using Common.Packet;
using Common.Util;
using System.Collections.Generic;

public class Battle : Node
{
    public COpponentInfo    PVP_User
    {
        get;
        private set;
    }

    public CDeckData        PVP_Deck
    {
        get;
        private set;
    }

    public List<CCardInfo> PVP_CardInfo
    {
        get;
        private set;
    }

    public long MatchSequence
    {
        get;
        private set;
    }

    public int fluctuationPvpArea
    {
        get;
        set;
    }





    public BATTLE_KIND CurBattleKind
    {
        get;
        set;
    }

    public CRevengeMatchInfo RevengeMatchInfoData
    {
        get;
        set;
    }


    public bool AutoPlayBattle
    {
        get;
        set;
    }



    public int BattleStartLevel     //전투 시작시의 레벨.
    {
        get;
        set;
    }


    public string   AssetBundleURL_PVP_Field;
    public int      AssetBundleVer_PVP_Field;
    public string   AssetBundleURL_PVE_Field;
    public int      AssetBundleVer_PVE_Field;




    public delegate void OnLoadBattleScene();
    public OnLoadBattleScene onLoadBattleScene;

    public delegate void OnBattleResultDelegate(PACKET_CG_GAME_PVP_RESULT_ACK packet);
    public OnBattleResultDelegate onBattleResultDelegate;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_START_PVP_MATCHING_ACK>(RCV_PACKET_CG_GAME_START_PVP_MATCHING_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_PVP_RESULT_ACK>(RCV_PACKET_CG_GAME_PVP_RESULT_ACK);


        return base.OnCreate();
    }





    //매칭.
    public void REQ_PACKET_CG_GAME_START_PVP_MATCHING_SYN()
    {
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.account.TutorialGroup == 10)   //튜토리얼중일땐 안보냄.
        {
            MatchSequence = 0;
            
            //유저정보 제작.
            COpponentInfo PVP_User_Info = new COpponentInfo();
            PVP_User_Info.m_AID = 0;
            PVP_User_Info.m_byGuildLevel = 1;
            PVP_User_Info.m_byLevel = 1;
            PVP_User_Info.m_byPvpArea = 1;
            PVP_User_Info.m_iRankingPoint = 0;
            PVP_User_Info.m_sGuildEmble = string.Empty;
            PVP_User_Info.m_sGuildName = string.Empty;
            PVP_User_Info.m_sUserName = Languages.ToString(TEXT_UI.MASTER_SOO_FOLLOWER);
            PVP_User = PVP_User_Info;

            
            CDeckData PVP_Deck_Data = new CDeckData();
            PVP_Deck_Data.m_bIsMainDeck = true;
            PVP_Deck_Data.m_CardCidList = new List<long>();
            for(int idx = 0; idx < 5; idx++)
            {
                PVP_Deck_Data.m_CardCidList.Add(idx + 1);
            }
            PVP_Deck_Data.m_iDeckNum = 0;
            PVP_Deck_Data.m_LeaderCid = 1;
            PVP_Deck_Data.m_Sequence = 0;
            PVP_Deck = PVP_Deck_Data;

            int[] CardList = new int[5]{37, 43, 28, 29, 30};
            List<CCardInfo> PVP_CardInfo_List = new List<CCardInfo>();
            for (int idx = 0; idx < 5; idx++)
            {
                CCardInfo tempInfo = new CCardInfo();
                tempInfo.m_bIsNew = false;
                tempInfo.m_byAccessoryLV = 1;
                tempInfo.m_byArmorLV = 1;
                tempInfo.m_byLevel = 1;
                tempInfo.m_bySkill = 1;
                tempInfo.m_byWeaponLV = 1;
                tempInfo.m_Cid = idx+1;
                tempInfo.m_iBattlePower = 0;
                tempInfo.m_iCardIndex = CardList[idx];
                PVP_CardInfo_List.Add(tempInfo);
            }
            PVP_CardInfo = PVP_CardInfo_List;
            
            if (onLoadBattleScene != null)
                onLoadBattleScene();
        }
        else
            Kernel.networkManager.WebRequest(new PACKET_CG_GAME_START_PVP_MATCHING_SYN());
    }

    public void RCV_PACKET_CG_GAME_START_PVP_MATCHING_ACK(PACKET_CG_GAME_START_PVP_MATCHING_ACK packet)
    {
        Log("{0}", packet.Result);

        Kernel.entry.account.heart = packet.m_iRemainHeart;
        MatchSequence = packet.m_MatchSequence;
        PVP_User = packet.m_OpponentInfo;
        PVP_Deck = packet.m_OpponentDeckInfo;
        PVP_CardInfo = packet.m_CardInfo;

        if (onLoadBattleScene != null)
        {
            onLoadBattleScene();
        }
    }






    //결과.
    public void REQ_PACKET_CG_GAME_PVP_RESULT_SYN(ePvpResult eResult, int BattleTime, int ResultStarPoint, string RevengeDeckString)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_PVP_RESULT_SYN()
        {
            m_eResult = eResult,
            m_MatchSequence = MatchSequence,
            m_iBattleTime = BattleTime,
            m_iResultStarPoint = ResultStarPoint,
            m_sDeckInfo = RevengeDeckString
        });
    }


    public void RCV_PACKET_CG_GAME_PVP_RESULT_ACK(PACKET_CG_GAME_PVP_RESULT_ACK packet)
    {
        Kernel.entry.account.prePVPArea = Kernel.entry.account.currentPvPArea;
        Kernel.entry.account.currentPvPArea = (byte)packet.m_iCurrentPvpArea;
        Kernel.entry.battle.fluctuationPvpArea = packet.m_iFluctuationPvpArea;
        Kernel.entry.account.starPoint = packet.m_iTotalStarPoint;
        Kernel.entry.account.rankingPoint = packet.m_iResultRankingPoint;

        if (onBattleResultDelegate != null)
        {
            onBattleResultDelegate(packet);
        }

//미처리.
//        public CRewardBox m_RewardBox;
//        public int m_iFluctuationPvpArea;
//        public int m_iCurrentPvpArea;
    }
}
