using Common.Packet;
using Common.Util;
using System.Collections.Generic;

public class Tutorial : Node
{
    public bool TutorialActive
    {
        get;
        set;
    }

    public TUTORIAL_TYPE CurTutorialType
    {
        get;
        set;
    }

    public int GroupNumber
    {
        get;
        set;
    }

    public int CurIndex
    {
        get;
        set;
    }

    public int WaitSeq
    {
        get;
        set;
    }

    public long CardInfo_CID
    {
        get;
        set;
    }


    public delegate void OnStartTutorial(int GroupNum);
    public OnStartTutorial onStartTutorial;

    public delegate void OnUpdateTutorialUI();
    public OnUpdateTutorialUI onUpdateTutorialUI;
    public delegate void OnResetLobbyUI();
    public OnResetLobbyUI onResetLobbyUI;

    public delegate void OnSetNextTutorial();
    public OnSetNextTutorial onSetNextTutorial;

    public delegate void OnSetNextTutorial_Delay();
    public OnSetNextTutorial_Delay onSetNextTutorial_Delay;


    public delegate void OnTutorialBattleDelegate();
    public OnTutorialBattleDelegate onTutorialBattleDelegate;

    public delegate void OnTutorialComplete();
    public OnTutorialComplete onTutorialComplete;



    public override Node OnCreate()
    {
        TutorialActive = false;
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_GAME_COMPLETE_TUTORIAL_ACK>(RCV_PACKET_CG_GAME_COMPLETE_TUTORIAL_ACK);

        return base.OnCreate();
    }



    //결과.   //다음 진행할 그룹번호 보내기.
    public void REQ_PACKET_CG_GAME_COMPLETE_TUTORIAL_SYN(int GroupNum)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_GAME_COMPLETE_TUTORIAL_SYN()
        {
            m_iTutorialGroup = GroupNum,
        });
    }


    public void RCV_PACKET_CG_GAME_COMPLETE_TUTORIAL_ACK(PACKET_CG_GAME_COMPLETE_TUTORIAL_ACK packet)
    {
        Kernel.entry.account.TutorialGroup = packet.m_iTutorialGroup;

        if (onTutorialComplete != null)
            onTutorialComplete();
    }




}
