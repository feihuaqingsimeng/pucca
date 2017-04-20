using Common.Packet;

public class AchievePvECompleteArea : AchieveBase
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
        if (packet.m_bIsClear)
        {
            int lastStageArea = (packet.m_iLastStageIndex / 100);
            if (lastStageArea >= m_AchieveGoal)
            {
                achieveAccumulate = lastStageArea;
            }
        }
    }
}
