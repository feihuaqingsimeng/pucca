using System;

/// <summary>
/// 골드소모
/// </summary>
public class AchieveGoldSpend : AchieveBase
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
        if (updateGold < 0)
        {
            achieveAccumulate = m_AchieveAccumulate + Math.Abs(updateGold);
        }
    }
}
