using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Packet;

public class UIAdventureSweep : UIObject
{
    public  Button      SweepButton;
    public  Text        SweepTicketCount;


    protected override void Awake()
    {
        base.Awake();

        SweepButton.onClick.AddListener(PressSweepButton);
    }



    protected override void Update()
    {
        base.Update();

        SweepTicketCount.text = Languages.GetNumberComma(Kernel.entry.account.clearTicket);
    }



    public void PressSweepButton()
    {
        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, Kernel.entry.adventure.SelectStageIndex);

        if(Kernel.entry.account.heart < StageData.Need_Heart)
        {
            UINotificationCenter.Enqueue(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_TOKEN));
//            UIAlerter.Alert(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_TOKEN), UIAlerter.Composition.Confirm); //임시. 하트부족.
        }
        else if(Kernel.entry.account.clearTicket <= 0)
        {
            UINotificationCenter.Enqueue(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_CLEAR_TICKET));
        }
        else
        {
            Kernel.entry.adventure.onUseClearTicket += RecvSweep;
            Kernel.entry.adventure.REQ_PACKET_CG_GAME_USE_CLEAR_TICKET_SYN();
            Kernel.uiManager.Close(ui);
        }
    }



    public void RecvSweep(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet)
    {
        Kernel.entry.adventure.onUseClearTicket -= RecvSweep;
        Kernel.entry.account.heart = packet.m_iRemainHeart;
        Kernel.entry.battle.BattleStartLevel = Kernel.entry.account.level;
        UIAdventureResult pResultMng = (UIAdventureResult)Kernel.uiManager.Open(UI.AdventureResult);
        pResultMng.ShowResult_Sweep(packet);

        Kernel.soundManager.PlayUISound(SOUND.SND_UI_PVP_PVE_RESULT_CLEAR);
    }


}
