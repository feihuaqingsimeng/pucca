using Common.Packet;
using System.Collections.Generic;

/// <summary>
/// 카드요청
/// </summary>
public class AchieveCardRequest : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.guild.onSupportRequestResult += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.guild.onSupportRequestResult -= Listener;
        }
    }

    void Listener(List<CGuildRequestCard> guildRequestCardList)
    {
        achieveAccumulate++;
    }
}
