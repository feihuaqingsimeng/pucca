
/// <summary>
/// 층 확장 횟수
/// </summary>
public class AchieveFranchiseRoomOpen : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.franchise.onRevRoomOpen += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.franchise.onRevRoomOpen -= Listener;
        }
    }

    void Listener()
    {
        achieveAccumulate++;
    }
}
