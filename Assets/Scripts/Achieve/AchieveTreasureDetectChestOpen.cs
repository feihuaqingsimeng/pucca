using Common.Packet;

/// <summary>
/// 전체 섬 입장 횟수
/// </summary>
public class AchieveTreasureDetectChestOpen : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.detect.onDetectOpenBox += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.detect.onDetectOpenBox -= Listener;
        }
    }

    void Listener(PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK packet)
    {
        achieveAccumulate++;
    }
}
