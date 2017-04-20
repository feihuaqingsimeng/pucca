using Common.Packet;

/// <summary>
/// 몬스터처치
/// </summary>
public class AchievePvEKillMob : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.adventure.onPveResultDelegate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.adventure.onPveResultDelegate -= Listener;
        }
    }

    void Listener(PACKET_CG_GAME_PVE_RESULT_ACK packet)
    {
        if(packet.m_bIsClear)
            achieveAccumulate = m_AchieveAccumulate + Kernel.entry.adventure.lastKillCount;
    }
}
