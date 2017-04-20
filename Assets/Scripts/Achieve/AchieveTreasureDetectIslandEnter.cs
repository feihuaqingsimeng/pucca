
public class AchieveTreasureDetectIslandEnter : AchieveBase
{
    public virtual int islandNum
    {
        get
        {
            return 0;
        }
    }

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.detect.onDetectStartAR += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.detect.onDetectStartAR -= Listener;
        }
    }

    void Listener()
    {
        if (islandNum == Kernel.entry.detect.islandNum)
        {
            achieveAccumulate++;
        }
    }
}
