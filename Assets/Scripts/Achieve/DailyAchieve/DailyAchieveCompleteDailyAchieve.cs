using Common.Packet;

/// <summary>
/// 일일 업적 완료
/// </summary>
public class DailyAchieveCompleteDailyAchieve : DailyAchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.achieve.onCompleteDailyAchieveResult += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.achieve.onCompleteDailyAchieveResult -= Listener;
        }
    }

    void Listener(CDailyAchieve dailyAchieve, CReceivedGoods receivedGoods, bool isLevelUp)
    {
        achieveAccumulate++;
    }
}
