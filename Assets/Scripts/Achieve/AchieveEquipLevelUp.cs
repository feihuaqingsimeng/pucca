using Common.Util;

/// <summary>
/// 캐릭터 장비업
/// </summary>
public class AchieveEquipLevelUp : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onEquipmentLevelUpCallback += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onEquipmentLevelUpCallback -= Listener;
        }
    }

    void Listener(long cid, eGoodsType goodsType)
    {
        achieveAccumulate++;
    }
}
