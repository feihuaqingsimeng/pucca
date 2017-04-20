
public class AchieveCardFind : AchieveBase
{
    public virtual Grade_Type gradeType
    {
        get
        {
            return Grade_Type.None;
        }
    }

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardInfoUpdate += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardInfoUpdate -= Listener;
        }
    }

    void Listener(long cid, int cardIndex, bool isNew)
    {
        if (isNew)
        {
            DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardIndex);
            if (card != null && card.Grade_Type == gradeType)
            {
                achieveAccumulate++;
            }
        }
    }
}
