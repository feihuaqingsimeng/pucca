
/// <summary>
/// 캐릭터 성급업
/// </summary>
public class AchieveSkillLevelUp : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onSkillLevelUpCallback += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onSkillLevelUpCallback -= Listener;
        }
    }

    void Listener(long cid)
    {
        achieveAccumulate++;
    }
}
