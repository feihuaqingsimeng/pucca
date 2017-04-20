using Common.Packet;

/// <summary>
/// 스테이지 전체 클리어 02
/// </summary>
public class AchievePvEAllStageClearHard : AchieveBase
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
            DB_StagePVE.Schema pveStageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, packet.m_iLastStageIndex);

            if (pveStageData == null)
                return;

            int lastStageNum    = pveStageData.Index / 100;
            int nextStageNum    = pveStageData.NextStageID / 100;

            // last와 next가 같은 num이면 스테이지의 마지막이 아님.
            if (lastStageNum == nextStageNum)
                return;

            achieveAccumulate = lastStageNum;
        }
    }
}
