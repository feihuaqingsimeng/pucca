using Common.Packet;

/// <summary>
/// 모험 클리어 (승/패 상관x)
/// </summary>
public class DailyAchievePvEComplete : DailyAchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.adventure.onPveResultDelegate += Listener;
            Kernel.entry.adventure.onUseClearTicket += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.adventure.onPveResultDelegate -= Listener;
            Kernel.entry.adventure.onUseClearTicket -= Listener;
        }
    }

    void Listener(PACKET_CG_GAME_PVE_RESULT_ACK packet)
    {
        //if (packet.m_bIsClear)
        {
            achieveAccumulate++;
        }
    }

    void Listener(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet)
    {
        achieveAccumulate++;
    }
}
