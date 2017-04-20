using Common.Packet;
using System.Collections.Generic;

/// <summary>
/// 상자 열기
/// </summary>
public class DailyAchieveChestOpen : DailyAchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestOpen += Listener;
            Kernel.entry.treasure.onOpenTreasureBoxCallback += Listener;
            Kernel.entry.normalShop.onOpenBoxItem += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestOpen -= Listener;
            Kernel.entry.treasure.onOpenTreasureBoxCallback -= Listener;
            Kernel.entry.normalShop.onOpenBoxItem -= Listener;
        }
    }

    void Listener(int boxIndex, int gold, List<CBoxResult> boxResultList)
    {
        achieveAccumulate++;
    }

    void Listener(int boxIndex, int gold, List<CBoxResult> boxResultList, List<CTreasureBox> newBoxList)
    {
        achieveAccumulate++;
    }
}
