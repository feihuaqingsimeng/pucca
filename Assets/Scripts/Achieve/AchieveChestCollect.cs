
/// <summary>
/// 상자획득
/// </summary>
public class AchieveChestCollect : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestAdd += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestAdd -= Listener;
        }
    }

    void Listener(int boxOrder, int boxIndex, int obtainArea)
    {
        achieveAccumulate++;
    }
}
