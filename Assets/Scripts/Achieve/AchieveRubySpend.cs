using System;

/// <summary>
/// 루비소모
/// </summary>
public class AchieveRubySpend : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onRubyUpdate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onRubyUpdate -= Listener;
        }
    }

    void Listener(int ruby, int updateRuby)
    {
        if (updateRuby < 0)
        {
            achieveAccumulate = m_AchieveAccumulate + Math.Abs(updateRuby);
        }
    }
}
