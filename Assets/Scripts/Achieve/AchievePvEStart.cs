using Common.Packet;

/// <summary>
/// 모험참여
/// </summary>
public class AchievePvEStart : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.adventure.onStartPVE_Battle += Listener;
            Kernel.entry.adventure.onUseClearTicket += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.adventure.onStartPVE_Battle -= Listener;
            Kernel.entry.adventure.onUseClearTicket -= Listener;
        }
    }

    void Listener()
    {
        achieveAccumulate++;
    }

    void Listener(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet)
    {
        achieveAccumulate++;
    }
}
