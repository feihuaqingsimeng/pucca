using Common.Packet;
using System.Collections.Generic;

/// <summary>
/// 카드지원
/// </summary>
public class AchieveCardSupport : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.guild.onSupportResult += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.guild.onSupportResult -= Listener;
        }
    }

    void Listener(List<CGuildRequestCard> guildRequestCardList, long sequence)
    {
        achieveAccumulate++;
    }
}
