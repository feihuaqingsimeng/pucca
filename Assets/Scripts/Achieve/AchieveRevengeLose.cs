using Common.Packet;
using Common.Util;

/// <summary>
/// 복수전 패배
/// </summary>
public class AchieveRevengeLose : AchieveBase
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
        if (Kernel.entry.revengeBattle.result == ePvpResult.Lose)
        {
            achieveAccumulate++;
        }
    }
}
