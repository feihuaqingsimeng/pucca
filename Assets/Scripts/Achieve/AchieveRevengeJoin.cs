
/// <summary>
/// 복수전 참여
/// </summary>
public class AchieveRevengeJoin : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.revengeBattle.onStartedRevengeMatch += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.revengeBattle.onStartedRevengeMatch -= Listener;
        }
    }

    void Listener(long sequence)
    {
        achieveAccumulate++;
    }
}
