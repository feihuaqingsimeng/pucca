using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 연승 달성
/// </summary>
public class AchievePvPSuccessiveWin : AchieveBase
{
    int m_WinningStreak;

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            m_WinningStreak = Kernel.entry.account.winningStreak;

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
        if (m_WinningStreak < Kernel.entry.account.winningStreak)
        {
            achieveAccumulate = achieveAccumulate + (Kernel.entry.account.winningStreak - m_WinningStreak);
            m_WinningStreak = Kernel.entry.account.winningStreak;
        }
    }
}
