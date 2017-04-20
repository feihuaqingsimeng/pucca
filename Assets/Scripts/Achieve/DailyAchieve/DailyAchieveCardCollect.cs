
/// <summary>
/// 카드 획득
/// </summary>
public class DailyAchieveCardCollect : DailyAchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardInfoUpdate += Listener;
            Kernel.entry.character.onSoulInfoUpdate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardInfoUpdate -= Listener;
            Kernel.entry.character.onSoulInfoUpdate -= Listener;
        }
    }

    void Listener(long cid, int cardIndex, bool isNew)
    {
        if (isNew)
        {
            achieveAccumulate++;
        }
    }

    void Listener(long sequence, int soulIndex, int soulCount, int updateCount)
    {
        if (updateCount > 0)
        {
            achieveAccumulate = m_AchieveAccumulate + updateCount;
        }
    }
}
