
/// <summary>
/// 가맹점 재화 회수량
/// </summary>
public class AchieveFranchiseRewardGet : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.franchise.onRevRoomReward += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.franchise.onRevRoomReward -= Listener;
        }
    }

    void Listener(int buildingNum, int roomIndex, int lastUpdateTime)
    {
        achieveAccumulate++;
    }
}
