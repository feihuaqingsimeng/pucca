
/// <summary>
/// 캐릭터 레벨업
/// </summary>
public class AchieveCardLevelUp : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardLevelUp += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardLevelUp -= Listener;
        }
    }

    void Listener(long cid)
    {
        achieveAccumulate++;
    }
}
