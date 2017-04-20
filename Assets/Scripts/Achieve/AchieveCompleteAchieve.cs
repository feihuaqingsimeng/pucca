using Common.Packet;

/// <summary>
/// 업적달성
/// </summary>
public class AchieveCompleteAchieve : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.achieve.onCompleteAchieveResult += Listener;
            Kernel.entry.achieve.onCompleteDailyAchieveResult += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.achieve.onCompleteAchieveResult -= Listener;
            Kernel.entry.achieve.onCompleteDailyAchieveResult -= Listener;
        }
    }

    void Listener(int achieveGroup, byte achieveCompleteStep, int achieveAccumulate, CReceivedGoods receivedGoods, bool isLevelUp)
    {
        this.achieveAccumulate++;
    }

    void Listener(CDailyAchieve dailyAchieve, CReceivedGoods receivedGoods, bool isLevelUp)
    {
        achieveAccumulate++;
    }
}
