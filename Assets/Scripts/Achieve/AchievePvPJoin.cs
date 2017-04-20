
/// <summary>
/// 매칭참여
/// </summary>
public class AchievePvPJoin : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.battle.onLoadBattleScene += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.battle.onLoadBattleScene -= Listener;
        }
    }

    void Listener()
    {
        achieveAccumulate++;
    }
}
