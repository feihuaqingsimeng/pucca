using Common.Packet;
using Common.Util;

/// <summary>
/// 복수전 승리
/// </summary>
public class AchieveRevengeWin : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.revengeBattle.onRevengeMatchResult += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.revengeBattle.onRevengeMatchResult -= Listener;
        }
    }

    void Listener(CReceivedGoods receivedGoods)
    {
        if (Kernel.entry.revengeBattle.result == ePvpResult.Win)
        {
            achieveAccumulate++;
        }
    }
}
