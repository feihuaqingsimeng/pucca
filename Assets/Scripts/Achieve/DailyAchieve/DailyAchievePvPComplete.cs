using Common.Packet;

/// <summary>
/// 매칭 클리어 (승/패 상관X)
/// </summary>
public class DailyAchievePvPComplete : DailyAchieveBase
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
        achieveAccumulate++;
    }
}
