
/// <summary>
/// 승점 보상 획득
/// </summary>
public class AchievePvPPointGet : AchieveBase
{
    int m_WinPoint;

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            m_WinPoint = Kernel.entry.account.winPoint;

            Kernel.entry.account.onUserBaseUpdate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onUserBaseUpdate -= Listener;
        }
    }

    void Listener(byte level, long exp, string name, byte currentPvPArea, int leaderCardIndex)
    {
        if (m_WinPoint < Kernel.entry.account.winPoint)
        {
            achieveAccumulate = achieveAccumulate + (Kernel.entry.account.winPoint - m_WinPoint);
            m_WinPoint = Kernel.entry.account.winPoint;
        }
    }
}
