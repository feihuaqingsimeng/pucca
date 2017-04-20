
/// <summary>
/// 스마일 포인트 획득 량
/// </summary>
public class AchieveFranchiseSmilePointGet : AchieveBase
{
    int m_SmilePoint;

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            m_SmilePoint = Kernel.entry.account.smilePoint;

            Kernel.entry.account.onGoodsUpdate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onGoodsUpdate -= Listener;
        }
    }

    void Listener(int friendship, int gold, int heart, int ranking, int ruby, int star, int guildPoint, int revengePoint, int smilePoint)
    {
        if (m_SmilePoint < Kernel.entry.account.smilePoint)
        {
            achieveAccumulate = achieveAccumulate + (Kernel.entry.account.smilePoint - m_SmilePoint);
            m_SmilePoint = Kernel.entry.account.smilePoint;
        }
    }
}
