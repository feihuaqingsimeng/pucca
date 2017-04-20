using System.Collections;

/// <summary>
/// 1일1회 출석 수
/// </summary>
public class AchieveGameAttendance : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null && Kernel.entry.account.isFirstLogIn)
        {
            achieveAccumulate++;
            //Kernel.achieveManager.RemoveAchieveBase(this);
        }
    }
}
