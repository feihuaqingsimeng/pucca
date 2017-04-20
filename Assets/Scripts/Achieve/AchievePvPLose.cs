using Common.Packet;

/// <summary>
/// 매칭패배
/// </summary>
public class AchievePvPLose : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.battle.onBattleResultDelegate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.battle.onBattleResultDelegate -= Listener;
        }
    }

    void Listener(PACKET_CG_GAME_PVP_RESULT_ACK packet)
    {
        if (packet.m_eResult == Common.Util.ePvpResult.Lose)
        {
            achieveAccumulate++;
        }
    }
}
