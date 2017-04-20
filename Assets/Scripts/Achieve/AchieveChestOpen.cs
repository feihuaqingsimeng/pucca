using Common.Packet;
using System.Collections.Generic;

/// <summary>
/// 상자열기
/// </summary>
public class AchieveChestOpen : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestOpen += Listener;
            Kernel.entry.treasure.onOpenTreasureBoxCallback += Listener;
            Kernel.entry.normalShop.onOpenBoxItem += Listener;
            Kernel.entry.secretBusiness.onOpenSecretBoxCallback += Listener;
            Kernel.entry.detect.onDetectOpenBox += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestOpen -= Listener;
            Kernel.entry.treasure.onOpenTreasureBoxCallback -= Listener;
            Kernel.entry.normalShop.onOpenBoxItem -= Listener;
            Kernel.entry.secretBusiness.onOpenSecretBoxCallback -= Listener;
            Kernel.entry.detect.onDetectOpenBox -= Listener;
        }
    }

    void Listener(PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK packet)
    {
        achieveAccumulate++;
    }

    void Listener(int cardIndex)
    {
        achieveAccumulate++;
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
