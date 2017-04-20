
/// <summary>
/// 골드수집
/// </summary>
public class AchieveGoldCollect : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onGoldUpdate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onGoldUpdate -= Listener;
        }
    }

    void Listener(int gold, int updateGold)
    {
        if (updateGold > 0)
        {
            achieveAccumulate = m_AchieveAccumulate + updateGold;
        }
    }
}
