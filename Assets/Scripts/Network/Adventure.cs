using Common.Packet;
using Common.Util;
using System.Collections.Generic;

public class Adventure : Node
{
    public int PreSelectStageIndex
    {
        get;
        set;
    }

    public long BattleSequence
    {
        get;
        set;
    }

    public int SelectStageIndex
    {
        get;
        set;
    }

    public int SelectAreaIndex
    {
        get;
        set;
    }
    
    public byte lastKillCount
    {
        get;
        set;
    }

    public delegate void OnStartPVE_Battle();
    public OnStartPVE_Battle onStartPVE_Battle;

    public delegate void OnPveResultDelegate(PACKET_CG_GAME_PVE_RESULT_ACK packet);
    public OnPveResultDelegate onPveResultDelegate;

    public delegate void OnUseClearTicket(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet);
    public OnUseClearTicket onUseClearTicket;


    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_START_PVE_ACK>(RCV_PACKET_CG_GAME_START_PVE_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_PVE_RESULT_ACK>(RCV_PACKET_CG_GAME_PVE_RESULT_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_USE_CLEAR_TICKET_ACK>(RCV_PACKET_CG_GAME_USE_CLEAR_TICKET_ACK);

        return base.OnCreate();
    }





    //랭킹_PVP
    public void REQ_PACKET_CG_GAME_START_PVE_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_START_PVE_SYN()
        {
            m_iStageIndex = SelectStageIndex
        });
    }


    public void RCV_PACKET_CG_GAME_START_PVE_ACK(PACKET_CG_GAME_START_PVE_ACK packet)
    {
        Kernel.entry.account.heart = packet.m_iRemainHeart;
        BattleSequence = packet.m_Sequence;

        if (onStartPVE_Battle != null)
        {
            onStartPVE_Battle();
        }
    }










    //랭킹_PVP_결과.
    public void REQ_PACKET_CG_GAME_PVE_RESULT_SYN(bool IsClear)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_PVE_RESULT_SYN()
        {
            m_Sequence = BattleSequence,
            m_bIsClear = IsClear,
            m_iStageIndex = SelectStageIndex,
            m_byKillCount = lastKillCount,
        });
    }


    public void RCV_PACKET_CG_GAME_PVE_RESULT_ACK(PACKET_CG_GAME_PVE_RESULT_ACK packet)
    {
        if (onPveResultDelegate != null)
        {
            onPveResultDelegate(packet);
        }
    }
















    //소탕권 사용.
    public void REQ_PACKET_CG_GAME_USE_CLEAR_TICKET_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_USE_CLEAR_TICKET_SYN()
        {
            m_iStageIndex = SelectStageIndex
        });
    }


    public void RCV_PACKET_CG_GAME_USE_CLEAR_TICKET_ACK(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet)
    {
        if (onUseClearTicket != null)
        {
            onUseClearTicket(packet);
        }
    }


}
