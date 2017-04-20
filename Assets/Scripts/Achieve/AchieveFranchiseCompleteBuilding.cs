
/// <summary>
/// 가맹점 건물 완성수
/// </summary>
using System.Collections.Generic;
public class AchieveFranchiseCompleteBuilding : AchieveBase
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
        int buildingCount = 0;

        Dictionary<int, List<FranchiseRoomData>> allBuildingData = Kernel.entry.franchise.FindAllBuildingData();

        foreach (var buildingInfo in allBuildingData.Values)
        {
            bool isCompletBuilding = true;

            foreach (FranchiseRoomData floorInfo in buildingInfo)
            {
                if (floorInfo == null || !floorInfo.m_bOpened)
                    isCompletBuilding = false;
            }

            if(isCompletBuilding)
                buildingCount++;
        }

        achieveAccumulate = buildingCount;
    }
}
